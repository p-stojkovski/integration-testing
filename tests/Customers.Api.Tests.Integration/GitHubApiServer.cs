using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Customers.Api.Tests.Integration;

internal class GitHubApiServer : IDisposable
{
    private WireMockServer _server;

    public string Url => _server.Url!;

    public void Start()
    {
        _server = WireMockServer.Start();
    }

    public void SetupUser(string username)
    {
        _server
            .Given(Request.Create()
                .WithPath($"/users/{username}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(GenerateGitHubUserResponseBody(username))
                .WithHeader("content-type", "application/json; charset=utf-8")
                .WithStatusCode(200));
    }

    public void SetupThrottledUser(string username)
    {
        _server
            .Given(Request.Create()
                .WithPath($"/users/{username}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(@"
                    ""message"": ""API rate limit exceeded for 127.0.0.1"",
                    ""documentation_url"": ""https://docs.github.com/rest/overview""
                ")
                .WithHeader("content-type", "application/json; charset=utf-8")
                .WithStatusCode(403));
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }

    private static string GenerateGitHubUserResponseBody(string username)
    {
        return $@"{{
                  ""id"": ""12345"",
                  ""username"": ""{username}"",
                  ""email"": ""{username}@example.com"",
                }}";
    }
}
