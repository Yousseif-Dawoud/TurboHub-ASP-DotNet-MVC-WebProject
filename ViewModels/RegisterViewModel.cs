
namespace TurboHub.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "الاسم الكامل مطلوب")]
    [MaxLength(150)]
    public required string FullName { get; set; }

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
    [MaxLength(20)]
    public required string PhoneNumber { get; set; }

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [MinLength(8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "كلمتا المرور غير متطابقتين")]
    public required string ConfirmPassword { get; set; }
}