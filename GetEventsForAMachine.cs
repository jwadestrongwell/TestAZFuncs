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
    public static class GetEventsForAMachine
    {
        [FunctionName("GetEventsForAMachine")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Get), Route = null)] HttpRequest req,
            ILogger log)
        {
            string machineID = req.Headers["machineid"];

            string startDate = req.Query["startdate"];

            string endDate = req.Query["enddate"];

            if (string.IsNullOrEmpty(machineID))
            {
                log.LogError("Invalid app settings configured - missing MachineID");
                return new InternalServerErrorResult();
            }

            if (string.IsNullOrEmpty(startDate) ||
               string.IsNullOrEmpty(endDate))
            {
                log.LogError("Invalid app settings configured- missing start or end date");
                return new InternalServerErrorResult();
            }

            log.LogInformation($"Getting listing of events for machine: {machineID}");
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
               .Create("60840d11-dbd4-4927-92e8-c10656621ddb")
               .WithTenantId("de46ae9d-eaed-4ac7-91dc-0454e314c3b6")
               .WithClientSecret("~62MsXzg2zvSn~ZoqkCz-k-4-kIC5I~l9e")
               .Build();

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);


            var queryOptions = new List<Microsoft.Graph.QueryOption>()
            {
                new Microsoft.Graph.QueryOption("startDateTime", startDate),
                new Microsoft.Graph.QueryOption("endDateTime", endDate)
            };


            object machineEventsList = null;
            try
            {
                machineEventsList = await graphClient.Users[machineID].CalendarView.Request(queryOptions).GetAsync();
            }
            catch
            {
                return new OkObjectResult(null);
            }


            return new OkObjectResult(machineEventsList);
        }
    }
}



