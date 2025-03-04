using System.Threading.Tasks;

namespace EnglishDraughtsProject.Services;

public interface IGameLogicService
{
    Task<string> GetHintAsync();
    bool Move(int fromX, int fromY, int toX, int toY);
}