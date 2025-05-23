using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RaceSimulator.Models
{
    public class RaceTrack
    {
        public List<RacingCar> Cars { get; } = new();
        public List<Mechanic> Mechanics { get; } = new();
        public List<ILoader> Loaders { get; } = new();

        public async Task StartRaceAsync()
        {
            var tasks = Cars.Select(car => Task.Run(() => car.StartRaceAsync())).ToList();

            while (true)
            {
                await Task.Delay(200);

                if (Cars.Any(c => c.HasFinished))
                {
                    StopRace();
                    break;
                }

                if (tasks.All(t => t.IsCompleted))
                    break;
            }
        }

        public void StopRace()
        {
            foreach (var car in Cars)
                car.StopRace();
        }

        public object? CreateModelByName(string className, params object[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = assembly.GetTypes().FirstOrDefault(t => t.Name == className);
            return type == null ? null : Activator.CreateInstance(type, args);
        }
    }
}