using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Hinata.Markdown;
using Hinata.Models;
using Hinata.Repositories;
using Hinata.Utilities;
using Microsoft.AspNet.Identity;

namespace Hinata.Controllers
{
    [Authorize]
    [RoutePrefix("drafts")]
    public class DraftController : Controller
    {
        private readonly IDraftRepository _draftRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMarkdownParser _parser;

        public DraftController(IDraftRepository draftRepository, IItemRepository itemRepository, IMarkdownParser parser)
        {
            if (draftRepository == null) throw new ArgumentNullException("draftRepository");
            if (itemRepository == null) throw new ArgumentNullException("itemRepository");
            if (parser == null) throw new ArgumentNullException("parser");

            _draftRepository = draftRepository;
            _itemRepository = itemRepository;
            _parser = parser;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var drafts = await _draftRepository.GetByUserIdAsync(User.Identity.GetUserId());
            var user = await User.GetCurrentAsync();

            return View(new DraftIndexViewModel(drafts.ToArray(), user));
        }

        [HttpGet]
        [Route("new")]
        public async Task<ActionResult> Create()
        {
            var draft = new Draft
            {
                Title = "",
                Body = "",
                User = await User.GetCurrentAsync(),
                RegisterDateTimeUtc = DateTime.UtcNow,
            };
            await _draftRepository.SaveAsync(draft);

            return RedirectToRoute("DraftEdit", new { id = draft.Id });
        }

        [HttpGet]
        [Route("edit/{id}", Name = "DraftEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            var user = await User.GetCurrentAsync();
            var draft = await _draftRepository.FindByIdAsync(id);
            if (draft == null || draft.User.Id != user.Id) return HttpNotFound();

            return View(new DraftEditViewModel(draft));
        }

        [HttpPost]
        [Route("edit")]
        public async Task<ActionResult> Edit(DraftEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await User.GetCurrentAsync();

            var draft = model.Entity;
            draft.RegisterDateTimeUtc = DateTime.UtcNow;
            draft.User = user;

            if (model.RegisterMode == DraftRegisterMode.Draft)
            {
                await _draftRepository.SaveAsync(draft);

                return RedirectToAction("Index");
            }

            var original = await _itemRepository.FindByIdAsync(model.Id);

            if (original != null && original.User.Id != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            var item = model.Entity;

            if (model.RegisterMode == DraftRegisterMode.Private)
            {
                item.IsPrivate = true;
            }

            await _itemRepository.SaveAsync(item);

            await _draftRepository.DeleteAsync(draft);

            return RedirectToRoute("ItemIndex", new { id = item.Id });
        }


        [HttpGet]
        [Route("delete/{id}", Name = "DraftDelete")]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await User.GetCurrentAsync();
            var draft = await _draftRepository.FindByIdAsync(id);
            if (draft == null || draft.User.Id != user.Id) return HttpNotFound();

            await _draftRepository.DeleteAsync(draft);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("preview")]
        public async Task<ActionResult> Preview(string id)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            var draft = await _draftRepository.FindByIdAsync(id);
            if (draft == null || draft.User.Id != User.Identity.GetUserId()) return HttpNotFound();

            return PartialView("_DraftPreviewPartial", DraftPreviewViewModel.Create(draft, _parser));
        }

        [HttpPost]
        [ValidateInput(false)]
        [Route("transform")]
        public ActionResult Transform(string markdown)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            var html = _parser.Transform(markdown);

            return PartialView("_EditPreviewPartial",html);
        }
    }
}