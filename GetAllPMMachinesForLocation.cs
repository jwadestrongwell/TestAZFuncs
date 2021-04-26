using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System.Web.Http;
using System.Collections.Generic;

namespace STR.AZFunc
{
    public static class GetAllPMMachinesForLocation
    {
        [FunctionName("GetAllPMMachinesForLocation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Get), Route = null)] HttpRequest req,
            ILogger log)
        {

            string locationName = req.Query["location"];

            if (string.IsNullOrEmpty(locationName))
            {
                log.LogError("Invalid app settings configured - Location not specified");
                return new InternalServerErrorResult();
            }

            log.LogInformation($"Getting all PM machines for: {locationName}");

            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
               .Create("60840d11-dbd4-4927-92e8-c10656621ddb")
               .WithTenantId("de46ae9d-eaed-4ac7-91dc-0454e314c3b6")
               .WithClientSecret("~62MsXzg2zvSn~ZoqkCz-k-4-kIC5I~l9e")
               .Build();

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            var queryOptions = new List<Microsoft.Graph.QueryOption>()
            {
                new Microsoft.Graph.QueryOption("filter", $"officeLocation/any(a:a+eq+'{locationName}')")

                // filter=startswith(displayName,'a')
            };


            object machineList = null;
            try
            {
                //categories/any(a:a+eq+'Red+Category')
                // var request = graphClient.Users.Request().Filter($"officeLocation/any(a:a+eq+'{locationName}')");
                // string filterString = $"startswith(displayName, 'PM') and startswith(officeLocation, 'Bri')";
                //   string filterString = $"startswith(displayName, 'PM')";
                 string filterString = $"startswith(displayName, 'PM-')";
                //string filterString = $"startswith(OfficeLocation, 'BRI')";
                var request = graphClient.Users.Request().Filter(filterString).Select(x => new
                {
                    x.Id,
                    x.DisplayName,                 
                    x.UserPrincipalName,
                    x.AccountEnabled,
                    x.Identities,                  
                    x.OfficeLocation,              
                    x.Mail
                });

                var result = await request.GetAsync();
                machineList = result;
            }



            catch (Exception err)
            {
                return new BadRequestObjectResult(err.Message);
            }

            return new OkObjectResult(machineList);





        }

    }
}
