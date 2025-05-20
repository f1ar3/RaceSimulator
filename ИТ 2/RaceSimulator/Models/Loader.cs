using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace RaceSimulator.Models
{
     public class Loader : ILoader
    {
        private readonly Random _random = new Random();
        
        public bool IsBusy { get; private set; }

        public async Task MoveToAccident()
        {
            IsBusy = true;
            await Task.Delay(1000); // Имитация движения
            IsBusy = false;
        }

        public async Task PerformAction()
        {
            IsBusy = true;
            await Task.Delay(_random.Next(1000, 3000)); // Имитация уборки
            IsBusy = false;
        }
    }
}