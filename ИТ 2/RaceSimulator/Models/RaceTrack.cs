using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceSimulator.Models
{
    public class RaceTrack
    {
        public event Action<string>? OnEventLogged;

        private readonly List<RacingCar> _cars = new List<RacingCar>();
        private readonly List<Mechanic> _mechanics;
        private readonly ILoader _loader;

        public RaceTrack(int mechanicCount, ILoader loader)
        {
            _mechanics = Enumerable.Range(0, mechanicCount)
                .Select(_ => new Mechanic())
                .ToList();
            _loader = loader;
        }

        public void AddCar(RacingCar car)
        {
            car.NeedPitStop += OnNeedPitStop;
            car.Crashed += OnCarCrashed;
            _cars.Add(car);
        }

        private async void OnNeedPitStop(RacingCar car)
        {
            LogEvent($"{car.Name} требует пит-стоп");
            car.EnterPit();

            var availableMechanic = _mechanics.FirstOrDefault(m => !m.IsBusy);
            if (availableMechanic != null)
            {
                LogEvent($"Назначен механик для {car.Name}");
                await availableMechanic.ChangeTires(car);
                car.ExitPit();
            }
            else
            {
                LogEvent($"Нет свободных механиков для {car.Name}");
            }
        }

        private async void OnCarCrashed(RacingCar car)
        {
            LogEvent($"Обработка аварии {car.Name}");
            if (!_loader.IsBusy)
            {
                await _loader.MoveToAccident();
                _loader.PerformAction();

                await Task.Delay(3000);
                car.ExitPit();
                LogEvent($"{car.Name} восстановлен после аварии");
            }
            else
            {
                LogEvent($"Погрузчик занят, не может помочь {car.Name}");
            }
        }

        private void LogEvent(string message) => OnEventLogged?.Invoke($"[Трасса] {message}");
    }
}