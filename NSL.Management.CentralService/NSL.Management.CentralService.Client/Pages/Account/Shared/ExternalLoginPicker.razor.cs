using Microsoft.AspNetCore.Components;
using NSL.Management.CentralService.Shared.Client.DotNetIdentity;

namespace NSL.Management.CentralService.Client.Pages.Account.Shared
{
    public partial class ExternalLoginPicker
    {
        private AuthenticationScheme[] externalLogins = [];

        [SupplyParameterFromQuery] public string? ReturnUrl { get; set; }

        //protected override async Task OnInitializedAsync()
        //{
        //    externalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
        //}
    }
}