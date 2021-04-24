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

namespace STR.AZFunc
{
    public static class GetMachineInformation
    {
        [FunctionName("GetMachineInformation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Get), Route = null)] HttpRequest req,
            ILogger log)
        {

            string machineID = req.Headers["machineid"];

            if (string.IsNullOrEmpty(machineID))
            {
                log.LogError("Invalid app settings configured - machine not specified");
                return new InternalServerErrorResult();
            }

            log.LogInformation($"Getting information for machine: {machineID}");

            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
               .Create("60840d11-dbd4-4927-92e8-c10656621ddb")
               .WithTenantId("de46ae9d-eaed-4ac7-91dc-0454e314c3b6")
               .WithClientSecret("~62MsXzg2zvSn~ZoqkCz-k-4-kIC5I~l9e")
               .Build();

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);


            object machineInfo = null;
            try
            {
                machineInfo = await graphClient.Users[machineID].Request().GetAsync();
            }
            catch
            {
                return new OkObjectResult(null);
            }


            return new OkObjectResult(machineInfo);





        }

    }
}
