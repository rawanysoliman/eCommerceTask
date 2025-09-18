using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Ecommerce.Application.Dtos.Auth;
using Ecommerce.Tests.Integration.Utils;
using Xunit;

namespace Ecommerce.Tests.Integration
{
	public class ProductsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
	{
		private readonly CustomWebApplicationFactory _factory;
		public ProductsIntegrationTests(CustomWebApplicationFactory factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetProducts_Unauthorized_Without_Token()
		{
			var client = _factory.CreateClient();
			var res = await client.GetAsync("/api/Products");
			Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
		}

		
	}
}
