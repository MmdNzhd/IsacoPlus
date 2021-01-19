using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using KaraYadak.Models;
using KaraYadak.ViewModels;

namespace KaraYadak.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private string upload(IFormFile image)
        {
            var fileName = DateTime.Now.Ticks.ToString();
            fileName += Path.GetFileName(image.FileName);
            var path = _env.WebRootPath + "/uploads/" + fileName;
            image.CopyTo(new FileStream(path, FileMode.Create));
            return fileName;
        }

        public SettingsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Settings.OrderByDescending(i => i.UpdatedAt).ToListAsync());
        }

        public IActionResult LandingTexts()
        {
            LandingPageTextViewModel model = new LandingPageTextViewModel
            {
                sitetitle = _context.Settings.Where(i => i.Key == "sitetitle").FirstOrDefault().Value,
                instagram = _context.Settings.Where(i => i.Key == "instagram").FirstOrDefault().Value,
                linkedin = _context.Settings.Where(i => i.Key == "linkedin").FirstOrDefault().Value,
                aboutus_h2 = _context.Settings.Where(i => i.Key == "aboutus_h2").FirstOrDefault().Value,
                aboutus = _context.Settings.Where(i => i.Key == "aboutus").FirstOrDefault().Value,
                introrear = _context.Settings.Where(i => i.Key == "introrear").FirstOrDefault().Value,
                introback = _context.Settings.Where(i => i.Key == "introback").FirstOrDefault().Value,
                visionrear = _context.Settings.Where(i => i.Key == "visionrear").FirstOrDefault().Value,
                visionback = _context.Settings.Where(i => i.Key == "visionback").FirstOrDefault().Value,
                missionrear = _context.Settings.Where(i => i.Key == "missionrear").FirstOrDefault().Value,
                missionback = _context.Settings.Where(i => i.Key == "missionback").FirstOrDefault().Value,
                trustus_h2 = _context.Settings.Where(i => i.Key == "trustus_h2").FirstOrDefault().Value,
                trustus_p = _context.Settings.Where(i => i.Key == "trustus_p").FirstOrDefault().Value,
                appsection_h2 = _context.Settings.Where(i => i.Key == "appsection_h2").FirstOrDefault().Value,
                appsection_p = _context.Settings.Where(i => i.Key == "appsection_p").FirstOrDefault().Value,
                address_center = _context.Settings.Where(i => i.Key == "address_center").FirstOrDefault().Value,
                address = _context.Settings.Where(i => i.Key == "address").FirstOrDefault().Value,
                phone = _context.Settings.Where(i => i.Key == "phone").FirstOrDefault().Value,
                mobile = _context.Settings.Where(i => i.Key == "mobile").FirstOrDefault().Value,
                email = _context.Settings.Where(i => i.Key == "email").FirstOrDefault().Value,
                timing = _context.Settings.Where(i => i.Key == "timing").FirstOrDefault().Value,
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LandingTexts(LandingPageTextViewModel input)
        {
            _context.Settings.Where(i => i.Key == "sitetitle").FirstOrDefault().Value = input.sitetitle;
            _context.Settings.Where(i => i.Key == "instagram").FirstOrDefault().Value = input.instagram;
            _context.Settings.Where(i => i.Key == "linkedin").FirstOrDefault().Value = input.linkedin;
            _context.Settings.Where(i => i.Key == "aboutus_h2").FirstOrDefault().Value = input.aboutus_h2;
            _context.Settings.Where(i => i.Key == "aboutus").FirstOrDefault().Value = input.aboutus;
            _context.Settings.Where(i => i.Key == "introrear").FirstOrDefault().Value = input.introrear;
            _context.Settings.Where(i => i.Key == "introback").FirstOrDefault().Value = input.introback;
            _context.Settings.Where(i => i.Key == "visionrear").FirstOrDefault().Value = input.visionrear;
            _context.Settings.Where(i => i.Key == "visionback").FirstOrDefault().Value = input.visionback;
            _context.Settings.Where(i => i.Key == "missionrear").FirstOrDefault().Value = input.missionrear;
            _context.Settings.Where(i => i.Key == "missionback").FirstOrDefault().Value = input.missionback;
            _context.Settings.Where(i => i.Key == "trustus_h2").FirstOrDefault().Value = input.trustus_h2;
            _context.Settings.Where(i => i.Key == "trustus_p").FirstOrDefault().Value = input.trustus_p;
            _context.Settings.Where(i => i.Key == "appsection_h2").FirstOrDefault().Value = input.appsection_h2;
            _context.Settings.Where(i => i.Key == "appsection_p").FirstOrDefault().Value = input.appsection_p;
            _context.Settings.Where(i => i.Key == "address_center").FirstOrDefault().Value = input.address_center;
            _context.Settings.Where(i => i.Key == "address").FirstOrDefault().Value = input.address;
            _context.Settings.Where(i => i.Key == "phone").FirstOrDefault().Value = input.phone;
            _context.Settings.Where(i => i.Key == "mobile").FirstOrDefault().Value = input.mobile;
            _context.Settings.Where(i => i.Key == "email").FirstOrDefault().Value = input.email;
            _context.Settings.Where(i => i.Key == "timing").FirstOrDefault().Value = input.timing;
            await _context.SaveChangesAsync();

            return RedirectToAction("texts");
        }

        public IActionResult LandingImages()
        {
            //_context.Images.RemoveRange(_context.Images.ToList());
            //_context.SaveChanges();
            //return Json(_context.Images.ToList());
            var model = new LandingPageImagesViewModel
            {
                topsliders = _context.Images.Where(i => i.Key == "landing-top-sliders").Select(i => i.Url).ToList(),
                laptopsliders = _context.Images.Where(i => i.Key == "landing-laptop-sliders").Select(i => i.Url).ToList(),
                applicationsliders = _context.Images.Where(i => i.Key == "landing-application-sliders").Select(i => i.Url).ToList(),
                trustusimage = _context.Images.Where(i => i.Key == "landing-trustus-image").Select(i => i.Url).FirstOrDefault()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LandingImageTopSlider(IList<IFormFile> files)
        {
            var s = new List<string>();
            foreach (IFormFile source in files)
            {
                string filePath = "";
                if (source != null && source.Length > 0)
                {
                    var newfilename = "landing-top-sliders-" + DateTime.Now.Ticks.ToString() + Path.GetExtension(source.FileName);
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\uploads", newfilename);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await source.CopyToAsync(stream);
                    }
                    filePath = "/uploads/" + newfilename;
                }
                s.Add(filePath);
                await _context.Images.AddAsync(new Image
                {
                    Key = "landing-top-sliders",
                    Url = filePath,
                });
            }
            await _context.SaveChangesAsync();
            return Json(s);
        }

        [HttpPost]
        public async Task<IActionResult> LandingImageLaptopSlider(IList<IFormFile> files)
        {
            var s = new List<string>();
            foreach (IFormFile source in files)
            {
                string filePath = "";
                if (source != null && source.Length > 0)
                {
                    var newfilename = "landing-laptop-sliders-" + DateTime.Now.Ticks.ToString() + Path.GetExtension(source.FileName);
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\uploads", newfilename);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await source.CopyToAsync(stream);
                    }
                    filePath = "/uploads/" + newfilename;
                }
                await _context.Images.AddAsync(new Image
                {
                    Key = "landing-laptop-sliders",
                    Url = filePath,
                });
            }
            await _context.SaveChangesAsync();
            return Json(s);
        }

        [HttpPost]
        public async Task<IActionResult> LandingImageApplicationSlider(IList<IFormFile> files)
        {
            var s = new List<string>();
            foreach (IFormFile source in files)
            {
                string filePath = "";
                if (source != null && source.Length > 0)
                {
                    var newfilename = "landing-application-sliders-" + DateTime.Now.Ticks.ToString() + Path.GetExtension(source.FileName);
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\uploads", newfilename);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await source.CopyToAsync(stream);
                    }
                    filePath = "/uploads/" + newfilename;
                }
                await _context.Images.AddAsync(new Image
                {
                    Key = "landing-application-sliders",
                    Url = filePath,
                });
            }
            await _context.SaveChangesAsync();
            return Json(s);
        }

        [HttpPost]
        public async Task<IActionResult> LandingImageTrustus(IFormFile file)
        {
            var s = new List<string>();
            string filePath = "";
            if (file != null && file.Length > 0)
            {
                var newfilename = "landing-trustus-image-" + DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName);
                filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\uploads", newfilename);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                filePath = "/uploads/" + newfilename;
            }

            if (_context.Images.Where(i => i.Key == "landing-trustus-image").Count() > 0)
            {
                _context.Images.Where(i => i.Key == "landing-trustus-image").FirstOrDefault().Url = filePath;
            }
            else
            {
                await _context.Images.AddAsync(new Image
                {
                    Key = "landing-trustus-image",
                    Url = filePath,
                });
            }
            await _context.SaveChangesAsync();
            return Json(s);
        }

        public async Task<IActionResult> DeleteImage(string imageurl)
        {
            _context.Images.Remove(_context.Images.FirstOrDefault(i => i.Url == imageurl));
            await _context.SaveChangesAsync();
            var fullPath = _env.WebRootPath + Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\uploads", imageurl);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            return Content("ok");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Key,Value")] Setting setting)
        {
            setting.CreatedAt = DateTime.Now;
            setting.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                setting.Key.ToLower().Replace(' ', '_').Trim();
                _context.Add(setting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(setting);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings.FindAsync(id);
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Key,Value")] Setting setting)
        {
            if (id != setting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    setting.UpdatedAt = DateTime.Now;
                    _context.Update(setting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SettingExists(setting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(setting);
        }

        public IActionResult EditAll()
        {

            SettingVM setting = new SettingVM()
            {
                Footer = _context.Settings.Where(x => x.Key.Equals("Footer")).Select(x => x.Value).FirstOrDefault(),
                AboutUs = _context.Settings.Where(x => x.Key.Equals("AboutUs")).Select(x => x.Value).FirstOrDefault(),
                Address = _context.Settings.Where(x => x.Key.Equals("Address")).Select(x => x.Value).FirstOrDefault(),
                ContactUs = _context.Settings.Where(x => x.Key.Equals("ContactUs")).Select(x => x.Value).FirstOrDefault(),
                Email = _context.Settings.Where(x => x.Key.Equals("Email")).Select(x => x.Value).FirstOrDefault(),
                Facebook = _context.Settings.Where(x => x.Key.Equals("Facebook")).Select(x => x.Value).FirstOrDefault(),
                Instagram = _context.Settings.Where(x => x.Key.Equals("Instagram")).Select(x => x.Value).FirstOrDefault(),
                linkedin = _context.Settings.Where(x => x.Key.Equals("linkedin")).Select(x => x.Value).FirstOrDefault(),
                Phone = _context.Settings.Where(x => x.Key.Equals("Phone")).Select(x => x.Value).FirstOrDefault(),
                PhoneNumber = _context.Settings.Where(x => x.Key.Equals("PhoneNumber")).Select(x => x.Value).FirstOrDefault(),
                SiteLogo = _context.Settings.Where(x => x.Key.Equals("SiteLogo")).Select(x => x.Value).FirstOrDefault(),
                SiteTitle = _context.Settings.Where(x => x.Key.Equals("SiteTitle")).Select(x => x.Value).FirstOrDefault(),
                Successful = _context.Settings.Where(x => x.Key.Equals("Successful")).Select(x => x.Value).FirstOrDefault(),
                Telegram = _context.Settings.Where(x => x.Key.Equals("Telegram")).Select(x => x.Value).FirstOrDefault(),
                Twitter = _context.Settings.Where(x => x.Key.Equals("Twitter")).Select(x => x.Value).FirstOrDefault(),
                whatsapp = _context.Settings.Where(x => x.Key.Equals("whatsapp")).Select(x => x.Value).FirstOrDefault(),
                Slider1Text = _context.Settings.Where(x => x.Key.Equals("Slider1")).Select(x => x.Value).FirstOrDefault(),
                Slider2Text = _context.Settings.Where(x => x.Key.Equals("Slider2")).Select(x => x.Value).FirstOrDefault(),
                Slider3Text = _context.Settings.Where(x => x.Key.Equals("Slider3")).Select(x => x.Value).FirstOrDefault(),
                Slider4Text = _context.Settings.Where(x => x.Key.Equals("Slider4")).Select(x => x.Value).FirstOrDefault(),
                Slider5Text = _context.Settings.Where(x => x.Key.Equals("Slider5")).Select(x => x.Value).FirstOrDefault(),



                Slider1Pic= _context.Settings.Where(x => x.Key.Equals("Slider1Pic")).Select(x => x.Value).FirstOrDefault(),
                Slider2Pic= _context.Settings.Where(x => x.Key.Equals("Slider2Pic")).Select(x => x.Value).FirstOrDefault(),
                Slider3Pic= _context.Settings.Where(x => x.Key.Equals("Slider3Pic")).Select(x => x.Value).FirstOrDefault(),
                Slider4Pic= _context.Settings.Where(x => x.Key.Equals("Slider4Pic")).Select(x => x.Value).FirstOrDefault(),
                Slider5Pic= _context.Settings.Where(x => x.Key.Equals("Slider5Pic")).Select(x => x.Value).FirstOrDefault(),


                Category1 = _context.Settings.Where(x => x.Key.Equals("Category1")).Select(x => x.Value).FirstOrDefault(),
                Category1Title = _context.Settings.Where(x => x.Key.Equals("Category1")).Select(x => x.Title).FirstOrDefault(),
                Category2 = _context.Settings.Where(x => x.Key.Equals("Category2")).Select(x => x.Value).FirstOrDefault(),
                Category2Title = _context.Settings.Where(x => x.Key.Equals("Category2")).Select(x => x.Title).FirstOrDefault(),
                Category3 = _context.Settings.Where(x => x.Key.Equals("Category3")).Select(x => x.Value).FirstOrDefault(),
                Category3Title = _context.Settings.Where(x => x.Key.Equals("Category3")).Select(x => x.Title).FirstOrDefault(),
                Category4 = _context.Settings.Where(x => x.Key.Equals("Category4")).Select(x => x.Value).FirstOrDefault(),
                Category4Title = _context.Settings.Where(x => x.Key.Equals("Category4")).Select(x => x.Title).FirstOrDefault(),
                Category5 = _context.Settings.Where(x => x.Key.Equals("Category5")).Select(x => x.Value).FirstOrDefault(),
                Category5Title = _context.Settings.Where(x => x.Key.Equals("Category5")).Select(x => x.Title).FirstOrDefault(),
                Category6 = _context.Settings.Where(x => x.Key.Equals("Category6")).Select(x => x.Value).FirstOrDefault(),
                Category6Title = _context.Settings.Where(x => x.Key.Equals("Category6")).Select(x => x.Title).FirstOrDefault(),
                Category7 = _context.Settings.Where(x => x.Key.Equals("Category7")).Select(x => x.Value).FirstOrDefault(),
                Category7Title = _context.Settings.Where(x => x.Key.Equals("Category7")).Select(x => x.Title).FirstOrDefault()
            };
            return View(setting);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSiteSetting(SiteSettingVM siteSetting, IFormFile Category7, IFormFile SiteLogo, IFormFile Category1
            , IFormFile Category2, IFormFile Category3, IFormFile Category4, IFormFile Category5, IFormFile Category6,
            IFormFile sPic1, IFormFile sPic2, IFormFile sPic3, IFormFile sPic4, IFormFile sPic5)
        {

            if (ModelState.IsValid)
            {
                try
                {


                    //AboutUs
                    Setting aboutUsSetting = _context.Settings.Where(x => x.Key.Equals("AboutUs")).SingleOrDefault();
                    aboutUsSetting.Value = siteSetting.AboutUs;
                    aboutUsSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(aboutUsSetting).State = EntityState.Modified;
                    //ContactUs
                    Setting contactUsSetting = _context.Settings.Where(x => x.Key.Equals("ContactUs")).SingleOrDefault();
                    contactUsSetting.Value = siteSetting.ContactUs;
                    contactUsSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(contactUsSetting).State = EntityState.Modified;
                    //Footer
                    Setting footerSetting = _context.Settings.Where(x => x.Key.Equals("Footer")).SingleOrDefault();
                    footerSetting.Value = siteSetting.Footer;
                    footerSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(footerSetting).State = EntityState.Modified;
                    //Slider1Text
                    Setting slider1TextSetting = _context.Settings.Where(x => x.Key.Equals("Slider1")).SingleOrDefault();
                    slider1TextSetting.Value = siteSetting.Slider1Text;
                    slider1TextSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(slider1TextSetting).State = EntityState.Modified;
                    //Slider2Text
                    Setting slider2TextSetting = _context.Settings.Where(x => x.Key.Equals("Slider2")).SingleOrDefault();
                    slider2TextSetting.Value = siteSetting.Slider2Text;
                    slider2TextSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(slider2TextSetting).State = EntityState.Modified;
                    //Slider3Text
                    Setting slider3TextSetting = _context.Settings.Where(x => x.Key.Equals("Slider3")).SingleOrDefault();
                    slider3TextSetting.Value = siteSetting.Slider3Text;
                    slider3TextSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(slider3TextSetting).State = EntityState.Modified;

                    //Slider4Text
                    Setting slider4TextSetting = _context.Settings.Where(x => x.Key.Equals("Slider4")).SingleOrDefault();
                    slider4TextSetting.Value = siteSetting.Slider4Text;
                    slider4TextSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(slider4TextSetting).State = EntityState.Modified;


                    //Slider4Text
                    Setting slider5TextSetting = _context.Settings.Where(x => x.Key.Equals("Slider5")).SingleOrDefault();
                    slider5TextSetting.Value = siteSetting.Slider5Text;
                    slider5TextSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(slider5TextSetting).State = EntityState.Modified;
                    //SiteLogo
                    Setting siteLogoSetting = _context.Settings.Where(x => x.Key.Equals("SiteLogo")).SingleOrDefault();
                    if (SiteLogo != null)
                    {
                        siteLogoSetting.Value = upload(SiteLogo);
                    }
                    siteLogoSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(siteLogoSetting).State = EntityState.Modified;


                    //slider1Pic
                    Setting slider1PicSetting = _context.Settings.Where(x => x.Key.Equals("slider1Pic")).SingleOrDefault();
                    if (sPic1 != null)
                    {
                        slider1PicSetting.Value = upload(sPic1);
                    }
                    slider1PicSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(slider1PicSetting).State = EntityState.Modified;


                    //slider2Pic
                    Setting slider2PicSetting = _context.Settings.Where(x => x.Key.Equals("slider2Pic")).SingleOrDefault();
                    if (sPic2 != null)
                    {
                        slider2PicSetting.Value = upload(sPic2);
                    }
                    slider2PicSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(slider2PicSetting).State = EntityState.Modified;

                    //slider2Pic
                    Setting slider3PicSetting = _context.Settings.Where(x => x.Key.Equals("slider3Pic")).SingleOrDefault();
                    if (sPic3 != null)
                    {
                        slider3PicSetting.Value = upload(sPic3);
                    }
                    slider3PicSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(slider3PicSetting).State = EntityState.Modified;

                    //slider2Pic
                    Setting slider4PicSetting = _context.Settings.Where(x => x.Key.Equals("slider4Pic")).SingleOrDefault();
                    if (sPic4 != null)
                    {
                        slider4PicSetting.Value = upload(sPic4);
                    }
                    slider4PicSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(slider4PicSetting).State = EntityState.Modified;

                    //slider2Pic
                    Setting slider5PicSetting = _context.Settings.Where(x => x.Key.Equals("slider5Pic")).SingleOrDefault();
                    if (sPic5 != null)
                    {
                        slider5PicSetting.Value = upload(sPic5);
                    }
                    slider5PicSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(slider5PicSetting).State = EntityState.Modified;


                    //Category_1
                    Setting category_1Setting = _context.Settings.Where(x => x.Key.Equals("Category1")).SingleOrDefault();
                    if (Category1 != null)
                    {
                        category_1Setting.Value = upload(Category1);
                    }
                    category_1Setting.UpdatedAt = DateTime.Now;
                    _context.Entry(category_1Setting).State = EntityState.Modified;

                    //Category_2
                    Setting category_2Setting = _context.Settings.Where(x => x.Key.Equals("Category2")).SingleOrDefault();
                    if (Category2 != null)
                    {
                        category_2Setting.Value = upload(Category2);
                    }
                    category_2Setting.UpdatedAt = DateTime.Now;
                    _context.Entry(category_2Setting).State = EntityState.Modified;

                    //Category3
                    Setting category3Setting = _context.Settings.Where(x => x.Key.Equals("Category3")).SingleOrDefault();
                    if (Category3 != null)
                    {
                        category3Setting.Value = upload(Category3);
                    }
                    category3Setting.UpdatedAt = DateTime.Now;
                    _context.Entry(category3Setting).State = EntityState.Modified;

                    //Category4
                    Setting category4Setting = _context.Settings.Where(x => x.Key.Equals("Category4")).SingleOrDefault();
                    if (Category4 != null)
                    {
                        category4Setting.Value = upload(Category4);
                    }
                    category4Setting.UpdatedAt = DateTime.Now;
                    _context.Entry(category4Setting).State = EntityState.Modified;


                    //Category5
                    Setting category5Setting = _context.Settings.Where(x => x.Key.Equals("Category5")).SingleOrDefault();
                    if (Category5 != null)
                    {
                        category5Setting.Value = upload(Category5);
                    }
                    category5Setting.UpdatedAt = DateTime.Now;
                    _context.Entry(category5Setting).State = EntityState.Modified;

                    //Category6
                    Setting category6Setting = _context.Settings.Where(x => x.Key.Equals("Category6")).SingleOrDefault();
                    if (Category6 != null)
                    {
                        category6Setting.Value = upload(Category6);
                    }
                    category6Setting.UpdatedAt = DateTime.Now;
                    _context.Entry(category6Setting).State = EntityState.Modified;

                    //Category7
                    Setting category7Setting = _context.Settings.Where(x => x.Key.Equals("Category7")).SingleOrDefault();
                    if (Category7 != null)
                    {
                        category7Setting.Value = upload(Category7);
                    }
                    category7Setting.UpdatedAt = DateTime.Now;
                    _context.Entry(category7Setting).State = EntityState.Modified;


                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return Json(new { e });
                }
                //return RedirectToAction(nameof(Index));

                return Json(new { status = "1", message = "اطلاعات با موفقیت ثبت شد." });
            }
            return Json(new { status = "1", message = "اطلاعات با موفقیت ثبت شد." });
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSiteGeneralSetting(GeneralSettingVM generalSetting)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    //Address
                    Setting addressSetting = _context.Settings.Where(x => x.Key.Equals("Address")).SingleOrDefault();
                    addressSetting.Value = generalSetting.Address;
                    addressSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(addressSetting).State = EntityState.Modified;
                    //Email
                    Setting emailSetting = _context.Settings.Where(x => x.Key.Equals("Email")).SingleOrDefault();
                    emailSetting.Value = generalSetting.Email;
                    emailSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(emailSetting).State = EntityState.Modified;
                    //Facebook
                    Setting facebookSetting = _context.Settings.Where(x => x.Key.Equals("Facebook")).SingleOrDefault();
                    facebookSetting.Value = generalSetting.Facebook;
                    facebookSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(facebookSetting).State = EntityState.Modified;

                    //Instagram
                    Setting instagramSetting = _context.Settings.Where(x => x.Key.Equals("Instagram")).SingleOrDefault();
                    instagramSetting.Value = generalSetting.Instagram.ToLower();
                    instagramSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(instagramSetting).State = EntityState.Modified;


                    //linkedin
                    Setting linkedinSetting = _context.Settings.Where(x => x.Key.Equals("linkedin")).SingleOrDefault();
                    linkedinSetting.Value = generalSetting.linkedin;
                    linkedinSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(linkedinSetting).State = EntityState.Modified;

                    //Phone
                    Setting phoneSetting = _context.Settings.Where(x => x.Key.Equals("Phone")).SingleOrDefault();
                    phoneSetting.Value = generalSetting.Phone;
                    phoneSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(phoneSetting).State = EntityState.Modified;

                    //PhoneNumber
                    Setting phoneNumberSetting = _context.Settings.Where(x => x.Key.Equals("PhoneNumber")).SingleOrDefault();
                    phoneNumberSetting.Value = generalSetting.PhoneNumber;
                    phoneNumberSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(phoneNumberSetting).State = EntityState.Modified;


                    //SiteTitle
                    Setting siteTitleSetting = _context.Settings.Where(x => x.Key.Equals("SiteTitle")).SingleOrDefault();
                    siteTitleSetting.Value = generalSetting.SiteTitle;
                    siteTitleSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(siteTitleSetting).State = EntityState.Modified;

                    //Successful
                    Setting successfulSetting = _context.Settings.Where(x => x.Key.Equals("Successful")).SingleOrDefault();
                    successfulSetting.Value = generalSetting.Successful;
                    successfulSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(successfulSetting).State = EntityState.Modified;


                    //Telegram
                    Setting telegramSetting = _context.Settings.Where(x => x.Key.Equals("Telegram")).SingleOrDefault();
                    telegramSetting.Value = generalSetting.Telegram;
                    telegramSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(telegramSetting).State = EntityState.Modified;

                    //Twitter
                    Setting twitterSetting = _context.Settings.Where(x => x.Key.Equals("Twitter")).SingleOrDefault();
                    twitterSetting.Value = generalSetting.Twitter;
                    twitterSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(twitterSetting).State = EntityState.Modified;

                    //whatsapp
                    Setting whatsappSetting = _context.Settings.Where(x => x.Key.Equals("whatsapp")).SingleOrDefault();
                    whatsappSetting.Value = generalSetting.whatsapp;
                    whatsappSetting.UpdatedAt = DateTime.Now;
                    _context.Entry(whatsappSetting).State = EntityState.Modified;


                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return Json(new { e.Message, e.InnerException });
                }
                //return RedirectToAction(nameof(Index));
                return Json(new { status = "1", message = "اطلاعات با موفقیت ثبت شد." });
            }
            return Json(new { status = "1", message = "اطلاعات با موفقیت ثبت شد." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGeneralSetting(int id, [Bind("Id,Title,Key,Value")] Setting setting)
        {
            if (id != setting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    setting.UpdatedAt = DateTime.Now;
                    _context.Update(setting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SettingExists(setting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(setting);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (setting == null)
            {
                return NotFound();
            }
            _context.Settings.Remove(setting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SettingExists(int id)
        {
            return _context.Settings.Any(e => e.Id == id);
        }
    }
}
