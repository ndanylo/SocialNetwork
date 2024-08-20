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
