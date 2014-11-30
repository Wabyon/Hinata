using System;
using System.Collections.Generic;

namespace Hinata.Models
{
    public class UserItemSummaryViewModel
    {
        public string Id { get; private set; }

        public string Title { get; private set; }

        public User User { get; private set; }

        public bool IsPrivate { get; private set; }

        public DateTime RegisterDateTime { get; private set; }

        public IReadOnlyCollection<TagDetail> Tags { get; private set; }

        private UserItemSummaryViewModel()
        {
        }

        public static UserItemSummaryViewModel Create(Item item, User user)
        {
            var model = new UserItemSummaryViewModel
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