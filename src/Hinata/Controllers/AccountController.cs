using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Hinata.Identity;
using Hinata.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Hinata.Controllers
{
    [Authorize]
    [RoutePrefix("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser, string> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser, string> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("login")]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_LoginPartial", model);
            }

            var user = await _userManager.FindByNameAsync(model.UserNameOrEmail) ??
                       await _userManager.FindByEmailAsync(model.UserNameOrEmail);
            if (user != null && (await _userManager.FindAsync(user.UserName, model.Password)) != null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(user.Id))
                {
                    ModelState.AddModelError("", "メールの確認が完了していません。");
                    return PartialView("_LoginPartial", model);
                }
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserNameOrEmail, model.Password, true, false);

            switch (result)
            {
                case SignInStatus.Success:
                    return Json(new { RedirectUrl = Url.Action("Index", "Home") });
                case SignInStatus.RequiresVerification:
                    return Json(new { RedirectUrl = Url.Action("SendCode", "Account", new { RememberMe = true }) });
                default:
                    ModelState.AddModelError("", "ユーザー名かパスワードが間違っています");
                    return PartialView("_LoginPartial", model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return PartialView("_RegisterPartial", model);

            ApplicationUser user;
            if (GlobalSetting.AllowSetUserName)
            {
                user = new ApplicationUser {UserName = model.UserName, Email = model.Email};
            }
            else
            {
                var userName = new MailAddress(model.Email).User;
                user = new ApplicationUser { UserName = userName, Email = model.Email };
            }
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, Request.Url.Scheme);
                await _userManager.SendEmailAsync(user.Id, "アカウントの確認", "このリンクをクリックすることによってアカウントを確認してください <a href=\"" + callbackUrl + "\">こちら</a>");

                return Json(new { RedirectUrl = Url.Action("RegisterConfirmation", "Account") });
            }
            AddErrors(result);

            return PartialView("_RegisterPartial", model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("verifycode")]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                default:
                    ModelState.AddModelError("", "無効なコード。");
                    return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("email/confirm")]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (code == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("password/forgot")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("password/forgot")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // ユーザーが存在しないことや未確認であることを公開しません。
                    return View("ForgotPasswordConfirmation");
                }

                // アカウント確認とパスワード リセットを有効にする方法の詳細については、http://go.microsoft.com/fwlink/?LinkID=320771 を参照してください
                // このリンクを含む電子メールを送信します
                var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, Request.Url.Scheme);
                await _userManager.SendEmailAsync(user.Id, "パスワード", "のリセット <a href=\"" + callbackUrl + "\">こちら</a> をクリックして、パスワードをリセットしてください");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // ここで問題が発生した場合はフォームを再表示します
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("password/forgot/confirm")]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("password/reset")]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("password/reset")]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // ユーザーが存在しないことを公開しません。
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("password/reset/confirm")]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("register/confirm")]
        public ActionResult RegisterConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("sendcode")]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await _signInManager.GetVerifiedUserIdAsync();
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("sendcode")]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // トークンを生成して送信します。
            if (!await _signInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("logoff")]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}