using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Hinata.Identity;
using Hinata.Models;

namespace Hinata.Controllers
{
    [Authorize]
    [RoutePrefix("manage")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("index")]
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "パスワードが変更されました。"
                : message == ManageMessageId.SetTwoFactorSuccess ? "2 要素認証プロバイダーが設定されました。"
                : message == ManageMessageId.Error ? "エラーが発生しました。"
                : "";

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                //TwoFactor = await _userManager.GetTwoFactorEnabledAsync(User.Identity.GetUserId()),
                //BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId())
            };
            return View(model);
        }

        //[HttpPost]
        //public async Task<ActionResult> EnableTwoFactorAuthentication()
        //{
        //    await _userManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId().ToGuid(), true);
        //    var user = await _userManager.FindByIdAsync(User.Identity.GetUserId().ToGuid());
        //    //if (user != null)
        //    //{
        //    //    await SignInAsync(user, isPersistent: false);
        //    //}
        //    return RedirectToAction("Index", "Manage");
        //}

        //[HttpPost]
        //public async Task<ActionResult> DisableTwoFactorAuthentication()
        //{
        //    await _userManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId().ToGuid(), false);
        //    var user = await _userManager.FindByIdAsync(User.Identity.GetUserId().ToGuid());
        //    //if (user != null)
        //    //{
        //    //    await SignInAsync(user, isPersistent: false);
        //    //}
        //    return RedirectToAction("Index", "Manage");
        //}

        [HttpGet]
        [Route("password/change")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("password/change")]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = _userManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            Error
        }
    }
}