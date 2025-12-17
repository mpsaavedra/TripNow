using System;

namespace TripNow.Domain.Entities;

public class Country
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string RiskCode { get; private set; }

    private Country() { }

    public Country(string name, string riskCode)
    {
        Id = Guid.NewGuid();
        Name = name;
        RiskCode = riskCode;
    }
}
