using System;
using System.Collections.Generic;
using Hinata.Markdown;

namespace Hinata.Models
{
    public class ItemViewModel
    {
        public string Id { get; private set; }

        public string Title { get; private set; }

        public string Html { get; private set; }

        public User User { get; private set; }

        public bool IsPrivate { get; private set; }

        public DateTime RegisterDateTime { get; private set; }

        public IReadOnlyCollection<TagDetail> Tags { get; private set; }

        public bool CanEdit { get; private set; }

        private ItemViewModel()
        {            
        }

        public static ItemViewModel Create(Item item, IMarkdownParser parser, User user)
        {
            var model = new ItemViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Html = parser.Transform(item.Body),
                User = item.User,
                IsPrivate = item.IsPrivate,
                RegisterDateTime = TimeZoneInfo.ConvertTimeFromUtc(item.RegisterDateTimeUtc, user.TimeZoneInfo),
                Tags = item.Tags,
                CanEdit = item.User.Equals(user),
            };

            return model;
        }
    }

    public class ItemSummaryViewModel
    {
        public string Id { get; private set; }

        public string Title { get; private set; }

        public User User { get; private set; }

        public bool IsPrivate { get; private set; }

        public DateTime RegisterDateTime { get; private set; }

        public IReadOnlyCollection<TagDetail> Tags { get; private set; }

        private ItemSummaryViewModel()
        {
        }

        public static ItemSummaryViewModel Create(Item item, User user)
        {
            var model = new ItemSummaryViewModel
            {
                Id = item.Id,
                Title = item.Title,
                User = item.User,
                IsPrivate = item.IsPrivate,
                RegisterDateTime = TimeZoneInfo.ConvertTimeFromUtc(item.RegisterDateTimeUtc, user.TimeZoneInfo),
                Tags = item.Tags,
            };

            return model;
        }
    }
}