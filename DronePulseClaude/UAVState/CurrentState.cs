using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DronePulse2.UAVState
{
    // Independent CurrentState class like MissionPlanner
    public class CurrentState : ICloneable, IDisposable
    {
        // System identification
        public byte sysid { get; set; }
        public byte compid { get; set; }
        public MAVLink.MAV_TYPE type { get; set; }
        public MAVLink.MAV_AUTOPILOT autopilot { get; set; }

        // Time tracking
        public DateTime lastupdate { get; set; }
        public DateTime heartbeat { get; set; }

        // Flight status
        public bool armed { get; set; }
        public string mode { get; set; } = "";
        public byte basemode { get; set; }
        public uint custommode { get; set; }

        // Position and orientation
        public double lat { get; set; }
        public double lng { get; set; }
        public float alt { get; set; }
        public float altasl { get; set; } // Above sea level
        public float roll { get; set; }
        public float pitch { get; set; }
        public float yaw { get; set; }

        // Velocities
        public float groundspeed { get; set; }
        public float airspeed { get; set; }
        public float climb { get; set; }
        public float vx { get; set; }
        public float vy { get; set; }
        public float vz { get; set; }

        // Angular rates
        public float rollspeed { get; set; }
        public float pitchspeed { get; set; }
        public float yawspeed { get; set; }

        // GPS data
        public byte gpsstatus { get; set; }
        public byte satcount { get; set; }
        public float gpshdop { get; set; }
        public float gpsvel { get; set; }

        // Battery and power
        public float battery_voltage { get; set; }
        public int battery_remaining { get; set; }
        public float current { get; set; }
        public float battery_usedmah { get; set; }

        // System status
        public float load { get; set; }
        public byte rssi { get; set; }
        public byte remrssi { get; set; }

        // RC channels input
        public ushort ch1in { get; set; }
        public ushort ch2in { get; set; }
        public ushort ch3in { get; set; }
        public ushort ch4in { get; set; }
        public ushort ch5in { get; set; }
        public ushort ch6in { get; set; }
        public ushort ch7in { get; set; }
        public ushort ch8in { get; set; }

        // Servo outputs
        public ushort ch1out { get; set; }
        public ushort ch2out { get; set; }
        public ushort ch3out { get; set; }
        public ushort ch4out { get; set; }
        public ushort ch5out { get; set; }
        public ushort ch6out { get; set; }
        public ushort ch7out { get; set; }
        public ushort ch8out { get; set; }

        // Throttle percentage
        public byte ch3percent { get; set; }

        // Navigation data
        public float nav_roll { get; set; }
        public float nav_pitch { get; set; }
        public int nav_bearing { get; set; }
        public ushort target_bearing { get; set; }
        public ushort wp_dist { get; set; }
        public float alt_error { get; set; }
        public float aspd_error { get; set; }
        public float xtrack_error { get; set; }

        // Wind estimation
        public float wind_dir { get; set; }
        public float wind_vel { get; set; }

        // Mission status
        public ushort wp_seq { get; set; }
        public ushort mission_count { get; set; }

        // Vibration
        public float vibex { get; set; }
        public float vibey { get; set; }
        public float vibez { get; set; }

        // EKF status
        public bool ekf_bad { get; set; }
        public float ekf_poshorizrel { get; set; }
        public float ekf_poshorizabs { get; set; }
        public float ekf_posvert { get; set; }
        public float ekf_compass { get; set; }
        public float ekf_velocity { get; set; }

        // Terrain data
        public float terrain_alt { get; set; }

        // Failsafe
        public bool failsafe { get; set; }

        // Constructor
        public CurrentState()
        {
            lastupdate = DateTime.MinValue;
            heartbeat = DateTime.MinValue;
        }

        // ICloneable implementation
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        // IDisposable implementation
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources if any
                }
                disposed = true;
            }
        }

        ~CurrentState()
        {
            Dispose(false);
        }

        // Helper methods
        public bool IsConnected()
        {
            return (DateTime.Now - heartbeat).TotalSeconds < 10;
        }

        public bool HasGPSFix()
        {
            return gpsstatus >= 3;
        }

        public double DistanceToHome(double homeLat, double homeLng)
        {
            return GetDistance(lat, lng, homeLat, homeLng);
        }

        private double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371000; // Earth's radius in meters
            var dLat = ToRadians(lat2 - lat1);
            var dLng = ToRadians(lng2 - lng1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
