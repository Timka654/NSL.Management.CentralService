using System.ComponentModel.DataAnnotations;

namespace NSL.Management.CentralService.Shared.Models.RequestModels
{
    public partial class IdentityForgotPasswordRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}