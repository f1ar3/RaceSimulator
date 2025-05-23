using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using RaceSimulator.Models;

namespace RaceSimulator.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly RaceTrack _raceTrack = new();

        public ObservableCollection<string> Logs { get; } = new();
        public ObservableCollection<RacingCar> Cars { get; } = new();

        private bool _isRacing;
        public bool IsRacing
        {
            get => _isRacing;
            set { _isRacing = value; OnPropertyChanged(); }
        }

        public ICommand AddCarCommand { get; }
        public ICommand AddMechanicCommand { get; }
        public ICommand AddLoaderCommand { get; }
        public ICommand StartRaceCommand { get; }
        public ICommand StopRaceCommand { get; }

        public MainWindowViewModel()
        {
            AddCarCommand = new RelayCommand(AddCar);
            AddMechanicCommand = new RelayCommand(AddMechanic);
            AddLoaderCommand = new RelayCommand(AddLoader);
            StartRaceCommand = new RelayCommand(async () => await StartRaceAsync());
            StopRaceCommand = new RelayCommand(StopRace);
        }

        private int _carCounter = 1;
        private int _mechCounter = 1;
        private int _loaderCounter = 1;

        private void AddCar()
        {
            string carName = $"Car {_carCounter++}";
            var car = new RacingCar(carName);
            
            car.TiresWornOut += (_, _) => AddLog($"{car.Name}: шины изношены!");
            car.Collided += (_, _) => AddLog($"{car.Name}: столкновение!");

            _raceTrack.Cars.Add(car);
            foreach (var mechanic in _raceTrack.Mechanics)
                mechanic.Subscribe(car);
            foreach (var loader in _raceTrack.Loaders)
            {
                if (loader is Loader concreteLoader)
                    concreteLoader.Subscribe(car);
            }

            Cars.Add(car);
            AddLog($"Добавлена машина: {car.Name}");
        }

        private void AddMechanic()
        {
            var name = $"Mechanic {_mechCounter++}";
            var mech = new Mechanic(name);
            _raceTrack.Mechanics.Add(mech);
            foreach (var car in _raceTrack.Cars)
                mech.Subscribe(car);
            AddLog($"Добавлен механик: {name}");
        }

        private void AddLoader()
        {
            var name = $"Loader {_loaderCounter++}";
            var loader = new Loader(name);
            _raceTrack.Loaders.Add(loader);
            foreach (var car in _raceTrack.Cars)
                loader.Subscribe(car);
            AddLog($"Добавлен погрузчик: {name}");
        }

        private async Task StartRaceAsync()
        {
            if (IsRacing) return;
            IsRacing = true;
            AddLog("🏁 Гонка началась!");
            await _raceTrack.StartRaceAsync();
            IsRacing = false;
            AddLog("🏁 Гонка завершена!");
        }

        private void StopRace()
        {
            if (!IsRacing) return;
            _raceTrack.StopRace();
            IsRacing = false;
            AddLog("⛔ Гонка остановлена вручную.");
        }

        private void AddLog(string message)
        {
            Logs.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
