using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Customers.Api.Tests.Integration.CustomerController;

public class CreateCustomerControllerTests : IClassFixture<CustomerApiFactory>
{
    private readonly CustomerApiFactory _factory;
    private readonly HttpClient _httpClient;

    private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
        .RuleFor(x => x.Email, f => f.Person.Email)
        .RuleFor(x => x.FullName, f => f.Person.FullName)
        .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGitHubUser)
        .RuleFor(x => x.DateOfBirth, f => f.Person.DateOfBirth.Date);

    public CreateCustomerControllerTests(CustomerApiFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Create_CreatesUser_WhenDataIsValid()
    {
        //Arrange
        var customer = _customerGenerator.Generate();

        //Act
        var response = await _httpClient.PostAsJsonAsync("customers", customer);

        //Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();

        customerResponse.Should().BeEquivalentTo(customer);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should()
            .Be($"http://localhost/customers/{customerResponse!.Id}");
    }
}
