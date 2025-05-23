using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using RaceSimulator.Models;
using Avalonia.Threading;

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
        
        public ICommand ResetRaceCommand { get; }
        
        public ObservableCollection<Mechanic> Mechanics { get; } = new();
        public ObservableCollection<Loader> Loaders { get; } = new();

        public MainWindowViewModel()
        {
            AddCarCommand = new RelayCommand(AddCar);
            AddMechanicCommand = new RelayCommand(AddMechanic);
            AddLoaderCommand = new RelayCommand(AddLoader);
            StartRaceCommand = new RelayCommand(async () => await StartRaceAsync());
            StopRaceCommand = new RelayCommand(StopRace);
            ResetRaceCommand = new RelayCommand(ResetRace);
        }

        private int _carCounter = 1;
        private int _mechCounter = 1;
        private int _loaderCounter = 1;

        private void AddCar()
        {
            string carName = $"Car {_carCounter++}";
            var car = new RacingCar(carName)
            {
                VerticalOffset = Cars.Count * 40
            };

            
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
            Mechanics.Add(mech);
            
            foreach (var car in _raceTrack.Cars)
                mech.Subscribe(car);

            mech.LogRequested += message =>
            {
                Dispatcher.UIThread.Post(() => AddLog(message));
            };

            mech.RepairFailed += async car =>
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    var loader = Loaders.FirstOrDefault();
                    if (loader != null)
                    {
                        var visual = new LoaderVisual(loader.Name)
                        {
                            X = 0,
                            Y = car.VerticalOffset
                        };

                        LoaderVisuals.Add(visual);
                        await visual.MoveToAsync(car);

                        Cars.Remove(car);
                        LoaderVisuals.Remove(visual);
                        AddLog($"🚜 {loader.Name} увёз {car.Name} с трассы");
                    }
                    else
                    {
                        AddLog($"⚠️ Нет доступных погрузчиков для эвакуации {car.Name}");
                    }
                });
            };

            AddLog($"Добавлен механик: {name}");
        }


        private void AddLoader()
        {
            var name = $"Loader {_loaderCounter++}";
            var loader = new Loader(name);

            loader.CarRemoved += async car =>
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    var visual = new LoaderVisual(loader.Name)
                    {
                        X = 0,
                        Y = car.VerticalOffset
                    };

                    LoaderVisuals.Add(visual);
                    
                    await visual.MoveToAsync(car);

                    await Task.Delay(1000);

                    Cars.Remove(car);
                    LoaderVisuals.Remove(visual);

                    AddLog($"🚜 {loader.Name} увёз {car.Name} с трассы.");
                });
            };


            _raceTrack.Loaders.Add(loader);
            Loaders.Add(loader);

            foreach (var car in _raceTrack.Cars)
                loader.Subscribe(car);

            AddLog($"Добавлен погрузчик: {name}");
        }
        
        private async Task StartRaceAsync()
        {
            if (IsRacing) return;
            IsRacing = true;
            AddLog("🏁 Гонка началась!");

            var raceTasks = Cars.Select(async car =>
            {
                await car.StartRaceAsync();
                if (car.HasFinished)
                {
                    Dispatcher.UIThread.Post(() =>
                        AddLog($"🏁 {car.Name} пересёк финишную черту!"));
                }
            });

            await Task.WhenAll(raceTasks);

            IsRacing = false;

            var winner = Cars
                .Where(c => c.HasFinished && c.FinishTime.HasValue)
                .OrderBy(c => c.FinishTime)
                .FirstOrDefault();

            if (winner != null)
                AddLog($"🥇 Победитель: {winner.Name}!");

            AddLog("🏁 Все участники завершили гонку.");
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
        
        private void ResetRace()
        {
            _raceTrack.StopRace();
            Cars.Clear();
            _raceTrack.Cars.Clear();
            AddLog("🔄 Гонка сброшена.");
        }
        
        public ObservableCollection<LoaderVisual> LoaderVisuals { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
