using System;
using System.Linq;
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
    [RoutePrefix("items")]
    public class ItemController : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMarkdownParser _parser;

        public ItemController(IItemRepository itemRepository,
            IMarkdownParser parser)
        {
            if (itemRepository == null) throw new ArgumentNullException("itemRepository");
            if (parser == null) throw new ArgumentNullException("parser");

            _itemRepository = itemRepository;
            _parser = parser;
        }

        [HttpGet]
        [Route("{id}", Name = "ItemIndex")]
        public async Task<ActionResult> Index(string id)
        {
            var item = await _itemRepository.FindByIdAsync(id);

            if (item == null) return HttpNotFound();

            var user = await User.GetCurrentAsync();

            return View(ItemViewModel.Create(item, _parser, user));
        }

        [HttpGet]
        [Route("{id}/delete", Name = "ItemDelete")]
        public async Task<ActionResult> Delete(string id)
        {
            var item = await _itemRepository.FindByIdAsync(id);

            if (item == null || item.User.Id != User.Identity.GetUserId()) return HttpNotFound();

            await _itemRepository.DeleteAsync(item);

            return RedirectToAction("Drafts");
        }

        [HttpGet]
        [Route("partial/following")]
        public async Task<ActionResult> GetFollowing(int skip, int take)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            var user = await User.GetCurrentAsync();

            var items = (await _itemRepository.GetFollowingAsync(User.Identity.GetUserId(), skip, take)).Select(x => ItemSummaryViewModel.Create(x, user));

            return PartialView("_ItemSummariesPartial", items);
        }

        [HttpGet]
        [Route("partial/public")]
        public async Task<ActionResult> GetPublic(int skip, int take)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            var user = await User.GetCurrentAsync();

            var items = (await _itemRepository.GetPublicAsync(User.Identity.GetUserId(), skip, take)).Select(x => ItemSummaryViewModel.Create(x, user));

            return PartialView("_ItemSummariesPartial", items);
        }
    }
}