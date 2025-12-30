using System;

namespace TripNow.Domain.Entities;

public class Country : Common.BaseEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Status { get; private set; }

    private Country() { }

    public Country(string name, string status)
    {
        Id = Guid.NewGuid();
        Name = name;
        Status = status;
    }
}
