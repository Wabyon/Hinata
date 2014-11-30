namespace Hinata.Models
{
    public class TagDetail : Tag
    {
        public TagDetail(string name) : base(name)
        {
        }

        public string Version { get; set; }

        public int OrderNo { get; set; }
    }
}
