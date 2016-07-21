﻿using AuthBot.Models;
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
            return String.Format("{0}/oauth2/authorize?client_id={1}&response_type=Assertion&state={2}&scope={3}&redirect_uri={4}", AuthSettings.EndpointUrl, AuthSettings.ClientId, encodedCookie, AuthSettings.Tenant, AuthSettings.RedirectUrl);
        }
        public static async Task<VSOAuthResult> GetTokenByAuthCodeAsync(string authorizationCode)
        {
            VSOAuthResult authResult=null;
            
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
                        
                        authResult = VSOAuthResult.FromVSOAuthenticationResult(strWebAuthResponse);
                    }
                }
            }
            

            //if (!String.IsNullOrEmpty(authorizationCode))
            //{
            //    error = PerformTokenRequest(GenerateRequestPostData(authorizationCode), out token);
            //    if (!String.IsNullOrEmpty(error))
            //    {
            //        //Manage error
            //    }

            //}

            return authResult;
        }

        //private static String PerformTokenRequest(String postData, out TokenModel token)
        //{
        //    var error = String.Empty;
        //    var strResponseData = String.Empty;

        //    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://app.vssps.visualstudio.com/oauth2/token");

        //    webRequest.Method = "POST";
        //    webRequest.ContentLength = postData.Length;
        //    webRequest.ContentType = "application/x-www-form-urlencoded";

        //    using (StreamWriter swRequestWriter = new StreamWriter(webRequest.GetRequestStream()))
        //    {
        //        swRequestWriter.Write(postData);
        //    }

        //    try
        //    {
        //        HttpWebResponse hwrWebResponse = (HttpWebResponse)webRequest.GetResponse();

        //        if (hwrWebResponse.StatusCode == HttpStatusCode.OK)
        //        {
        //            using (StreamReader srResponseReader = new StreamReader(hwrWebResponse.GetResponseStream()))
        //            {
        //                strResponseData = srResponseReader.ReadToEnd();
        //            }

        //            token = JsonConvert.DeserializeObject<TokenModel>(strResponseData);
        //            return null;
        //        }
        //    }
        //    catch (WebException wex)
        //    {
        //        error = "Request Issue: " + wex.Message;
        //    }
        //    catch (Exception ex)
        //    {
        //        error = "Issue: " + ex.Message;
        //    }

        //    token = new TokenModel();
        //    return error;
        //}


        public static string GenerateRequestPostData(string code)
        {
            return string.Format("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&client_assertion={0}&grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&assertion={1}&redirect_uri={2}",
                HttpUtility.UrlEncode(AuthSettings.ClientSecret),
                HttpUtility.UrlEncode(code),
                AuthSettings.RedirectUrl
                );
        }

        public static string GenerateRefreshPostData(string refreshToken)
        {
            return string.Format("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&client_assertion={0}&grant_type=refresh_token&assertion={1}&redirect_uri={2}",
                HttpUtility.UrlEncode(AuthSettings.ClientSecret),
                HttpUtility.UrlEncode(refreshToken),
                AuthSettings.RedirectUrl
                );

        }


    }
}