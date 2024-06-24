using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

var wireMockServer = WireMockServer.Start();

Console.WriteLine($"Wiremock is now running on: {wireMockServer.Url}");

wireMockServer
    .Given(Request.Create()
        .WithPath("/users/petar")
        .UsingGet())
    .RespondWith(Response.Create()
        //.WithBody("This is coming from wire mock")
        .WithBodyAsJson(new { id = "12345", username = "petar" })
        .WithHeader("content-type", "application/json; charset=utf-8")
        .WithStatusCode(200));

Console.ReadKey();
wireMockServer.Dispose();