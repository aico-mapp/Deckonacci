using System;

namespace Calendar.Scripts.Data.Progress
{
    public interface IPropertyChanged
    {
        event Action OnPropertyChanged;
    }
}