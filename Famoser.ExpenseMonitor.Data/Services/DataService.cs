using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Famoser.ExpenseMonitor.Data.Entities.Communication;
using Famoser.FrameworkEssentials.Logging;
using Newtonsoft.Json;

namespace Famoser.ExpenseMonitor.Data.Services
{
    public class DataService : IDataService
    {
        private const string ApiUrl = "https://api.expensemonitor.famoser.ch/";

        public Task<BooleanResponse> PostExpense(ExpenseRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            return PostForBoolean(new Uri(ApiUrl + "expenses/act"), json);
        }

        public Task<BooleanResponse> PostExpenseCollection(ExpenseCollectionRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            return PostForBoolean(new Uri(ApiUrl + "expensecollections/act"), json);
        }

        public async Task<ExpenseResponse> GetExpense(ExpenseRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var resp = await PostForString(new Uri(ApiUrl + "expenses/act"), json);
            if (resp.IsSuccessfull)
            {
                try
                {
                    return JsonConvert.DeserializeObject<ExpenseResponse>(resp.Response);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.LogException(ex, this);
                    return new ExpenseResponse()
                    {
                        ErrorMessage = "Unserialisation failed for Description " + resp.Response
                    };
                }
            }
            return new ExpenseResponse()
            {
                ErrorMessage = resp.ErrorMessage
            };
        }

        public async Task<ExpenseCollectionResponse> GetExpenseCollections(ExpenseCollectionRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var resp = await PostForString(new Uri(ApiUrl + "expensecollections/act"), json);
            if (resp.IsSuccessfull)
            {
                try
                {
                    return JsonConvert.DeserializeObject<ExpenseCollectionResponse>(resp.Response);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.LogException(ex, this);
                    return new ExpenseCollectionResponse()
                    {
                        ErrorMessage = "Unserialisation failed for Description " + resp.Response
                    };
                }
            }
            return new ExpenseCollectionResponse()
            {
                ErrorMessage = resp.ErrorMessage
            };
        }

        private async Task<StringReponse> DownloadString(Uri url)
        {
            try
            {
                using (var client = new HttpClient(
                    new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip
                                                 | DecompressionMethods.Deflate
                    }))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                    var resp = await client.GetAsync(url);
                    var res = new StringReponse()
                    {
                        Response = await resp.Content.ReadAsStringAsync()
                    };
                    if (resp.IsSuccessStatusCode)
                        return res;
                    res.ErrorMessage = "Request not successfull: Status Code " + resp.StatusCode + " returned. Message: " + res.Response;
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
                return new StringReponse()
                {
                    ErrorMessage = "Request failed for url " + url
                };
            }
        }

        private async Task<BooleanResponse> PostForBoolean(Uri url, string content)
        {
            BooleanResponse resp = null;
            try
            {
                using (var client = new HttpClient(
                    new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip
                                                 | DecompressionMethods.Deflate
                    }))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

                    var credentials = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("json", content)
                    });

                    var res = await client.PostAsync(url, credentials);
                    var respo = await res.Content.ReadAsStringAsync();
                    if (respo == "true")
                        resp = new BooleanResponse() { Response = true };
                    else
                    {
                        if (respo == "false")
                            resp = new BooleanResponse() { Response = false };
                        else
                        {
                            resp = new BooleanResponse() { ErrorMessage = respo };
                            LogHelper.Instance.Log(LogLevel.Error, "Post failed for url " + url + " with json " + content + " Reponse recieved: " + respo, this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
                resp = new BooleanResponse() { ErrorMessage = "Post failed for url " + url };
            }
            return resp;
        }

        private async Task<StringReponse> PostForString(Uri url, string content)
        {
            try
            {
                using (var client = new HttpClient(
                    new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip
                                                 | DecompressionMethods.Deflate
                    }))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

                    var credentials = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("json", content)
                    });

                    var res = await client.PostAsync(url, credentials);
                    var resp = new StringReponse()
                    {
                        Response = await res.Content.ReadAsStringAsync()
                    };
                    if (res.IsSuccessStatusCode)
                        return resp;
                    resp.ErrorMessage = "Request not successfull: Status Code " + res.StatusCode + " returned. Message: " + resp.Response;
                    return resp;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
                return new StringReponse()
                {
                    ErrorMessage = "Request failed for url " + url
                };
            }
        }
    }
}
