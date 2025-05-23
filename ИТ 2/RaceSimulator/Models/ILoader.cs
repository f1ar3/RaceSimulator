namespace RaceSimulator.Models
{
    public interface ILoader
    {
        string Name { get; }

        void Load(RacingCar car);
    }
}