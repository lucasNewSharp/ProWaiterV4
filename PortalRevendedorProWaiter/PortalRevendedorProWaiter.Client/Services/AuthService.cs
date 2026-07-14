using PortalRevendedorProWaiter.Shared;
using PortalRevendedorProWaiter.Shared.IdentityModel;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PortalRevendedorProWaiter.Shared.ViewModels;

namespace PortalRevendedorProWaiter.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _nav;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage,
                            NavigationManager nav)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
            _nav = nav;
        }

        public async Task<DefaultRequestResult> Register(RegisterModel registerModel)
        {
            var result = await _httpClient.PostJsonAsync<DefaultRequestResult>("api/accounts", registerModel);

            return result;
        }

        public async Task<DefaultRequestResult> ConfirmEmail(ConfirmEmailModel confirmEmailModel)
        {
            var result = await _httpClient.PostJsonAsync<DefaultRequestResult>("api/ConfirmEmail", confirmEmailModel);
            return result;
        }

        public async Task<DefaultRequestResult> ResendEmailConfirmation(LoginModel loginModel)
        {
            var result = await _httpClient.PostJsonAsync<DefaultRequestResult>("api/ResendEmailConfirmation", loginModel);
            return result;
        }

        public async Task<DefaultRequestResult> ResendForgotPasswordEmail(ForgotPasswordModel forgotPasswordModel)
        {
            var result = await _httpClient.PostJsonAsync<DefaultRequestResult>("api/ForgotPassword", forgotPasswordModel);
            return result;
        }

        public async Task<DefaultRequestResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var result = await _httpClient.PostJsonAsync<DefaultRequestResult>("api/ResetPassword", resetPasswordModel);
            return result;
        }

        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            var loginAsJson = JsonSerializer.Serialize(loginModel);
            var response = await _httpClient.PostAsync("api/Login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
            var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode)
            {
                return loginResult;
            }

            await _localStorage.SetItemAsync("authToken", loginResult.Token);
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResult.Token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            return loginResult;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
