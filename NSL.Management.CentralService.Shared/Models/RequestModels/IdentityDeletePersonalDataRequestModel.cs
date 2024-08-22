using System.ComponentModel.DataAnnotations;

namespace NSL.Management.CentralService.Shared.Models.RequestModels
{
    public partial class IdentityDeletePersonalDataRequestModel
    {
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}