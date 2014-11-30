using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hinata.Models;
using Hinata.Repositories;
using Microsoft.WindowsAzure.Storage;

namespace Hinata.Azure.Storage
{
    public class ImageRepository : IImageRepository
    {
        private readonly CloudStorageAccount _account;

        private const string ContainerName = @"hinataimageblob";

        public ImageRepository(string storageConnectionString)
        {
            _account = CloudStorageAccount.Parse(storageConnectionString);
        }

        public async Task SaveAsync(Image image)
        {
            var client = _account.CreateCloudBlobClient();
            var container = client.GetContainerReference(ContainerName);
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlockBlobReference(image.UniqueFileName);
            blob.Metadata["FileName"] = Uri.EscapeDataString(image.FileName);
            blob.Metadata["ItemId"] = image.ItemId;
            blob.Metadata["UserId"] = image.User.Id;
            blob.Metadata["UserName"] = image.User.UserName;
            blob.Properties.ContentType = image.ContentType;

            await blob.UploadFromByteArrayAsync(image.Data, 0, image.Data.Count());
        }

        public async Task<Image> FindAsync(string name)
        {
            var client = _account.CreateCloudBlobClient();
            var container = client.GetContainerReference(ContainerName);

            var blob = container.GetBlockBlobReference(name);

            byte[] data;
            using (var ms = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(ms);
                data = ms.ToArray();
            }

            var image = new Image(Uri.UnescapeDataString(blob.Metadata["FileName"]), data, blob.Properties.ContentType, blob.Metadata["ItemId"],
                new User(blob.Metadata["UserId"], blob.Metadata["UserName"]), name);

            return image;
        }
    }
}
