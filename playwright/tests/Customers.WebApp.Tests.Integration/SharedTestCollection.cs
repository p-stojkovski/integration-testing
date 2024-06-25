using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.WebApp.Tests.Integration;

[CollectionDefinition("Test collection")]
public class SharedTestCollection : ICollectionFixture<SharedTestContext>
{
}
