using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DddCqrsJwtDemo.Domain.Primitives;

namespace DddCqrsJwtDemo.Domain.ValueObjects;

public class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email cannot be empty.", nameof(value));
        }

        var trimmedValue = value.Trim();

        if (!EmailRegex.IsMatch(trimmedValue))
        {
            throw new ArgumentException("Email is invalid.", nameof(value));
        }

        return new Email(trimmedValue);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
