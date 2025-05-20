using System;
using System.Collections.ObjectModel;
using System.Text;
using RaceSimulator.Models;
using ReactiveUI;

namespace RaceSimulator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly RaceTrack _raceTrack;
        private bool _isRaceRunning;
        private string _statusMessage = "Готов к старту";
        private readonly StringBuilder _consoleOutputBuilder = new StringBuilder();

        public ObservableCollection<RacingCar> Cars { get; } = new ObservableCollection<RacingCar>();
        private string _consoleOutput = string.Empty;
 public string ConsoleOutput
        {
            get => _consoleOutput;
            private set => this.RaiseAndSetIfChanged(ref _consoleOutput, value);
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public bool IsRaceRunning
        {
            get => _isRaceRunning;
            set => this.RaiseAndSetIfChanged(ref _isRaceRunning, value);
        }

        public MainWindowViewModel()
        {
            _raceTrack = new RaceTrack(3, new Loader());
            _raceTrack.OnEventLogged += LogEvent;

            InitializeCars();
        }

        private void InitializeCars()
        {
            var cars = new[]
            {
                new RacingCar("Red Bull", 0, 0),
                new RacingCar("Ferrari", 0, 0),
                new RacingCar("Mercedes", 0, 0),
                new RacingCar("McLaren", 0, 0)
            };

            foreach (var car in cars)
            {
                car.OnEventLogged += LogEvent;
                _raceTrack.AddCar(car);
                Cars.Add(car);
            }
        }

        private void LogEvent(string message)
        {
            _consoleOutputBuilder.AppendLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            ConsoleOutput = _consoleOutputBuilder.ToString();
        }

        public void StartRace()
        {
            if (IsRaceRunning) return;

            IsRaceRunning = true;
            StatusMessage = "Гонка началась!";
            foreach (var car in Cars)
            {
                car.StartRace();
            }
        }

        public void StopRace()
        {
            if (!IsRaceRunning) return;

            IsRaceRunning = false;
            StatusMessage = "Гонка остановлена";
            foreach (var car in Cars)
            {
                car.StopRace();
            }
        }

        public void AddRandomCar()
        {
            var random = new Random();
            var carNames = new[] { "Williams", "Alpine", "Aston Martin", "AlphaTauri", "Alfa Romeo", "Haas" };
            var name = carNames[random.Next(carNames.Length)];

            var car = new RacingCar(name, 0, 0);
            car.OnEventLogged += LogEvent;
            _raceTrack.AddCar(car);
            Cars.Add(car);
            StatusMessage = $"Добавлен новый болид: {name}";
        }
    }
}