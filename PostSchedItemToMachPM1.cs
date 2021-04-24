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

namespace STR.AZFunc
{
    public static class PostSchedItemToMachPM1
    {
        [FunctionName("PostSchedItemToMachPM1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Post), Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            SetPM1Event(log);

            return new OkObjectResult(responseMessage);
        }

        public static async void SetPM1Event(ILogger log)
        {

            var bodyInfo = "{\"subject\": \"Let's run a part\", \"body\": {\"contentType\": \"HTML\", \"content\": \"Does noon work for you?\" }, \"start\": {  \"dateTime\": \"2021-04-21T12:00:00\",      \"timeZone\": \"Eastern Standard Time\"  },  \"end\": {      \"dateTime\": \"2021-04-22T14:00:00\",      \"timeZone\": \"Eastern Standard Time\"  },  \"location\":{      \"displayName\":\"Harry's Bar\"  }}";

            // Build a client application.
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create("60840d11-dbd4-4927-92e8-c10656621ddb")
                .WithTenantId("de46ae9d-eaed-4ac7-91dc-0454e314c3b6")
                .WithClientSecret("~62MsXzg2zvSn~ZoqkCz-k-4-kIC5I~l9e")
                .Build();

            var scopes = new string[] { "https://graph.microsoft.com/.default" };

            AuthenticationResult authResult = await confidentialClientApplication.AcquireTokenForClient(scopes).ExecuteAsync();
            log.LogInformation($"retrieved token: {authResult.AccessToken}");

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            // Create a new instance of GraphServiceClient with the authentication provider.
             GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            var pm1id = "554061a6-a3c0-44c4-97a3-17681ea361f8";

            // var user = await graphClient.Me.Request().GetAsync();

            var pm1User = await graphClient.Users[pm1id].Request().GetAsync();

            log.LogInformation("retrieved pm1");

            // //https://graph.microsoft.com/v1.0/users/554061a6-a3c0-44c4-97a3-17681ea361f8/calendar/events

            DateTime dummyStart = new DateTime(2021, 04, 23, 5, 10, 00);
            DateTime dummyEnd = new DateTime(2021, 04, 24, 5, 30, 00);
            Microsoft.Graph.Event dummyEvent = new Event();
            dummyEvent.Start = DateTimeTimeZone.FromDateTime(dummyStart, "America/New_York");
            dummyEvent.End = DateTimeTimeZone.FromDateTime(dummyEnd, "America/New_York");
            dummyEvent.Subject = "my Test Run";
            object setEvent;


            // var bearerToken = authProvider.ClientApplication.UserTokenCache..

            try
            {
                setEvent = await graphClient.Users[pm1id].Calendar.Events.Request().AddAsync(dummyEvent);
            }
            catch (Exception err)
            {
                log.LogInformation(err.Message);
            }
            var x = 1;
            log.LogInformation("added calendar event to pm1");
        }
    }


}
