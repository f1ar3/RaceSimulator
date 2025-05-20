using System;
using System.Threading;
using System.Threading.Tasks;

namespace RaceSimulator.Models
{
    public class Mechanic
    {
        private readonly Random _random = new Random();
        
        public bool IsBusy { get; private set; }

        public async Task ChangeTires(RacingCar car)
        {
            IsBusy = true;
            await Task.Run(() =>
            {
                Thread.Sleep(_random.Next(2000, 5000));
                car.TireWear = 0;
                IsBusy = false;
            });
        }
    }
}