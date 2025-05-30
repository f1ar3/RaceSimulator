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
        public bool HasFinished { get; private set; }
        private const double FinishLine = 100;
        public DateTime? FinishTime { get; private set; }

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

        public double CanvasX => Position * 8;

        public string Color => IsBroken ? "Gray" : "Red";
        public string BorderColor => NeedsTireChange ? "Yellow" : "Transparent";

        public event EventHandler? TiresWornOut;
        public event EventHandler? Collided;

        public RacingCar(string name)
        {
            Name = name;
            Position = 0;
        }

        public async Task StartRaceAsync()
        {
            _isRacing = true;

            while (_isRacing && !IsBroken && !HasFinished)
            {
                await Task.Delay(500);
                Position += _random.NextDouble() * 5;
                if (Position >= FinishLine)
                {
                    HasFinished = true;
                    FinishTime = DateTime.Now;
                    StopRace();
                }
                
                if (!NeedsTireChange && _random.NextDouble() < 0.01)
                {
                    NeedsTireChange = true;
                    OnPropertyChanged(nameof(BorderColor));
                    TiresWornOut?.Invoke(this, EventArgs.Empty);
                }

                if (_random.NextDouble() < 0.01)
                {
                    IsBroken = true;
                    OnPropertyChanged(nameof(Color));
                    Collided?.Invoke(this, EventArgs.Empty);
                }
            }
        }


        public void StopRace() => _isRacing = false;

        public void FixDamage()
        {
            IsBroken = false;
            OnPropertyChanged(nameof(Color));

            // Продолжить гонку
            if (!_isRacing)
                _ = StartRaceAsync();
        }


        public void ChangeTires()
        {
            NeedsTireChange = false;
            OnPropertyChanged(nameof(BorderColor));
        }

        private double _verticalOffset;
        public double VerticalOffset
        {
            get => _verticalOffset;
            set
            {
                if (Math.Abs(_verticalOffset - value) > 0.1)
                {
                    _verticalOffset = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private bool _isRepairing;
        public bool IsRepairing
        {
            get => _isRepairing;
            set
            {
                if (_isRepairing != value)
                {
                    _isRepairing = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsRemoved { get; private set; }

        public void MarkAsUnrepairable()
        {
            IsRemoved = true;
            StopRace();
            OnPropertyChanged(nameof(IsRemoved));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}