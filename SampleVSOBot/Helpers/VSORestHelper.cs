using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SampleVSOBot.Helpers
{
    public static class VSORestHelper
    {
        public static async Task<String> GetWorkItems(string token)
        {
            using (var client = new HttpClient())
            {
                var apiVersion = "1.0";
                StringBuilder strResult = new StringBuilder();

                var account = AuthBot.Models.AuthSettings.Tenant;
                var query = "Select [System.Id] From WorkItems Where[System.WorkItemType] = 'Bug' order by [System.CreatedDate] desc";

                var url = "https://" + account + ".visualstudio.com/defaultcollection/_apis/wit/";

                // Execute a query that returns work item IDs matching the specified criteria
                using (var request = new HttpRequestMessage(HttpMethod.Post, url + "wiql"))
                {
                    request.Headers.Add("Authorization", "Bearer " + token);
                    request.Headers.Add("Accept", "application/json;api-version=" + apiVersion);

                    Dictionary<string, string> body = new Dictionary<string, string>
                    {
                         {
                        "query", query
                        }
                     };

                    request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                    using (var response = await client.SendAsync(request))
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var workItems = JObject.Parse(content)["workItems"] as JArray;

                        string[] ids = workItems.Select<JToken, string>(w => (w["id"] + "")).Take(10).ToArray<string>();
                        string idsString = String.Join(",", ids);

                        // Get details for the last 10
                        using (var detailsRequest = new HttpRequestMessage(HttpMethod.Get, url + "workitems?ids=" + idsString + "&fields=System.Id,System.Title"))
                        {
                            detailsRequest.Headers.Add("Authorization", "Bearer " + token);
                            detailsRequest.Headers.Add("Accept", "application/json;api-version=" + apiVersion);

                            using (var detailsResponse = await client.SendAsync(detailsRequest))
                            {
                                var detailsContent = await detailsResponse.Content.ReadAsStringAsync();
                                var detailsWorkItems = JObject.Parse(detailsContent)["value"] as JArray;

                                foreach (dynamic workItem in detailsWorkItems)
                                {
                                    strResult.AppendFormat("Work item: {0} ({1})",
                                        workItem.fields["System.Id"],
                                        workItem.fields["System.Title"]);
                                }
                            }
                        }
                        return strResult.ToString();
                    }
                }
            }
        }
    }

}