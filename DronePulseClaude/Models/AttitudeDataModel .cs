using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DronePulseClaude.Models
{
    public class AttitudeDataModel:INotifyPropertyChanged
    {
        private float _roll;
        private float _pitch;
        private float _yaw;
        private float _rollspeed;
        private float _pitchspeed;
        private float _yawspeed;
        private DateTime _lastUpdate;
        private bool _isValid;

        public float Roll
        {
            get => _roll;
            set
            {
                if (Math.Abs(_roll - value) > 0.001f)
                {
                    _roll = value;
                    OnPropertyChanged(nameof(Roll));
                    OnPropertyChanged(nameof(RollDegrees));
                }
            }
        }

        public float Pitch
        {
            get => _pitch;
            set
            {
                if (Math.Abs(_pitch - value) > 0.001f)
                {
                    _pitch = value;
                    OnPropertyChanged(nameof(Pitch));
                    OnPropertyChanged(nameof(PitchDegrees));
                }
            }
        }

        public float Yaw
        {
            get => _yaw;
            set
            {
                if (Math.Abs(_yaw - value) > 0.001f)
                {
                    _yaw = value;
                    OnPropertyChanged(nameof(Yaw));
                    OnPropertyChanged(nameof(YawDegrees));
                }
            }
        }

        public float Rollspeed
        {
            get => _rollspeed;
            set
            {
                if (Math.Abs(_rollspeed - value) > 0.001f)
                {
                    _rollspeed = value;
                    OnPropertyChanged(nameof(Rollspeed));
                }
            }
        }

        public float Pitchspeed
        {
            get => _pitchspeed;
            set
            {
                if (Math.Abs(_pitchspeed - value) > 0.001f)
                {
                    _pitchspeed = value;
                    OnPropertyChanged(nameof(Pitchspeed));
                }
            }
        }

        public float Yawspeed
        {
            get => _yawspeed;
            set
            {
                if (Math.Abs(_yawspeed - value) > 0.001f)
                {
                    _yawspeed = value;
                    OnPropertyChanged(nameof(Yawspeed));
                }
            }
        }

        // Helper properties for degrees
        public float RollDegrees => Roll * (180.0f / (float)Math.PI);
        public float PitchDegrees => Pitch * (180.0f / (float)Math.PI);
        public float YawDegrees => Yaw * (180.0f / (float)Math.PI);

        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set
            {
                if (_lastUpdate != value)
                {
                    _lastUpdate = value;
                    OnPropertyChanged(nameof(LastUpdate));
                }
            }
        }

        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Reset()
        {
            Roll = 0;
            Pitch = 0;
            Yaw = 0;
            Rollspeed = 0;
            Pitchspeed = 0;
            Yawspeed = 0;
            IsValid = false;
            LastUpdate = DateTime.MinValue;
        }
    }
}
