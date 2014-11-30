using System;
using System.IO;

namespace Hinata.Models
{
    public class Image
    {
        public string UniqueFileName { get; private set; }
        public string FileName { get; private set; }
        public byte[] Data { get; private set; }
        public string ContentType { get; private set; }
        public string ItemId { get; private set; }
        public User User { get; private set; }

        public Image(string fileName, byte[] data, string contentType, string itemId, User user)
            : this(fileName, data, contentType, itemId, user, CreateUniqueFileName(fileName))
        {
        }

        public Image(string fileName, byte[] data, string contentType, string itemId, User user, string uniqueFileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("fileName is empty", "fileName");
            if (data == null) throw new ArgumentNullException("data");
            if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException("contentType is empty", "contentType");
            if (string.IsNullOrWhiteSpace(itemId)) throw new ArgumentException("itemId is empty", "itemId");
            if (user == null) throw new ArgumentNullException("user");
            if (string.IsNullOrWhiteSpace(uniqueFileName)) throw new ArgumentException("uniqueFileName is empty", "uniqueFileName");

            FileName = fileName;
            Data = data;
            ContentType = contentType;
            ItemId = itemId;
            User = user;
            UniqueFileName = uniqueFileName;
        }

        private static string CreateUniqueFileName(string fileName)
        {
            return Guid.NewGuid() + Path.GetExtension(fileName);
        }
    }
}
