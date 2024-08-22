using System.ComponentModel.DataAnnotations;

namespace NSL.Management.CentralService.Shared.Models.RequestModels
{
    public partial class IdentityIndexRequestModel
    {
        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }
    }
}