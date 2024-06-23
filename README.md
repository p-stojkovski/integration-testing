# From Zero to Hero: Integration testing in ASP.NET Core

### Why is important?
- Individual software modules are combined and tested as a group.
- Evaluate the complice of a system or component with specific functional requirments.
- Usually checks for the 'happy and unhappy path'.

Types of integration testing:
- Big bang
- Mixed
- Risky hot
- Top-down (most common, easiest)
- Bottom-up

### Why should you write integration tests?
- Gives better idea of how the system will perform when integrating with other components.
- Perfect mixture of high level and low level test.
- Easy to retroactively add tests in a system that isn't testable (when not used DI, SOLID, Inverstion of control).

### What is in scope for integration testing
![image](https://github.com/p-stojkovski/integration-testing/assets/3589356/e69b55cb-0577-48bf-8232-4cd24519ac54)

### The 5 core integration testing steps
1. Setup (create db, spin up docker container, seed data...)
2. Dependency Mocking (API -> using, consuming, and you want fake api that performs as a real)
3. Execution
4. Assertion
5. Cleanup

### Naming
- Action_Return_When -> Get_ReturnsNotFound_WhenCustomerDoesNotExists

### WebApplicationFactory
- A way to actually instanciate and run the api we are testing against in memory without having to worry about where it is, how it can start, how it can run.
- When the test is disposed, the api is desposed alongside.
- Test server in memory
- test class inherit IClassFixture<WebApplicationFactory<IApiMarker>> -> every test in the class will reuse the same webApplicationFactory
