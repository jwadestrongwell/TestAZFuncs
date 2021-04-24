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
            var machineInfo = await graphClient.Users[machineID].Request().GetAsync();

            if (machineInfo == null)
            {
                return new OkObjectResult(null);
            }
 
            return new OkObjectResult(machineInfo);

        }

        public static async void SetEventOnPultrusionMachine(ILogger log, string machineid)
        {
            // Build a client application.
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create("60840d11-dbd4-4927-92e8-c10656621ddb")
                .WithTenantId("de46ae9d-eaed-4ac7-91dc-0454e314c3b6")
                .WithClientSecret("~62MsXzg2zvSn~ZoqkCz-k-4-kIC5I~l9e")
                .Build();

            // var scopes = new string[] { "https://graph.microsoft.com/.default" };

            // AuthenticationResult authResult = await confidentialClientApplication.AcquireTokenForClient(scopes).ExecuteAsync();
            // log.LogInformation($"retrieved token: {authResult.AccessToken}");

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            // Create a new instance of GraphServiceClient with the authentication provider.
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            //var pm1id = "554061a6-a3c0-44c4-97a3-17681ea361f8";

            var pm1User = await graphClient.Users[machineid].Request().GetAsync();

            log.LogInformation("retrieved pm1");

            // //https://graph.microsoft.com/v1.0/users/554061a6-a3c0-44c4-97a3-17681ea361f8/calendar/events

            DateTime dummyStart = new DateTime(2021, 04, 24, 4, 00, 00);
            DateTime dummyEnd = new DateTime(2021, 04, 25, 4, 00, 00);
            Microsoft.Graph.Event dummyEvent = new Event();
            dummyEvent.Start = DateTimeTimeZone.FromDateTime(dummyStart, "America/New_York");
            dummyEvent.End = DateTimeTimeZone.FromDateTime(dummyEnd, "America/New_York");
            dummyEvent.Subject = "Shop Order #12312";
            var machLocation = new Location();
            machLocation.DisplayName = "PM-1";
            dummyEvent.Location = machLocation;
            dummyEvent.IsReminderOn = false;
            object setEvent;
            try
            {
                setEvent = await graphClient.Users[machineid].Calendar.Events.Request().AddAsync(dummyEvent);
                log.LogInformation($"CalendarUID: {((Event)setEvent).ICalUId}");
            }
            catch (ServiceException svcerr)
            {
                log.LogInformation($"ErrorMSG: {svcerr.Message}");
                log.LogInformation($"StatusCode:  {svcerr.StatusCode.ToString()}");
            }
            var x = 1;
        }
    }


}
