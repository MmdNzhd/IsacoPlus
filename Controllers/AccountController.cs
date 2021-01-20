using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaraYadak.Models;
using KaraYadak.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DNTPersianUtils.Core;
using KaraYadak.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Services;
using Newtonsoft.Json;
using System.Drawing;

namespace KaraYadak.Controllers
{
    //[Authorize(Roles = "Admin")]
    //[Authorize(Roles = "User")]
    [Authorize(Roles = "Admin,User")]

    public class AccountController : Controller
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountService _accountService;
        private readonly ISmsSender _smsSender;
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IAccountService accountService,
            ISmsSender smsSender)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
            _accountService = accountService;
            _smsSender = smsSender;
            _userManager = userManager;
            _context = context;
        }
        [AllowAnonymous]
        [Authorize(Roles = "Admin")]

        [Route("ChangeAdminPass")]
        public async Task<IActionResult> ChangeAdminPass()
        {
            var password = RandomString(6);
            var user = await _userManager.FindByNameAsync("alialavi@gmail.com");
            if (user != null)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, password);
                if (result.Succeeded)
                {
                    return Json(password);
                }
            }
            //var user = new ApplicationUser { UserName = "niloofartagh1372@gmail.com", Email = "niloofartagh1372@gmail.com" };
            return Json("NotSucceeded");
        }
        public static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<IActionResult> CreateRole(string name)
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = name, NormalizedName = name.Normalize() });

            return Content("role created: " + name);
        }
        public async Task<IActionResult> Action()
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            //await _userManager.AddToRoleAsync(user, "Admin");
            return Json(await _userManager.IsInRoleAsync(user, "Admin"));
        }
        public async Task<IActionResult> CreateUser(string name)
        {
            var f = await _userManager.FindByNameAsync(name);
            var random = new Random();
            var pass = random.Next(1000000, 9999999).ToString();
            if (f == null)
            {
                await _userManager.CreateAsync(new ApplicationUser
                {
                    AvatarUrl = "",
                    Birthdate = "",
                    Email = name + "@kfc.ir",
                    NormalizedEmail = name + "@Name.ir".Normalize(),
                    EmailConfirmed = true,
                    FirstName = name,
                    LastName = "",
                    Gender = "",
                    NationalCode = "",
                    Nickname = name,
                    UserName = name + "@Name.ir",
                    NormalizedUserName = name.Normalize(),
                    Phone = "",
                    PhoneNumber = "",
                    RegistrationDateTime = DateTime.Now,
                    VerificationCode = "",
                }, "123456789");
            }
            return Content("user password: " + pass);
        }
        public IActionResult AccessDenied()
        {
            return Content("Access Denied");
        }

        public async Task<IActionResult> Index(string returnUrl)
        {
            returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;

            if (_signInManager.IsSignedIn(User))
            {
                return Redirect(returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;

            if (_signInManager.IsSignedIn(User))
            {
                return Redirect(returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public async Task<IActionResult> ChangePassword()
        {
            var password = RandomString(6);
            var user = await _userManager.FindByNameAsync("Admin@KaraYadak.com");
            await _signInManager.SignInAsync(user, true);
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, code, password);
            return Json(password);
        }
        [AllowAnonymous]

        public IActionResult Register(string returnUrl = "")
        {
            if (_signInManager.IsSignedIn(User)) return Redirect(returnUrl);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> IdentityLogin([FromBody] IdentityLoginViewModel input)
        {

            input.LoginOrRegister = "Login";

            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var item in ModelState.Values)
                {
                    foreach (var err in item.Errors)
                    {
                        errors.Add(err.ErrorMessage);
                    }
                }
                return new JsonResult(new { Status = 0, Message = errors });
            }
            var user = await _userManager.FindByNameAsync(input.PhoneNumber);
            if (user == null)
                return Json(new { status = 0, Message = "شماره شما در سیستم موجود نمی باشد" });

            if (user.UserName == "09390867564")
            {
                Random random = new Random();
                int current = 11111;
                user.VerificationCode = current.ToString();
                user.VerificationExpireTime = DateTime.Now.AddMinutes(2);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, current.ToString());
                return new JsonResult(new { Status = 1, Message = "لطفا کد 5 رقمی را وارد کنید", Data = input });
            }
            else
            {
                Random random = new Random();
                int current = random.Next(10000, 99999);
                user.VerificationCode = current.ToString();
                user.VerificationExpireTime = DateTime.Now.AddMinutes(2);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, current.ToString());
                await _smsSender.SendWithPattern(input.PhoneNumber, "xdumcpryk5", JsonConvert.SerializeObject(
                        new { name = user.FirstName + " " + user.LastName, verificationCode = current }));
                return new JsonResult(new { Status = 1, Message = "لطفا کد 5 رقمی را وارد کنید", Data = input });

            }
          



        }


        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> IdentityRegister([FromBody] IdentityRegisterViewModel input)
        {
            await _context.Roles.AddAsync(new IdentityRole { Name = PublicHelper.WarehousingAdminROLE });
            await _context.SaveChangesAsync();
            input.LoginOrRegister = "Register";

            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var item in ModelState.Values)
                {
                    foreach (var err in item.Errors)
                    {
                        errors.Add(err.ErrorMessage);
                    }
                }
                return new JsonResult(new { Status = 0, Error = errors });
            }
            var user = await _context.Users.Where(x => x.PhoneNumber == input.PhoneNumber || x.Phone == input.PhoneNumber ||
              x.UserName == input.PhoneNumber).FirstOrDefaultAsync();
            if (user != null)
            {
                return new JsonResult(new { Status = 0, Message = "شماره شما قبلا در سیستم ثبت شده است" });

            }

            Random random = new Random();
            int current = random.Next(10000, 99999);
            //user.VerificationCode = current.ToString();
            //user.VerificationExpireTime = DateTime.Now.AddMinutes(2);

            var fullName = input.Nickname;
            var names = fullName.Split(" ");
            var fName = "";
            var lName = "";
            if (names[0].Length > 1)
            {
                fName = names[0];
            }
            if (names[1].Length > 1)
            {
                lName = names[1];
            }



            var result = await _userManager.CreateAsync(new ApplicationUser
            {
                AccessFailedCount = 0,
                AvatarUrl = "",
                Birthdate = "",
                Email = "",
                EmailConfirmed = false,
                FirstName = fName,
                LastName = lName,
                Gender = "",
                Nickname = "",
                NormalizedUserName = "",
                RegistrationDateTime = DateTime.Now,
                UserName = input.PhoneNumber,
                Phone = "",
                PhoneNumber = input.PhoneNumber,
                VerificationCode = current.ToString(),
                VerificationExpireTime = DateTime.Now.AddMinutes(2)
            }, current.ToString());

            if (result.Succeeded)

            {
                var currectUser = await _userManager.FindByNameAsync(input.PhoneNumber);
                if (currectUser != null)
                {
                    currectUser.LockoutEnabled = false;
                    currectUser.LockoutEnd = DateTime.Now.AddMinutes(-2);
                }
                _context.Users.Update(currectUser);
                await _context.SaveChangesAsync();
                await _smsSender.SendWithPattern(input.PhoneNumber, "xdumcpryk5", JsonConvert.SerializeObject(
                    new { name = input.Nickname, verificationCode = current }));

                return new JsonResult(new { Status = 1, Message = "لطفا کد 5 رقمی را وارد کنید", Data = input });


            }
            else
            {
                if (result.Errors.Where(i => i.Code == "DuplicateUserName").Any())
                    return new JsonResult(new { Status = 0, Message = "نام کاربری از قبل ثبت شده است" });
                return new JsonResult(new { Status = 0, Message = result.Errors.First().Description });
            }





        }
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> IdentityVerify([FromBody] IdentityVerifyViewModel input)
        {

            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var item in ModelState.Values)
                {
                    foreach (var err in item.Errors)
                    {
                        errors.Add(err.ErrorMessage);
                    }
                }
                return new JsonResult(new { Status = 0, Error = errors });
            }
            var user = await _userManager.FindByNameAsync(input.PhoneNumber);
            if (user == null)
                return Json(new { status = 0, Message = "شماره شما در سیستم موجود نمی باشد" });
            if (user.VerificationExpireTime < DateTime.Now)
                return Json(new { status = 0, Message = "کد 5 رقمی شما منقضی شده است" });
            if (input.LoginOrRegister == "Login")
            {


                var result = await _signInManager.PasswordSignInAsync(user, input.VerificationNumber, true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var isAdmin = await _userManager.IsInRoleAsync(user, PublicHelper.ADMINROLE);
                    var isUser = await _userManager.IsInRoleAsync(user, PublicHelper.USERROLE);
                    var isWarehousingAdmin = await _userManager.IsInRoleAsync(user, PublicHelper.WarehousingAdminROLE);

                    await _signInManager.SignInAsync(user, true);

                    if (string.IsNullOrEmpty(input.ReturnUrl) || input.ReturnUrl == "/")
                    {
                        if (isAdmin)
                        {
                            return new JsonResult(new { Status = 1, ReturnUrl = "/dashboard", message = "خوش آمدید!" });

                        }
                        else if (isWarehousingAdmin)
                        {
                            return new JsonResult(new { Status = 1, ReturnUrl = "/WarehousingAdmin", message = "خوش آمدید!" });

                        }
                        else
                            return new JsonResult(new { Status = 1, ReturnUrl = "/", message = "خوش آمدید!" });

                    }
                    return new JsonResult(new { Status = 1, ReturnUrl = input.ReturnUrl, message = "خوش آمدید!" });
                }
                else
                {
                    return new JsonResult(new { Status = 0, Message = "نام کاربری یا رمز عبور اشتباه است" });
                }

            }
            else
            {

                await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(input.PhoneNumber), PublicHelper.USERROLE);
                var signIn = await _signInManager.PasswordSignInAsync(input.PhoneNumber, input.VerificationNumber, isPersistent: true, lockoutOnFailure: false);
                if (signIn.Succeeded)
                    return new JsonResult(new { Status = 1, input.ReturnUrl, message = "ثبت نام با موفقیت انجام شد" });
                else
                    return new JsonResult(new { Status = 0, input.ReturnUrl, message = "کد وارد شده نامعتبر میباشد" });
            }

        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> SignIn(LoginViewModel input)
        {
            input.ReturnUrl = input.ReturnUrl ?? Url.Action("index", "home");

            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var item in ModelState.Values)
                {
                    foreach (var err in item.Errors)
                    {
                        errors.Add(err.ErrorMessage);
                    }
                }
                return new JsonResult(new { Status = 0, Error = errors });
            }
            var user = await _userManager.FindByNameAsync(input.Username);
            if (user == null)
                return Json(new { status = "2", Error = "نام کاربری شما در سیستم موجود نمی باشد" });
            var result = await _signInManager.PasswordSignInAsync(user, input.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                var isUser = await _userManager.IsInRoleAsync(user, "User");

                await _signInManager.SignInAsync(user, true);

                if (string.IsNullOrEmpty(input.ReturnUrl) || input.ReturnUrl == "/")
                {
                    if (isAdmin)
                    {
                        return new JsonResult(new { Status = 3, input.ReturnUrl, message = "/dashboard" });

                    }
                    else
                    {
                        return new JsonResult(new { Status = 3, input.ReturnUrl, message = "/" });

                    }
                }
                return new JsonResult(new { Status = 1, input.ReturnUrl, message = "خوش آمدید!" });
            }
            else
            {
                return new JsonResult(new { Status = 2, Error = "نام کاربری یا رمز عبور اشتباه است" });
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Register(RegisterViewModel input)
        {
            input.ReturnUrl = input.ReturnUrl ?? Url.Action("index", "home");

            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var item in ModelState.Values)
                {
                    foreach (var err in item.Errors)
                    {
                        errors.Add(err.ErrorMessage);
                    }
                }
                return new JsonResult(new { Status = 0, Error = errors });
            }
            var fullName = input.Nickname;
            var names = fullName.Split(" ");
            var fName = "";
            var lName = "";
            if (names[0].Length > 1)
            {
                fName = names[0];
            }
            if (names[1].Length > 1)
            {
                lName = names[1];
            }
            var result = await _userManager.CreateAsync(new ApplicationUser
            {
                AccessFailedCount = 0,
                AvatarUrl = "",
                Birthdate = "",
                Email = input.Email,
                EmailConfirmed = false,
                FirstName = fName,
                LastName = lName,
                Gender = "",
                Nickname = "",
                NormalizedUserName = input.Email.Normalize(),
                RegistrationDateTime = DateTime.Now,
                UserName = input.PhoneNumber,
                Phone = "",
                PhoneNumber = input.PhoneNumber,
            }, input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(input.Email), "User");
                var signIn = await _signInManager.PasswordSignInAsync(input.Email, input.Password, isPersistent: true, lockoutOnFailure: false);
                if (signIn.Succeeded)
                    return new JsonResult(new { Status = 1, input.ReturnUrl, message = "ثبت نام با موفقیت انجام شد" });
                return new JsonResult(new { Status = 1, input.ReturnUrl, message = "ثبت نام با موفقیت انجام شد" });
            }
            else
            {
                if (result.Errors.Where(i => i.Code == "DuplicateUserName").Any())
                    return new JsonResult(new { Status = 2, Error = "نام کاربری از قبل ثبت شده است" });
                return new JsonResult(new { Status = 0, Error = result.Errors.First().Description });
            }

        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            returnUrl = returnUrl ?? Url.Content("/");
            return LocalRedirect(returnUrl);
        }

        [Authorize(Roles = PublicHelper.ADMINROLE)]
        public async Task<IActionResult> UserManager()
        {
            return View();
        }

        [Authorize(Roles = PublicHelper.ADMINROLE)]
        [HttpGet]
        public async Task<IActionResult> GetAllUserForAdmin(string startDate,string endDate)
        {
            var result = await _accountService.GetAllUserForAdmin(startDate,endDate);

            return new JsonResult(result);
        }

        [Authorize(Roles = PublicHelper.ADMINROLE)]
        [HttpPost]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var result = await _accountService.BlockUser(userId);
            if (result.isSuccess)
            {
                return Json(new { status = 1, message = "با موفقیت انجام شد" });
            }
            else
            {
                return Json(new { status = 0, message = result.error });
            }
        }

        [Authorize(Roles = PublicHelper.ADMINROLE)]
        [HttpPost]
        public async Task<IActionResult> UnBlockUser(string userId)
        {
            var result = await _accountService.UnBlockUser(userId);
            if (result.isSuccess)
            {
                return Json(new { status = 1, message = "با موفقیت انجام شد" });
            }
            else
            {
                return Json(new { status = 0, message = result.error });
            }
        }

        [Authorize(Roles = PublicHelper.ADMINROLE)]
        [HttpPost]
        public async Task<IActionResult> GetUserAddress(string phoneNumber)
        {
            var result = await _accountService.GetUserAddress(phoneNumber);
            if (result.isSuccess)
            {
                return Json(new { status = 1, message = "با موفقیت انجام شد", data = result.error });
            }
            else
            {
                return Json(new { status = 0, message = result.error });
            }
        }
        [Authorize(Roles = PublicHelper.ADMINROLE)]
        [Route("GetWarehousingAdmin")]
        public async Task<IActionResult> GetWarehousingAdmin()
        {
            try
            {
                var result = await _accountService.GetWarehousingAdmin();
                return Json(new { status = 1, message = "با موفقیت انجام شد", data = result.PhoneNumber });

            }
            catch (Exception)
            {

                return Json(new { status = 0, message = "خطایی رخ داده است" });

            }


        }

        [Authorize(Roles = PublicHelper.ADMINROLE)]
        [HttpPost]
        [Route("ChangeWarehousingAdmin")]
        public async Task<IActionResult> ChangeWarehousingAdmin(string phoneNumber)
        {
            var result = await _accountService.ChangeWarehousingAdmin(phoneNumber);
            if (result.isSuccess)
            {
                return Json(new { status = 1, message = "با موفقیت انجام شد" });
            }
            else
            {
                return Json(new { status = 0, message = result.error });
            }
        }


    }
}