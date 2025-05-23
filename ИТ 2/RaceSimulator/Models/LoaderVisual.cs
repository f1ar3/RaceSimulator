using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RaceSimulator.Models
{
    public class LoaderVisual : INotifyPropertyChanged
    {
        public string Name { get; }
        public RacingCar? Target { get; private set; }

        private double _x;
        public double X
        {
            get => _x;
            set
            {
                if (Math.Abs(_x - value) > 0.1)
                {
                    _x = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _y;
        public double Y
        {
            get => _y;
            set
            {
                if (Math.Abs(_y - value) > 0.1)
                {
                    _y = value;
                    OnPropertyChanged();
                }
            }
        }

        public LoaderVisual(string name)
        {
            Name = name;
        }

        public async Task MoveToAsync(RacingCar car)
        {
            Target = car;
            Y = car.VerticalOffset;

            while (X < car.CanvasX - 30)
            {
                X += 5;
                await Task.Delay(50);
            }

            await Task.Delay(1000); 
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}