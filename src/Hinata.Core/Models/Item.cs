using System;

namespace Hinata.Models
{
    public class Item
    {
        private readonly TagDetailCollection _tags = new TagDetailCollection();

        public string Id { get; set; }

        public string Title { get; set; }

        public User User { get; set; }

        public bool IsPrivate { get; set; }

        public DateTime RegisterDateTimeUtc { get; set; }

        public string Body { get; set; }

        public TagDetailCollection Tags { get { return _tags; } }
    }
}
