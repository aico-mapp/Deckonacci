namespace Game.Scripts.Game.DeviceAdaptation
{
    public interface IDeviceAdjustable
    {
        void AdjustResolution();
        bool IsIpad();
    }
}