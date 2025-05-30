using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace RaceSimulator.Models
{
    public class Mechanic
    {
        public string Name { get; }

        private readonly ConcurrentQueue<RacingCar> _repairQueue = new();
        private bool _isRunning;
        private Task? _processingTask;

        public event Action<string>? LogRequested;
        public event Action<RacingCar>? RepairFailed;

        public Mechanic(string name)
        {
            Name = name;
            _isRunning = true;
            _processingTask = Task.Run(ProcessRepairs);
        }

        public void Subscribe(RacingCar car)
        {
            car.Collided += (_, _) =>
            {
                if (!car.IsBroken || car.IsRemoved) return;

                car.StopRace();
                _repairQueue.Enqueue(car);
                LogRequested?.Invoke($"🔧 {Name} получил заявку на ремонт {car.Name}");
            };
        }

        private async Task ProcessRepairs()
        {
            while (_isRunning)
            {
                if (_repairQueue.TryDequeue(out var car))
                {
                    if (!car.IsBroken || car.IsRemoved) continue;

                    car.IsRepairing = true;
                    LogRequested?.Invoke($"🔧 {Name} начал чинить {car.Name}");

                    await Task.Delay(3000);

                    bool success = new Random().NextDouble() < 0.8;
                    if (success)
                    {
                        car.FixDamage();
                        LogRequested?.Invoke($"✅ {Name} успешно починил {car.Name}");
                    }
                    else
                    {
                        car.MarkAsUnrepairable();
                        LogRequested?.Invoke($"❌ {Name} не смог починить {car.Name}");
                        RepairFailed?.Invoke(car);
                    }

                    car.IsRepairing = false;
                }
                else
                {
                    await Task.Delay(300);
                }
            }
        }

        public void Stop() => _isRunning = false;
    }
}
