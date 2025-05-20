using System;
using System.Threading.Tasks;

namespace RaceSimulator.Models
{
    public class RacingCar
    {
        public event Action<RacingCar>? NeedPitStop;
        public event Action<RacingCar>? Crashed;
        public event Action<string>? OnEventLogged;

        private readonly Random _random = new Random();
        private bool _isRacing;
        private double _tireWear;
        private bool _isInPit;
        private bool _isCrashed;

        public string Name { get; }
        public double TireWear
        {
            get => _tireWear;
            set => _tireWear = Math.Round(value, 2);
        }

        public RacingCar(string name, double startX, double startY)
        {
            Name = name;
        }

        public void StartRace()
        {
            _isRacing = true;
            LogEvent("Гонка начата");
            Task.Run(() => RaceLoop());
        }

        public void StopRace()
        {
            _isRacing = false;
            LogEvent("Гонка остановлена");
        }

        private async void RaceLoop()
        {
            while (_isRacing)
            {
                if (_isCrashed || _isInPit)
                {
                    await Task.Delay(100);
                    continue;
                }

                TireWear += _random.NextDouble() * 0.1;
                if (TireWear > 1 && !_isInPit)
                {
                    LogEvent($"Покрышки изношены (уровень: {TireWear:F2}), требуется пит-стоп");
                    NeedPitStop?.Invoke(this);
                }

                if (_random.NextDouble() < 0.01)
                {
                    LogEvent("АВАРИЯ!");
                    _isCrashed = true;
                    Crashed?.Invoke(this);
                }

                await Task.Delay(100);
            }
        }

        public void EnterPit()
        {
            _isInPit = true;
            LogEvent("Заехал на пит-стоп");
        }

        public void ExitPit()
        {
            _isInPit = false;
            _isCrashed = false;
            TireWear = 0;
            LogEvent("Покинул пит-стоп");
        }

        private void LogEvent(string message) => OnEventLogged?.Invoke($"[{Name}] {message}");
    }
}