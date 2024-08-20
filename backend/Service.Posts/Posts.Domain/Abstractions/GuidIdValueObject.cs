using CSharpFunctionalExtensions;
using System.Reflection;

namespace Notifications.Domain.Abstractions
{
    public abstract class GuidIdValueObject<T> : ValueObject where T : GuidIdValueObject<T>
    {
        public Guid Id { get; }

        protected GuidIdValueObject(Guid id)
        {
            Id = id;
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Id;
        }

        public static implicit operator Guid(GuidIdValueObject<T> valueObject)
        {
            return valueObject.Id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is GuidIdValueObject<T> other)
            {
                return Id.Equals(other.Id);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(GuidIdValueObject<T> a, GuidIdValueObject<T> b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.Id == b.Id;
        }

        public static bool operator !=(GuidIdValueObject<T> a, GuidIdValueObject<T> b)
        {
            return !(a == b);
        }

        public static T Empty => CreateEmptyInstance();

        private static T CreateEmptyInstance()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Guid) }, null);
            if (constructor == null)
                throw new InvalidOperationException("A constructor with a Guid parameter was not found.");

            return (T)constructor.Invoke(new object[] { Guid.Empty });
        }
    }
}
