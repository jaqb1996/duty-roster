using System;

namespace ClassLibrary.Models
{
    public interface ISchedulePresentationData
    {
        int Id { get; }
        DateTime LastDay { get; }
        string Name { get; }
        DateTime StartingDay { get; }
    }
}