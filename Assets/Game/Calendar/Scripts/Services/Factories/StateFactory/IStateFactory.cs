using Game.Calendar.Scripts.Services;

namespace Calendar.Scripts.Services.Factories.StateFactory
{
    public interface IStateFactory : IGlobalService
    {
        void CreateAllStates();
    }
}