#if SERVER

using Microsoft.AspNetCore.Identity;

namespace NSL.Management.CentralService.Shared.Models
{
    public partial class UserModel : IdentityUser
    {
        public virtual ServerModel? Server { get; set; }
    }
}

#endif
