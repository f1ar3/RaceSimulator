using System;

namespace RaceSimulator.Models
{
    public class Loader : ILoader
    {
        public string Name { get; }

        public Loader(string name)
        {
            Name = name;
        }

        public void Subscribe(RacingCar car)
        {
            car.Collided += OnCarCrashed;
        }

        private void OnCarCrashed(object? sender, EventArgs e)
        {
            if (sender is RacingCar car)
            {
                Console.WriteLine($"[Погрузчик {Name}] Эвакуирует {car.Name} после аварии...");
                Load(car);
            }
        }

        public void Load(RacingCar car)
        {
            car.FixDamage();
        }
    }
}