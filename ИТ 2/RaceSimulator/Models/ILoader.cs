using System.Threading.Tasks;

namespace RaceSimulator.Models
{
public interface ILoader
    {
        bool IsBusy { get; }
        Task MoveToAccident();
        Task PerformAction();
    }
}