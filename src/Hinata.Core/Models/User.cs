using System;

namespace Hinata.Models
{
    public class User : IEquatable<User>
    {
        public string Id { get; private set; }

        public string UserName { get; private set; }

        public TimeZoneInfo TimeZoneInfo { get; set; }

        public User(string id, string userName)
        {
            Id = id;
            UserName = userName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other)) return false;

            return ReferenceEquals(this, other) || string.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}
