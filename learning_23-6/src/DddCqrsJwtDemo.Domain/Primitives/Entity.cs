using System;

namespace DddCqrsJwtDemo.Domain.Primitives;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return ((Entity)obj).Id == Id;
    }

    public bool Equals(Entity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other.GetType() != GetType()) return false;
        return other.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() * 41;
    }

    public static bool operator ==(Entity? a, Entity? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(Entity? a, Entity? b)
    {
        return !(a == b);
    }
}
