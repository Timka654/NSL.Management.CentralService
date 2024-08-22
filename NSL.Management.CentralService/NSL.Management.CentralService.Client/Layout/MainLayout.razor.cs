using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Routing;

namespace NSL.Management.CentralService.Client.Layout
{
    public partial class MainLayout
    {
        Sidebar sidebar;
        IEnumerable<NavItem> navItems;

        private async Task<SidebarDataProviderResult> SidebarDataProvider(SidebarDataProviderRequest request)
        {
            if (navItems is null)
                navItems = GetNavItems();

            return await Task.FromResult(request.ApplyTo(navItems));
        }

        private IEnumerable<NavItem> GetNavItems()
        {
            navItems = new List<NavItem>
        {
            new NavItem { Id = "1", Href = "/", IconName = IconName.HouseDoorFill, Text = "Home", Match=NavLinkMatch.All},
            new NavItem { Id = "2", Href = "/servers", IconName = IconName.PlusSquareFill, Text = "Servers"},
        };

            return navItems;
        }

    }
}
