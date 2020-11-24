using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OctopusEnergy.SmartMeterData
{
    class SmartMeterReadings
    {
        private static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            IConfiguration initialiseConfig(string jsonConfigName)
            {
                // Build configuration

                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                    .AddJsonFile(jsonConfigName, false)
                    .Build();

                return config;
            }

            Task getElectricityReadings(string ApiKey, string CURL)
            {
                //electricity call
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), CURL))
                    {
                        var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Concat(ApiKey, ":")));
                        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                        //var response = await httpClient.SendAsync(request);
                        var response = httpClient.SendAsync(request);
                        return response;
                    }
                }
            }

            Task getGasReadings(string ApiKey, string CURL)
            {
                //gas call
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), CURL))
                    {
                        var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Concat(ApiKey, ":")));
                        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                        //var response = await httpClient.SendAsync(request);
                        var response = httpClient.SendAsync(request);
                        return response;
                    }
                }
            }

            Configuration = initialiseConfig("appsettings.json");

            string APIKey = Configuration.GetSection("APIKey").Value;
            Console.WriteLine(String.Concat("API Key: ",  APIKey));

            //electricity params
            string electricityMeterMPAN = Configuration.GetSection("Electricity:MPAN").Value ;
            Console.WriteLine(String.Concat("electricityMeterMPAN: ", electricityMeterMPAN));

            string electricityMeterSerial = Configuration.GetSection("Electricity:Serial").Value;
            Console.WriteLine(String.Concat("electricityMeterSerial: ", electricityMeterSerial));

            string electricityCURL = String.Concat("https://api.octopus.energy/v1/electricity-meter-points/", electricityMeterMPAN, "/meters/", electricityMeterSerial, "/consumption/");
            Console.WriteLine(String.Concat("electricityCURL: ", electricityCURL));

            //gas params
            string gasMeterMPRN = Configuration.GetSection("Gas:MPRN").Value; ;
            Console.WriteLine(String.Concat("gasMeterMPRN: ", gasMeterMPRN));

            string gasMeterSerial = Configuration.GetSection("Gas:Serial").Value; ;
            Console.WriteLine(String.Concat("gasMeterSerial: ", gasMeterSerial));

            string gasCURL = String.Concat("https://api.octopus.energy/v1/gas-meter-points/", gasMeterMPRN, "/meters/", gasMeterSerial, "/consumption/");
            Console.WriteLine(String.Concat("gasCURL: ", gasCURL));

            var electricity = getElectricityReadings(APIKey, electricityCURL);

            var gas = getGasReadings(APIKey, gasCURL);

            string agileTariffPart1 = "AGILE-18-02-21";
            string agileTariffPart2 = "E-1R-AGILE-18-02-21-B";

            string agileTariffCURL = String.Concat("https://api.octopus.energy/v1/products/", agileTariffPart1, "/electricity-tariffs/", agileTariffPart2, "/standard-unit-rates/");

            //get agile tariff
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), agileTariffCURL))
                {
                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Concat(APIKey, ":")));
                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                    var response = httpClient.SendAsync(request);
                    //var response = await httpClient.SendAsync(request);
                }
            }
        }
    }
}
