using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Hinata.Models;
using Hinata.Repositories;
using Hinata.Utilities;

namespace Hinata.Controllers
{
    [Authorize]
    [RoutePrefix("images")]
    public class ImageController : Controller
    {
        private readonly IImageRepository _imageRepository;

        private readonly string[] _permittedExtensions = {"png", "gif", "jpg", "jpeg", "bmp"};
        private const int MaxLength = 2097152; // 2MB

        public ImageController(IImageRepository imageRepository)
        {
            if (imageRepository == null) throw new ArgumentNullException("imageRepository");

            _imageRepository = imageRepository;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<ActionResult> Upload(string itemid)
        {
            var file = Request.Files[0];

            if (file == null || file.ContentLength <= 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (file.ContentLength > MaxLength) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (string.IsNullOrWhiteSpace(file.FileName)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ext = ext.Replace(".", "");
            if (_permittedExtensions.All(x => x != ext)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await User.GetCurrentAsync();

            var data = new byte[file.ContentLength];
            await file.InputStream.ReadAsync(data, 0, file.ContentLength);
            var image = new Image(file.FileName, data, file.ContentType, itemid, user);
            await _imageRepository.SaveAsync(image);
            var imageUrl = Url.Content(@"~/images/" + image.UniqueFileName);
            return Json(new { image.FileName, Url = imageUrl });
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<ActionResult> Get(string name)
        {
            var image = await _imageRepository.FindAsync(name);
            return new FileContentResult(image.Data, image.ContentType);
        }
    }
}