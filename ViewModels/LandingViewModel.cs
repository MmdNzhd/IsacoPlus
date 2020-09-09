using System.Collections.Generic;
using KaraYadak.ViewModels;

namespace KaraYadak.Models
{
    public class LandingViewModel
    {
        public string SiteTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string Telegram { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public string Address { get; set; }
        public string LiknedIn { get; set; }
        public string Successful { get; set; }
        public string SendPrice { get; set; }
        public string Map { get; set; }
        public string Slider_1 { get; set; }
        public string Slider_2 { get; set; }
        public string Slider_3 { get; set; }
        public string Slider_4 { get; set; }
        public List<ProductCategoryType> CategoryTypes { get; set; }
        public List<string> Favorites { get; set; }
        public string Services_1 { get; set; }
        public string Services_2 { get; set; }
        public string Services_3 { get; set; }
        public string Services_4 { get; set; }
        public string Services_1_Img { get; set; }
        public string Services_2_Img { get; set; }
        public string Services_3_Img { get; set; }
        public string Services_4_Img { get; set; }
        public string Cat_1 { get; set; }
        public string Cat_2 { get; set; }
        public string Cat_3 { get; set; }
        public string Cat_4 { get; set; }
        public string Cat_5 { get; set; }
        public string Cat_6 { get; set; }
        public string Product_Slider { get; set; }
        public string Product_Slider_2 { get; set; }
        public string AboutUs_h2 { get; set; }
        public string AboutUs_p { get; set; }
        public string IntroRear{ get; set; }
        public string IntroBack { get; set; }
        public string VisionRear { get; set; }
        public string VisionBack { get; set; }
        public string MissionRear { get; set; }
        public string MissionBack { get; set; }
        public List<string> LaptopSlides { get; set; }
        public string TrustUs_h2 { get; set; }
        public string TrustUs_p { get; set; }
        public string TrustUs_img { get; set; }
        public IEnumerable<ProductWithCategoryVM> Products { get; set; }
        public IEnumerable<ProductWithCategoryVM> Products2 { get; set; }
        public IEnumerable<ProductWithCategoryVM> Specials { get; set; }
        public string AppSection_h2 { get; set; }
        public string AppSection_p { get; set; }
        public List<string> AppSection_img { get; set; }
        public List<AppSectionDetails> AppSectionDetails { get; set; }
        public string Address_h2 { get; set; }
        public string Address_p { get; set; }
        public string Phone_p { get; set; }
        public string Email_p { get; set; }
        public string Timing_p { get; set; }
        public string Map_lng { get; set; }
        public string Map_lat { get; set; }
    }
    public class TestimonialViewModel
    {
        public string Text { get; set; }
        public string Person { get; set; }
        public string Organization { get; set; }
    }
    public class AppSectionDetails
    {
        public string icon { get; set; }
        public string h4 { get; set; }
        public string p { get; set; }
    }
}
