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

        public void AddCar(string name)
        {
            var car = new RacingCar(name);
            
            foreach (var mechanic in Mechanics)
                mechanic.Subscribe(car);

            foreach (var loader in Loaders.OfType<Loader>())
                loader.Subscribe(car);

            Cars.Add(car);
        }

        public void AddMechanic(string name)
        {
            Mechanics.Add(new Mechanic(name));
        }

        public void AddLoader(string name)
        {
            Loaders.Add(new Loader(name));
        }

        public async Task StartRaceAsync()
        {
            var tasks = Cars.Select(car => car.StartRaceAsync()).ToList();
            await Task.WhenAll(tasks);
        }

        public void StopRace()
        {
            foreach (var car in Cars)
                car.StopRace();
        }

        // Рефлексия: создаёт объект по имени класса (например, "RacingCar")
        public object? CreateModelByName(string className, params object[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = assembly.GetTypes().FirstOrDefault(t => t.Name == className);
            if (type == null) return null;
            return Activator.CreateInstance(type, args);
        }
    }
}