using System;
using System.Linq;

namespace Hinata.Models
{
    public class Draft : Item
    {
        public bool IsContributed { get; set; }

        public Draft() : this(NewId())
        {
        }

        public Draft(string id)
        {
            Id = id;
        }

        private static string NewId()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, 20)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
        }
    }
}