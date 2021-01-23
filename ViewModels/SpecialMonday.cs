using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak
{
    public class SpecialMonday
    {
        public string Title { get; set; }
        public IFormFile TopSliderPic1 { get; set; }
        public IFormFile TopSliderPic2 { get; set; }
        public IFormFile TopSliderPic3 { get; set; }
        public IFormFile TopSliderPic4 { get; set; }
        public IFormFile TopSliderPic5 { get; set; }
        public IFormFile TopSliderPic6 { get; set; }

        public IFormFile LeftPic { get; set; }
        public IFormFile RightPic { get; set; }


        public IFormFile MondayVerticalBaner1 { get; set; }
        public IFormFile MondayVerticalBaner2 { get; set; }
        public IFormFile MondayVerticalBaner3 { get; set; }

    }

    public class SpecialMondayBanner
    {
        public string Title { get; set; }
        public string TopSliderPic1 { get; set; }
        public string TopSliderPic2 { get; set; }
        public string TopSliderPic3 { get; set; }
        public string TopSliderPic4 { get; set; }
        public string TopSliderPic5 { get; set; }
        public string TopSliderPic6 { get; set; }

        public string LeftPic { get; set; }
        public string RightPic { get; set; }


        public string MondayVerticalBaner1 { get; set; }
        public string MondayVerticalBaner2 { get; set; }
        public string MondayVerticalBaner3 { get; set; }

    }
    public class SpecialMondayProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryIdLvl1 { get; set; }
        public int CategoryIdLvl2 { get; set; }
        public int CategoryIdLvl3 { get; set; }
        public string Categories { get; set; }
        public string Code { get; set; }
        public int Discount { get; set; }
    }

    public class SpecialMondayInfo
    {
        public SpecialMondayBanner SpecialMondayBanner { get; set; }
        public List<SpecialMondayProduct> Products { get; set; }
    }

    public class AddProductInMondayViewModel
    {
        [DisplayName("کد محصول")]
        [Required(ErrorMessage = "لطفا  {0}  را وارد کنید")]
        public string Code { get; set; }

        [DisplayName(" درصد تخفیف")]
        [Required(ErrorMessage = "لطفا  {0}  را وارد کنید")]
        public float Discount { get; set; }
    }
}
