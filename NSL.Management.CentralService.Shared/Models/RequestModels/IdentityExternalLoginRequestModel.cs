using System.ComponentModel.DataAnnotations;

namespace NSL.Management.CentralService.Shared.Models.RequestModels
{
    public partial class IdentityExternalLoginRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}