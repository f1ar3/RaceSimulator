using System;

namespace RaceSimulator.Models
{
    public class Loader : ILoader
    {
        public string Name { get; }
        
        public event Action<RacingCar>? CarRemoved;

        public Loader(string name)
        {
            Name = name;
        }

        public void Subscribe(RacingCar car)
        {
            car.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(car.IsRemoved) && car.IsRemoved)
                {
                    Load(car);
                }
            };
        }

        public void Load(RacingCar car)
        {
            car.StopRace();
            
            CarRemoved?.Invoke(car);
        }
    }
}