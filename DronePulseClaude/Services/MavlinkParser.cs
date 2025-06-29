using DronePulse2.UAVState;
using DronePulseClaude.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MAVLink;

namespace DronePulseClaude.Services
{
    public class MavlinkParser
    {
        private readonly MavlinkParse _parser;
        private readonly Dictionary<byte, byte> _systemComponentMap;

        public HudDataModel HudData { get; private set; }
        public CurrentState cs { get; private set; }
        public AttitudeDataModel AttitudeData { get; private set; }

        public event EventHandler<HudDataModel> HudDataUpdated;
        public event EventHandler<AttitudeDataModel> AttitudeDataUpdated;
        public event EventHandler<string> MessageParsed;
        public event EventHandler<string> ParseError;

        public MavlinkParser()
        {
            _parser = new MavlinkParse();
            _systemComponentMap = new Dictionary<byte, byte>();

            HudData = new HudDataModel();
            AttitudeData = new AttitudeDataModel();
            cs = new CurrentState();
        }

        //public void ParseData(byte[] data)
        //{
        //    foreach (byte b in data)
        //    {
        //        try
        //        {
        //            MAVLinkMessage message = _parser.ReadPacket();
        //            if (message != null)
        //            {
        //                ProcessMessage(message);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ParseError?.Invoke(this, $"Error parsing byte 0x{b:X2}: {ex.Message}");
        //        }
        //    }
        //}
        // Option 1: Process the entire byte array at once
        public void ParseData(byte[] data)
        {
            try
            {
                using (var stream = new MemoryStream(data))
                {
                    MAVLinkMessage message;
                    while ((message = _parser.ReadPacket(stream)) != null)
                    {
                        ProcessMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                ParseError?.Invoke(this, $"Error parsing data: {ex.Message}");
            }
        }

        private void ProcessMessage(MAVLinkMessage message)
        {
            try
            {
                // Track system/component mapping
                _systemComponentMap[message.sysid] = message.compid;

                switch (message.msgid)
                {
                    case (uint)MAVLink.MAVLINK_MSG_ID.VFR_HUD:
                        ProcessVfrHudMessage(message);
                        break;

                    case (uint)MAVLink.MAVLINK_MSG_ID.ATTITUDE:
                        ProcessAttitudeMessage(message);
                        break;

                    case (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT:
                        ProcessHeartbeatMessage(message);
                        break;

                    case (uint)MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT:
                        ProcessGlobalPositionMessage(message);
                        break;

                    default:
                        MessageParsed?.Invoke(this, $"Received message: {Enum.GetName(typeof(MAVLink.MAVLINK_MSG_ID), message.msgid)} (ID: {message.msgid}) from System {message.sysid}, Component {message.compid}");
                        break;
                }
            }
            catch (Exception ex)
            {
                ParseError?.Invoke(this, $"Error processing message ID {message.msgid}: {ex.Message}");
            }
        }

        private void ProcessVfrHudMessage(MAVLinkMessage message)
        {
            var vfrHud = (mavlink_vfr_hud_t)message.data;

            HudData.Airspeed = vfrHud.airspeed;
            HudData.Groundspeed = vfrHud.groundspeed;
            HudData.Altitude = vfrHud.alt;
            HudData.Climb = vfrHud.climb;
            HudData.Heading = vfrHud.heading;
            //HudData.Throttle = vfrHud.throttle;
            HudData.LastUpdate = DateTime.Now;
            HudData.IsValid = true;

            MessageParsed?.Invoke(this, $"VFR_HUD: AS={vfrHud.airspeed:F1}m/s, GS={vfrHud.groundspeed:F1}m/s, Alt={vfrHud.alt:F1}m, Hdg={vfrHud.heading}°, Thr={vfrHud.throttle}%");
            HudDataUpdated?.Invoke(this, HudData);
        }

        private void ProcessAttitudeMessage(MAVLinkMessage message)
        {
            var attitude = (mavlink_attitude_t)message.data;

            AttitudeData.Roll = attitude.roll;
            AttitudeData.Pitch = attitude.pitch;
            AttitudeData.Yaw = attitude.yaw;
            AttitudeData.Rollspeed = attitude.rollspeed;
            AttitudeData.Pitchspeed = attitude.pitchspeed;
            AttitudeData.Yawspeed = attitude.yawspeed;
            AttitudeData.LastUpdate = DateTime.Now;
            AttitudeData.IsValid = true;

            MessageParsed?.Invoke(this, $"ATTITUDE: Roll={AttitudeData.RollDegrees:F1}°, Pitch={AttitudeData.PitchDegrees:F1}°, Yaw={AttitudeData.YawDegrees:F1}°");
            AttitudeDataUpdated?.Invoke(this, AttitudeData);
        }

        private void ProcessHeartbeatMessage(MAVLinkMessage message)
        {
            var heartbeat = (mavlink_heartbeat_t)message.data;
            string vehicleType = Enum.GetName(typeof(MAV_TYPE), heartbeat.type) ?? "Unknown";
            string flightMode = heartbeat.custom_mode.ToString();

            MessageParsed?.Invoke(this, $"HEARTBEAT: Type={vehicleType}, Mode={flightMode}, Armed={((heartbeat.base_mode & (byte)MAV_MODE_FLAG.SAFETY_ARMED) != 0)}");
        }

        private void ProcessGlobalPositionMessage(MAVLinkMessage message)
        {
            var globalPos = (mavlink_global_position_int_t)message.data;

            double lat = globalPos.lat / 1e7;
            double lon = globalPos.lon / 1e7;
            double alt = globalPos.alt / 1000.0;

            MessageParsed?.Invoke(this, $"GLOBAL_POSITION: Lat={lat:F6}°, Lon={lon:F6}°, Alt={alt:F1}m");
        }

        public void Reset()
        {
            HudData.Reset();
            AttitudeData.Reset();
            _systemComponentMap.Clear();
        }
    
}
}
