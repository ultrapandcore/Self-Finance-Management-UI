using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace SelfFinanceManagerUI.Shared.Components
{
    public class AuthenticatedComponent : ComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected bool IsAuthStateInitialized { get; set; } = false;

        [Inject]
        protected ILogger<AuthenticatedComponent> Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await AuthenticationStateTask;
                IsAuthStateInitialized = true;
                await OnAuthInitializedAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while initializing authentication state");
                throw;
            }
            finally
            {
                await base.OnInitializedAsync();
            }
        }

        protected virtual async Task OnAuthInitializedAsync()
        {
            await Task.CompletedTask;
        }
    }
}