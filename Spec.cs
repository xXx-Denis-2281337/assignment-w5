using FluentAssertions;
using Xunit;
using FsCheck;
using System.Collections.Generic;
using System;
using System.Linq;
using KmaOoad18.Assignments.Week5.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using SutStartup = KmaOoad18.Assignments.Week5.Startup;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace KmaOoad18.Assignments.Week5
{
    public class Spec :
    IDisposable,
    IClassFixture<WebApplicationFactory<SutStartup>>
    {
        private readonly WebApplicationFactory<SutStartup> _factory;

        public Spec(WebApplicationFactory<SutStartup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot("./");
                builder.ConfigureServices(services =>
                {
                    services.AddDbContext<LoyaltyContext, TestContext>(options =>
                    options.UseSqlite("Data Source=test.db"));
                });
            });
        }

        [Fact]
        public async Task BasicScenario()
        {
            // Given loyalty client
            var client = new LoyaltyCustomerTestClient(_factory);

            // And some customer
            var name = "James Jameson";
            var phone = "088 913-49-84";
            var loyaltyCard = string.Empty;

            // And some product 
            var sku = $"sku{DateTime.Now.Ticks}";
            var price = 100m;
            await client.AddProduct(sku, sku, price);


            // When I launch customer's loyalty program
            loyaltyCard = await client.LaunchLoyalty(name, phone);

            // Then I expect that customer's balance to be zero
            (await client.LoyaltyBalance(loyaltyCard)).Should().Be(0);

            // When I purchase some product with qty=1 and price=100
            var purchase = new List<(string, int)> { (sku, 1) };

            await client.ProcessPurchase(purchase, loyaltyCard);

            // Then I expect balance to be 10 (0 + 100 * 1 * 0.1)
            var balance1 = await client.LoyaltyBalance(loyaltyCard);
            balance1.Should().Be(10);

            // When I add special offering to make every bonus X10
            await client.AddSpecialOffering(sku, PromotionType.Multiplier, 10);

            // And make same purchase
            await client.ProcessPurchase(purchase, loyaltyCard);

            // Then I expect balance to be 110 (10 + 100 * 1 * 0.1 * 10)
            var balance2 = await client.LoyaltyBalance(loyaltyCard);
            balance2.Should().Be(110);

            // When I remove special offering
            await client.RemoveSpecialOffering(sku);

            // And make same purchase
            await client.ProcessPurchase(purchase, loyaltyCard);

            // Then I expect balance to be 120 (110 + 100 * 1 * 0.1)
            var balance3 = await client.LoyaltyBalance(loyaltyCard);
            balance3.Should().Be(120);


            // When I make same purchase and use bonus
            await client.ProcessPurchase(purchase, loyaltyCard, useLoyaltyPoints: true);

            // Then I expect balance to be 75, because 50 is spent as discount for half amount, customer pays 100-50=50 and receives 5 points for bonus
            var balance4 = await client.LoyaltyBalance(loyaltyCard);
            balance4.Should().Be(75);
        }



        #region Extended Tests


        [Fact]
        public async Task CanProcessPurchaseExtended()
        {
            var (products, purchased) = SeedProducts();

            // Given loyalty client
            var client = new LoyaltyCustomerTestClient(_factory);

            // And loyalty card
            var loyaltyCard = await client.LaunchLoyalty("Rick Richardson", "074 454-89-90");

            // And some products
            foreach (var p in products)
            {
                await client.AddProduct(p.Sku, p.Name, p.Price);
            }

            // When I process customer's purchase
            await client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance
            var balance1 = await client.LoyaltyBalance(loyaltyCard);

            // And then process same purchase
            await client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance again
            var balance2 = await client.LoyaltyBalance(loyaltyCard);

            // Then I expect balance to double
            balance2.Should().BeGreaterThan(0);
            balance2.Should().Be(balance1 * 2);
        }

        [Fact]
        public async Task CanApplySpecialOfferingsExtended()
        {
            var (products, purchased) = SeedProducts();


            // Given loyalty client
            var client = new LoyaltyCustomerTestClient(_factory);

            // And loyalty card
            var loyaltyCard = await client.LaunchLoyalty("Rick Richardson", "074 454-89-90");

            // And some products
            foreach (var p in products)
            {
                await client.AddProduct(p.Sku, p.Name, p.Price);
            }

            // When I process customer's purchase
            await client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance
            var balance1 = await client.LoyaltyBalance(loyaltyCard);

            // And add X3 special offering
            foreach (var p in purchased)
            {
                await client.AddSpecialOffering(p.Sku, PromotionType.Multiplier, 3);
            }

            // And then process same purchase again
            await client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance again
            var balance2 = await client.LoyaltyBalance(loyaltyCard);

            // Then I expect balance to quadruple!!!
            balance2.Should().BeGreaterThan(0);
            balance2.Should().Be(balance1 * 4);
        }

        [Fact]
        public async Task CanRemoveSpecialOfferingsExtended()
        {
            var (products, purchased) = SeedProducts();


            // Given loyalty client
            var client = new LoyaltyCustomerTestClient(_factory);

            // And loyalty card
            var loyaltyCard = await client.LaunchLoyalty("Rick Richardson", "074 454-89-90");

            // And some products
            foreach (var p in products)
            {
                await client.AddProduct(p.Sku, p.Name, p.Price);
            }

            // When I process customer's purchase
            await client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance
            var balance1 = await client.LoyaltyBalance(loyaltyCard);

            // And add X3 special offering
            foreach (var p in purchased)
            {
                await client.AddSpecialOffering(p.Sku, PromotionType.Multiplier, 3);
            }

            // And then process same purchase again
            await client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And then remove special offering
            foreach (var p in purchased)
            {
                await client.RemoveSpecialOffering(p.Sku);
            }

            // And process same purchase once more
            await client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);


            // And get balance again
            var balance2 = await client.LoyaltyBalance(loyaltyCard);

            // Then I expect balance grows 5 times
            balance2.Should().BeGreaterThan(0);
            balance2.Should().Be(balance1 * 5);
        }

        [Fact]
        public async Task CanDeductLoyaltyPointsExtended()
        {
            var (products, purchased) = SeedProducts();

            // Given loyalty client
            var client = new LoyaltyCustomerTestClient(_factory);

            // And loyalty card
            var loyaltyCard = await client.LaunchLoyalty("Rick Richardson", "074 454-89-90");

            // And some products
            foreach (var p in products)
            {
                await client.AddProduct(p.Sku, p.Name, p.Price);
            }

            // When I process customer's purchase
            await client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard);

            // And get balance
            var balance1 = await client.LoyaltyBalance(loyaltyCard);

            // And then process same purchase again with loyalty deduction=ON
            await client.ProcessPurchase(purchased.Select(p => (p.Sku, p.Qty)).ToList(), loyaltyCard, true);

            // And get balance again
            var balance2 = await client.LoyaltyBalance(loyaltyCard);

            // Then I expect balance to be 90% of previous one
            balance2.Should().BeGreaterThan(0);
            balance2.Should().Be(balance1 * 0.9m);
        }


        private (List<Product>, List<Purchase>) SeedProducts()
        {
            var seed = DateTime.Now.Second + 3;
            var factor = (seed % 17) + 4;
            var productSeed = seed * factor;

            var prices = new List<int>();

            for (int i = 0; i < factor; i++)
            {
                prices.Add(productSeed + i);
            }

            var products = prices.Distinct().Select(price => new Product { Name = $"sku{price}", Sku = $"sku{price}", Price = price * 1.0m }).ToList();

            var purchased = products.Take(products.Count > 5 ? 5 : products.Count).Select(pp => new Purchase { Sku = pp.Sku, Qty = factor }).ToList();

            return (products, purchased);
        }

        public void Dispose()
        {
            using (var db = new TestContext())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM [SpecialOfferings]");
                db.Database.ExecuteSqlCommand("DELETE FROM [Products]");
                db.Database.ExecuteSqlCommand("DELETE FROM [Customers]");
            }
        }

        private struct Purchase
        {
            public string Sku;
            public int Qty;
        }

        private struct Product
        {
            public string Name;
            public string Sku;
            public decimal Price;
        }

        #endregion
    }

    internal class LoyaltyCustomerTestClient
    {
        private readonly HttpClient _client;

        public LoyaltyCustomerTestClient(WebApplicationFactory<SutStartup> factory)
        {
            this._client = factory.CreateClient();
        }

        internal async Task AddProduct(string sku, string name, decimal price)
        => await Post("/api/admin/products",
                        new ProductDto
                        {
                            Name = name,
                            Sku = sku,
                            Price = price
                        });

        internal async Task AddSpecialOffering(string sku, string promotionType, int promo)
        => await Post($"/api/admin/products/{sku}/special-offerings",
                        new PromotionDto
                        {
                            PromotionType = promotionType,
                            PromotionValue = promo
                        });


        internal async Task<string> LaunchLoyalty(string customerName, string customerPhone)
        => await (await Post("/api/admin/loyalties",
                            new NewLoyaltyDto
                            {
                                CustomerName = customerName,
                                CustomerPhone = customerPhone
                            })
            ).Content
            .ReadAsStringAsync();


        internal async Task<decimal> LoyaltyBalance(string loyaltyCard)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/customer/{loyaltyCard}/balance");

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            
            return Decimal.Parse(responseContent, provider: CultureInfo.InvariantCulture);
        }

        internal async Task ProcessPurchase(List<(string Sku, int Qty)> list, string loyaltyCard, bool useLoyaltyPoints = false)
        => await Post($"/api/customer/{loyaltyCard}/purchase",
                        new OrderDto
                        {
                            OrderItems = list.Select(i => new OrderItemDto { Qty = i.Qty, Sku = i.Sku }).ToList(),
                            UseLoyaltyPoints = useLoyaltyPoints
                        });

        internal async Task RemoveSpecialOffering(string sku)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/admin/products/{sku}/special-offerings");

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }


        private async Task<HttpResponseMessage> Post<T>(string path, T dto)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, path);

            request.Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }

    internal class TestContext : LoyaltyContext
    {
        public TestContext()
        {
        }

        public TestContext(DbContextOptions<LoyaltyContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=test.db");
        }
    }
}