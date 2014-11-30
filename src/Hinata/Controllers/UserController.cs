using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Hinata.Models;
using Hinata.Repositories;
using Hinata.Utilities;

namespace Hinata.Controllers
{
    [Authorize]
    [RoutePrefix("users")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IItemRepository _itemRepository;

        public UserController(IUserRepository userRepository, IItemRepository itemRepository)
        {
            if (userRepository == null) throw new ArgumentNullException("userRepository");
            if (itemRepository == null) throw new ArgumentNullException("itemRepository");

            _userRepository = userRepository;
            _itemRepository = itemRepository;
        }

        [HttpGet]
        [Route("{name}", Name="UserIndex")]
        public async Task<ActionResult> Index(string name)
        {
            var user = await User.GetCurrentAsync();
            var targetUser = await _userRepository.FindByNameAsync(name);

            return View(targetUser.Equals(user) ? "MyPage" : "Index", targetUser);
        }

        [HttpGet]
        [Route("partial/userpublic")]
        public async Task<ActionResult> GetUserPublic(string name, int skip, int take)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            var user = await User.GetCurrentAsync();

            var targetuser = await _userRepository.FindByNameAsync(name);
            if (targetuser == null) return HttpNotFound();

            var items = (await _itemRepository.GetUserPublicAsync(targetuser.Id, skip, take)).Select(x => UserItemSummaryViewModel.Create(x, user));

            return PartialView("_UserItemSummariesPartial", items);
        }

        [HttpGet]
        [Route("partial/userprivate")]
        public async Task<ActionResult> GetUserPrivate(string name, int skip, int take)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            var user = await User.GetCurrentAsync();

            var targetuser = await _userRepository.FindByNameAsync(name);
            if (targetuser == null) return HttpNotFound();
            if (!targetuser.Equals(user)) return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);

            var items = (await _itemRepository.GetUserPrivateAsync(targetuser.Id, skip, take)).Select(x => UserItemSummaryViewModel.Create(x, user));

            return PartialView("_UserItemSummariesPartial", items);
        }
    }
}