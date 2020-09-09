using System.Collections.Generic;

namespace KaraYadak.ViewModels
{
    public class LandingPageImagesViewModel
    {
        public LandingPageImagesViewModel()
        {
            topsliders = new List<string>();
            laptopsliders = new List<string>();
            applicationsliders = new List<string>();
            trustusimage = "";
        }
        public List<string> topsliders { get; set; }
        public List<string> laptopsliders { get; set; }
        public List<string> applicationsliders { get; set; }
        public string trustusimage { get; set; }
    }
}
