using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RaceSimulator.Models
{
    public class RacingCar : INotifyPropertyChanged
    {
        private static readonly Random _random = new();
        private bool _isRacing;
        private double _position;

        public string Name { get; }
        public bool IsBroken { get; private set; }
        public bool NeedsTireChange { get; private set; }

        public event EventHandler? TiresWornOut;
        public event EventHandler? Collided;

        public double Position
        {
            get => _position;
            private set
            {
                if (Math.Abs(_position - value) > 0.01)
                {
                    _position = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanvasX));
                }
            }
        }

        public double CanvasX => Position * 5;

        public RacingCar(string name)
        {
            Name = name;
            Position = 0;
        }

        public async Task StartRaceAsync()
        {
            _isRacing = true;

            while (_isRacing && !IsBroken)
            {
                await Task.Delay(500);

                Position += _random.NextDouble() * 5;

                if (!NeedsTireChange && _random.NextDouble() < 0.05)
                {
                    NeedsTireChange = true;
                    TiresWornOut?.Invoke(this, EventArgs.Empty);
                }

                if (_random.NextDouble() < 0.02)
                {
                    IsBroken = true;
                    Collided?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void StopRace() => _isRacing = false;

        public void FixDamage()
        {
            IsBroken = false;
        }

        public void ChangeTires()
        {
            NeedsTireChange = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
