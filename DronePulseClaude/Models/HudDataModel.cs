using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DronePulseClaude.Models
{
    public class HudDataModel:INotifyPropertyChanged

    {
        private float _airspeed;
        private float _groundspeed;
        private float _altitude;
        private float _climb;
        private short _heading;
        private short _throttle;
        private DateTime _lastUpdate;
        private bool _isValid;

        public float Airspeed
        {
            get => _airspeed;
            set
            {
                if (Math.Abs(_airspeed - value) > 0.01f)
                {
                    _airspeed = value;
                    OnPropertyChanged(nameof(Airspeed));
                }
            }
        }

        public float Groundspeed
        {
            get => _groundspeed;
            set
            {
                if (Math.Abs(_groundspeed - value) > 0.01f)
                {
                    _groundspeed = value;
                    OnPropertyChanged(nameof(Groundspeed));
                }
            }
        }

        public float Altitude
        {
            get => _altitude;
            set
            {
                if (Math.Abs(_altitude - value) > 0.01f)
                {
                    _altitude = value;
                    OnPropertyChanged(nameof(Altitude));
                }
            }
        }

        public float Climb
        {
            get => _climb;
            set
            {
                if (Math.Abs(_climb - value) > 0.01f)
                {
                    _climb = value;
                    OnPropertyChanged(nameof(Climb));
                }
            }
        }

        public short Heading
        {
            get => _heading;
            set
            {
                if (_heading != value)
                {
                    _heading = value;
                    OnPropertyChanged(nameof(Heading));
                }
            }
        }

        public short Throttle
        {
            get => _throttle;
            set
            {
                if (_throttle != value)
                {
                    _throttle = value;
                    OnPropertyChanged(nameof(Throttle));
                }
            }
        }

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
            Airspeed = 0;
            Groundspeed = 0;
            Altitude = 0;
            Climb = 0;
            Heading = 0;
            Throttle = 0;
            IsValid = false;
            LastUpdate = DateTime.MinValue;
        }
    }

}
