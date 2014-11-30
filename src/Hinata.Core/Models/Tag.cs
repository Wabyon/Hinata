using System;

namespace Hinata.Models
{
    public class Tag : IEquatable<Tag>
    {
        public string Name { get; private set; }

        public Tag(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Tag;
            return Equals(other);
        }

        public bool Equals(Tag other)
        {
            return other != null && string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
