using AuthBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AuthBot.Helpers
{
    class VisualStudioOnlineHelper
    {
        public static string GetAuthUrlAsync(ResumptionCookie resumptionCookie)
        {
            //To Do Change Tenant to Scope
            var encodedCookie = Microsoft.Bot.Builder.Dialogs.UrlToken.Encode(resumptionCookie);
            var scopes = String.Join(",", AuthSettings.Scopes); 
            return String.Format("{0}/oauth2/authorize?client_id={1}&response_type=Assertion&state={2}&scope={3}&redirect_uri={4}", AuthSettings.EndpointUrl, AuthSettings.ClientId, encodedCookie, scopes, AuthSettings.RedirectUrl);
        }
        public static async Task<AuthResult> GetTokenByAuthCodeAsync(string authorizationCode)
        {
            AuthResult authResult = null;
            VSOAuthResult _VSOauthResult=null;
            
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://app.vssps.visualstudio.com");
                using (var request = new HttpRequestMessage(HttpMethod.Post, "/oauth2/token"))
                {                    
                    var postData = GenerateRequestPostData(authorizationCode);
                    request.Content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                    HttpResponseMessage response = await client.SendAsync(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var strWebAuthResponse = await response.Content.ReadAsStringAsync();

                        _VSOauthResult = VSOAuthResult.FromVSOAuthenticationResult(strWebAuthResponse);
                        authResult = AuthResult.FromVSOAuthenticationResult(_VSOauthResult);
                    }
                }

            }
            
            return authResult;
        }

        public static async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            AuthResult authResult = null;
            VSOAuthResult _VSOauthResult = null;

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://app.vssps.visualstudio.com");
                using (var request = new HttpRequestMessage(HttpMethod.Post, "/oauth2/token"))
                {
                    var postData = GenerateRefreshPostData(refreshToken);
                    request.Content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                    HttpResponseMessage response = await client.SendAsync(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var strWebAuthResponse = await response.Content.ReadAsStringAsync();

                        _VSOauthResult = VSOAuthResult.FromVSOAuthenticationResult(strWebAuthResponse);
                        authResult = AuthResult.FromVSOAuthenticationResult(_VSOauthResult);
                    }
                }

            }

            return authResult;
        }
        private static string GenerateRequestPostData(string code)
        {
            return string.Format("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&client_assertion={0}&grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&assertion={1}&redirect_uri={2}",
                HttpUtility.UrlEncode(AuthSettings.ClientSecret),
                HttpUtility.UrlEncode(code),
                AuthSettings.RedirectUrl
                );
        }
        private static string GenerateRefreshPostData(string refreshToken)
        {
            return string.Format("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&client_assertion={0}&grant_type=refresh_token&assertion={1}&redirect_uri={2}",
                HttpUtility.UrlEncode(AuthSettings.ClientSecret),
                HttpUtility.UrlEncode(refreshToken),
                AuthSettings.RedirectUrl
                );
        }


        public static bool IsTokenExpired(long _expiresOnUtcTicks) {

            return DateTime.UtcNow.Ticks > _expiresOnUtcTicks;

        }


    }
}
