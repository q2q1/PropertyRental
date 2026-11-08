using System.ComponentModel.DataAnnotations;

namespace PropertyRental.ViewModels;

public class EventReportCreateModel
{
    [Required(ErrorMessage = "Select a recipient.")]
    public string PropertyOwnerId { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(4000)]
    public string Description { get; set; } = string.Empty;
}

