using System;

namespace RaceSimulator.Models
{
    public class Mechanic
    {
        public string Name { get; }

        public Mechanic(string name)
        {
            Name = name;
        }

        public void Subscribe(RacingCar car)
        {
            car.TiresWornOut += OnTiresWornOut;
        }

        private void OnTiresWornOut(object? sender, EventArgs e)
        {
            if (sender is RacingCar car)
            {
                Console.WriteLine($"[Механик {Name}] Заменяет шины у машины {car.Name}...");
                car.ChangeTires();
            }
        }
    }
}