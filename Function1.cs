using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            string city = "Buenos Aires,AR"; //default
            string lang = "en"; //default
            var parameters = req.GetQueryNameValuePairs();

            if (parameters.Count() > 0 )
            {
                foreach (var item in parameters)
                {
                    if (item.Key.ToLower() == "city")
                    {
                        city = item.Value;
                    }
                    else if (item.Key.ToLower() == "lang")
                    {
                        lang = item.Value;
                    }

                }

            }

            string url = "https://api.openweathermap.org/data/2.5/weather?q="+ city + "&lang=" + lang + "&appid=d272d30facc707d3fc4b8f0b3ab67c6d";
            var client = new HttpClient();
            var response = client.GetAsync(url).Result;
            var content = response.Content;
            string responseString = content.ReadAsStringAsync().Result;
            var weather = JsonConvert.DeserializeObject<WeatherResponse>(responseString as string);
            string weatherState = weather.weather[0].description;
            string cityName = weather.name;


            return string.IsNullOrEmpty(weatherState) == true || string.IsNullOrEmpty(cityName) == true

                ? req.CreateResponse(HttpStatusCode.BadRequest, "Error with the API request.")
                : req.CreateResponse(HttpStatusCode.OK, $"The weather in {cityName} is {weatherState.ToLower()}" );
        }
    }


    class Weather
    {
        public int id { get; set; }
        public string description { get; set; }
    }

    class WeatherResponse
    {
        public System.Collections.Generic.IList<Weather> weather { get; set; }
        public string name { get; set; }
    }

}
