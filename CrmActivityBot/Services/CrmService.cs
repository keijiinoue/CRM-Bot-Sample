using CrmActivityBot.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CrmActivityBot.Services
{
    public class CrmService
    {
        public static string CallerId;

        public async Task<Appointments> GetCRMAppointments()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                  <entity name='appointment'>
                    <attribute name='subject' />
                    <attribute name='activityid' />
                    <order attribute='scheduledstart' descending='true' />
                    <filter type='and'>
                      <condition attribute='statecode' operator='in'>
                        <value>0</value>
                        <value>3</value>
                      </condition>
                      <condition attribute='scheduledstart' operator='olderthan-x-minutes' value='1' />
                      <condition attribute='description' operator='null' />
                    </filter>
                    <link-entity name='activityparty' from='activityid' to='activityid' alias='ab'>
                      <filter type='and'>
                        <condition attribute='partyid' operator='eq-userid' />
                        <condition attribute='participationtypemask' operator='in'>
                          <value>7</value>
                          <value>9</value>
                          <value>5</value>
                          <value>6</value>
                        </condition>
                      </filter>
                    </link-entity>
                  </entity>
                </fetch>";

            using (HttpClient httpClient = await GetClient())
            {
                try
                {
                    var encodedFetch = System.Web.HttpUtility.UrlEncode(fetch);
                    // レコードの取得
                    HttpResponseMessage response = await httpClient.GetAsync($"appointments?fetchXml={encodedFetch}");
                    if (response.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<Appointments>(await response.Content.ReadAsStringAsync());
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }           
        }

        public async Task<CrmUser> GetCrmUser(string botUserId)
        {
            using (HttpClient httpClient = await GetClient())
            {
                try
                {                   
                    // レコードの取得
                    HttpResponseMessage response = await httpClient.GetAsync($"systemusers(new_botuserid='{botUserId}')?$select=fullname,systemuserid");
                    if (response.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<CrmUser>(await response.Content.ReadAsStringAsync());
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public async Task Update(string entitySet, Guid id, object record)
        {
            // HttpClient の作成 
            using (HttpClient httpClient = await GetClient())
            {
                try
                {                    
                    // レコードの更新
                    HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{entitySet}({id})");
                    request.Content = new StringContent(JsonConvert.SerializeObject(record), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                        return;
                    else
                        throw new Exception(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                }
            }
        }

        private async Task<HttpClient> GetClient(bool isGet = false)
        {
            HttpClient client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            client.BaseAddress = new Uri(Settings.CrmUri + "api/data/v8.1/");
            var accessToken = await ADALService.GetAccessToken(Settings.CrmUri);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (!string.IsNullOrEmpty(CallerId))
                client.DefaultRequestHeaders.Add("MSCRMCallerID", CallerId);

            if (isGet)
            {
                client.DefaultRequestHeaders.Add(
                    "Accept", "application/json");
                client.DefaultRequestHeaders.Add(
                    "Prefer", "odata.include-annotations=\"*\"");
            }
            return client;
        }
    }
}
