using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Hinata.Markdown;

namespace Hinata.Models
{
    public class DraftIndexViewModel
    {
        private readonly List<DraftSummaryViewModel> _contributedDrafts = new List<DraftSummaryViewModel>();
        private readonly List<DraftSummaryViewModel> _unContributedDrafts = new List<DraftSummaryViewModel>();

        public IReadOnlyCollection<DraftSummaryViewModel> ContributedDrafts
        {
            get { return _contributedDrafts; }
        }

        public IReadOnlyCollection<DraftSummaryViewModel> UnContributedDrafts
        {
            get { return _unContributedDrafts; }
        }

        public DraftIndexViewModel(IReadOnlyCollection<Draft> drafts, User user)
        {
            _contributedDrafts =
                drafts.Where(x => x.IsContributed).Select(x => DraftSummaryViewModel.Create(x, user)).ToList();
            _unContributedDrafts =
                drafts.Where(x => !x.IsContributed).Select(x => DraftSummaryViewModel.Create(x, user)).ToList();
        }
    }

    public class DraftSummaryViewModel
    {
        public string Id { get; private set; }

        public string Title { get; private set; }

        public bool UnTitled { get; private set; }

        public string UserName { get; private set; }

        public DateTime RegisterDateTime { get; private set; }

        public bool IsContributed { get; private set; }

        public IReadOnlyCollection<Tag> Tags { get; private set; }

        private DraftSummaryViewModel()
        {
        }

        public static DraftSummaryViewModel Create(Draft draft, User user)
        {
            var model = new DraftSummaryViewModel
            {
                Id = draft.Id,
                Title = string.IsNullOrWhiteSpace(draft.Title) ? "タイトル未設定" : draft.Title,
                UnTitled = string.IsNullOrWhiteSpace(draft.Title),
                UserName = draft.User.UserName,
                RegisterDateTime = TimeZoneInfo.ConvertTimeFromUtc(draft.RegisterDateTimeUtc, user.TimeZoneInfo),
                IsContributed = draft.IsContributed,
                Tags = draft.Tags,
            };

            return model;
        }
    }

    public class DraftEditViewModel : IValidatableObject
    {
        public Draft Entity { get; private set; }

        public string Id
        {
            get { return Entity.Id; }
            set { Entity.Id = value; }
        }

        [AllowHtml]
        [Display(Name = "タイトル")]
        public string Title
        {
            get { return Entity.Title; }
            set { Entity.Title = string.IsNullOrEmpty(value) ? "" : value.Trim(); }
        }

        [Required]
        [AllowHtml]
        [Display(Name = "本文")]
        public string Body
        {
            get { return Entity.Body; }
            set { Entity.Body = string.IsNullOrEmpty(value) ? "" : value.Trim(); }
        }

        [Display(Name = "タグ")]
        public TagDetailCollection Tags
        {
            get { return Entity.Tags; }
        }

        [Display(Name = "タグ（複数登録する場合はカンマ区切りで入力してください）")]
        public string TagInlineString
        {
            get { return CreateTagInlineString(Entity.Tags); }
            set
            {
                Entity.Tags.Clear();
                var orderNo = 1;
                foreach (var tag in CreateTagDetailCollectionFromInlineText(value))
                {
                    tag.OrderNo = orderNo;
                    Entity.Tags.Add(tag);
                    orderNo++;
                }
            }
        }

        public User User
        {
            get { return Entity.User; }
            set { Entity.User = value; }
        }

        public DraftRegisterMode RegisterMode { get; set; }

        public DraftEditViewModel()
            : this(new Draft())
        {
        }

        public DraftEditViewModel(Draft draft)
        {
            Entity = draft;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RegisterMode != DraftRegisterMode.Draft && string.IsNullOrWhiteSpace(Title))
            {
                yield return new ValidationResult("タイトルは必須です。", new[] { "Title" });
            }
        }

        private static string CreateTagInlineString(IReadOnlyList<TagDetail> tags)
        {
            var ret = "";
            for (var i = 0; i < tags.Count; i++)
            {
                var tag = tags[i];
                if (i == 0)
                {
                    ret += CreateTagVersionString(tag);
                }
                else
                {
                    ret += "," + CreateTagVersionString(tag);
                }
            }

            return ret;
        }

        private static string CreateTagVersionString(TagDetail tag)
        {
            var ret = tag.Name;
            if (!string.IsNullOrWhiteSpace(tag.Version))
            {
                ret += string.Format("[{0}]", tag.Version);
            }
            return ret;
        }

        private static IEnumerable<TagDetail> CreateTagDetailCollectionFromInlineText(string inlineText)
        {
            return inlineText.Split(',').Select(CreateTagDetailFromText).Where(tag => tag != null);
        }

        private static TagDetail CreateTagDetailFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var regex = new Regex(@"\[(.+?)\]");
            if (!regex.IsMatch(text))
            {
                return new TagDetail(text.Trim());
            }
            var match = regex.Match(text);
            var name = text.Replace(match.Groups[0].Value, "").Trim();
            var version = match.Groups[1].Value.Trim();
            return string.IsNullOrWhiteSpace(name) ? null : new TagDetail(name) {Version = version};
        }
    }

    public enum DraftRegisterMode
    {
        Post = 0,
        Private = 1,
        Draft = 2,
    }

    public class DraftPreviewViewModel
    {
        public string Id { get; private set; }

        public string Title { get; private set; }

        public bool UnTitled { get; private set; }

        public string Html { get; private set; }

        public User User { get; private set; }

        public TagDetailCollection Tags { get; private set; }

        private DraftPreviewViewModel()
        {
        }

        public static DraftPreviewViewModel Create(Draft draft, IMarkdownParser parser)
        {
            var model = new DraftPreviewViewModel
            {
                Id = draft.Id,
                Title = string.IsNullOrWhiteSpace(draft.Title) ? "タイトル未設定" : draft.Title,
                UnTitled = string.IsNullOrWhiteSpace(draft.Title),
                Html = parser.Transform(draft.Body),
                User = draft.User,
                Tags = draft.Tags
            };

            return model;
        }
    }
}