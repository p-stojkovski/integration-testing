using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Api.Tests.Integration.CustomerController;

public class DeleteCustomerControllerTests : IClassFixture<CustomerApiFactory>
{
    private readonly HttpClient _httpClient;

    private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
        .RuleFor(x => x.Email, f => f.Person.Email)
        .RuleFor(x => x.FullName, f => f.Person.FullName)
        .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGitHubUser)
        .RuleFor(x => x.DateOfBirth, f => f.Person.DateOfBirth.Date);

    public DeleteCustomerControllerTests(CustomerApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Delete_ReturnsOk_WhenCustomerExists()
    {
        //Arrange
        var customer = _customerGenerator.Generate();
        var createdResponse = await _httpClient.PostAsJsonAsync("customers", customer);
        var createdCustomer = await createdResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        //Act
        var response = await _httpClient.DeleteAsync($"customers/{createdCustomer!.Id}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenCustomerDoesNotExists()
    {
        //Act
        var response = await _httpClient.DeleteAsync($"customers/{Guid.NewGuid()}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
