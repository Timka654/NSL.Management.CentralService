using Microsoft.AspNetCore.Components;
using NSL.Management.CentralService.Shared.Models;
using NSL.Management.CentralService.Shared.Models.RequestModels;

namespace NSL.Management.CentralService.Client.Pages.Account.Pages.Manage
{
    public partial class ChangePassword
    {
        private string? message;
        private UserModel user = default!;
        private bool hasPassword;

        // [CascadingParameter]
        // private HttpContext HttpContext { get; set; } = default!;

        private IdentityChangePasswordRequestModel Input { get; set; } = new();

        // protected override async Task OnInitializedAsync()
        // {
        //     user = await UserAccessor.GetRequiredUserAsync(HttpContext);
        //     hasPassword = await UserManager.HasPasswordAsync(user);
        //     if (!hasPassword)
        //     {
        //         RedirectManager.RedirectTo("Account/Manage/SetPassword");
        //     }
        // }

        private async Task OnValidSubmitAsync()
        {
            //     var changePasswordResult = await UserManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            //     if (!changePasswordResult.Succeeded)
            //     {
            //         message = $"Error: {string.Join(",", changePasswordResult.Errors.Select(error => error.Description))}";
            //         return;
            //     }

            //     await SignInManager.RefreshSignInAsync(user);
            //     Logger.LogInformation("User changed their password successfully.");

            //     RedirectManager.RedirectToCurrentPageWithStatus("Your password has been changed", HttpContext);
        }
    }
}