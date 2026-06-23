using System;
using System.Collections.Generic;
using DddCqrsJwtDemo.Domain.Primitives;

namespace DddCqrsJwtDemo.Domain.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string ZipCode { get; }

    private Address(string street, string city, string zipCode)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
    }

    public static Address Create(string street, string city, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street is required.", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZipCode is required.", nameof(zipCode));

        return new Address(street.Trim(), city.Trim(), zipCode.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return ZipCode;
    }

    public override string ToString() => $"{Street}, {City} {ZipCode}";
}
