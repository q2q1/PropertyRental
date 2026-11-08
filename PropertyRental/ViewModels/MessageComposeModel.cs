using System.ComponentModel.DataAnnotations;

namespace PropertyRental.ViewModels;

/// <summary>Form for composing a message — avoids [Required] on SenderId failing when the field is not posted.</summary>
public class MessageComposeModel
{
    [Required(ErrorMessage = "Select a recipient.")]
    public string ReceiverId { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required, StringLength(8000)]
    public string Body { get; set; } = string.Empty;
}
