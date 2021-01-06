using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public enum Genders
    {
        Male,
        Female
    }

    public enum AccessStatus
    {
        Enabled,
        Disabled
    }
    public enum PaymentType
    {
        [Display(Name = "آنلاین")]
        Online,
        [Display(Name = "حضوری")]
        InPerson
    }
    public enum RequestStatus
    {
        [Display(Name = "در انتظار بررسی")]
        Waiting,
        [Display(Name = "در حال ارسال")]
        Confirmed,
        [Display(Name = "رد شده")]
        Rejected,
        [Display(Name = "تحویل داده شده")]
        Finished,
        [Display(Name = "ناموفق")]
        Unsuccessful,
        [Display(Name = "در انتظار تخصیص به پیک")]
        Delivery,
        [Display(Name = "در انتظار پیک")]
        InDelivery,
        [Display(Name = "مرجوعی")]
        Returned

    }

    public enum VerificationCodeStatus
    {
        Stall,
        Used,
        Expired
    }

    public enum NotificationStatus
    {
        New,
        Seen,
        Archived
    }

    public enum UserTaskStatus
    {
        Waiting,
        Doing,
        Done,
        Rejected,
        Cancelled,
        Finnished
    }

    public enum ClientStatus
    {
        فعال,
        غیرفعال
    }
    public enum ProductStatus
    {
        دردسترس,
        خارج_از_دسترس,
        درفروشگاه,
        آماده_برای_فروش
    }

    public enum ReciptType
    {
        رسید,
        حواله,
        انبار_به_انبار
    }

    public enum GalleryType
    {
        Default,
        Product,
        Post
    }
    public enum CommentStatus
    {
        در_حال_بررسی,
        تایید_شده,
        رد_شده
    }
    public enum QrUse
    {
        Used,
        NotUsed,
    }

    public enum TicketStatus
    {
        [Display(Name = "فرستنده_یوزر_پاسخ_نگرفته")]
        UserSenderNotReply = 1,
        [Display(Name = "فرستنده_ادمین_پاسخ_گرفته")]
        AdminSenderReply = 2,
        [Display(Name = "فرستنده_یوزر_پاسخ_گرفته")]
        UserSenderReply = 3,
        [Display(Name = "فرستنده_ادمین_پاسخ_نگرفته")]
        AdminSenderNotReply = 4,

    }

    public enum TicketPriorityStatus
    {
        [Display(Name = "فوری")]
        Immediate = 1,
        [Display(Name = "معمولی")]
        Ordinary = 2,
        [Display(Name = "جهت اطلاع")]
        justForInformation = 3,
    }
    public enum Gateways
    {
        Saman,
        Mellat,
        Parsian,
        Pasargad,
        IranKish,
        Melli,
        AsanPardakht,
        ZarinPal,
        PayIr,
        IdPay,
        ParbadVirtual
    }
}
