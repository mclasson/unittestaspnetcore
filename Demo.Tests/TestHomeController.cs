using System;
using System.IO;
using Demo.Definitions;
using Demo.Models;
using Demo.Web.Controllers;
using Demo.Web.Hubs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Demo.Tests
{
    public class TestHomeController
    {
        [Fact]
        public void TestToGetCustomerWithId()
        {
            var repo = new Mock<IRepository<ICustomer>>();
            repo.Setup(x => x.GetById(1)).Returns(() =>
              {
                        return new Customer{Id = 1, CustomerName = "Acme", CreatedOn = DateTime.Now};

              });



            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();            
            var hub = new Mock<IHubContext<NotificationHub>>();

            hub.Setup(x => x.Clients).Returns(() => mockClients.Object);

            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testSettings.json");

            IConfiguration config = builder.Build();

            var p = new ServiceCollection()
             .AddSingleton<IConfiguration>(config)
             .AddSingleton <IRepository<ICustomer>> (repo.Object)
             .AddTransient<HomeController, HomeController>()
             .AddSingleton<IHubContext<NotificationHub>>(hub.Object)
             .BuildServiceProvider();

            //var controller = new SpotifyController(repo.Object, config);

            var controller = p.GetService<HomeController>();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Headers.Add("Cookie", "userid=mchammer;recordno=2");
            var albumc = controller.Customer(1)
                .Should()
                .BeAssignableTo<ViewResult>()
                .Subject.Model
                .Should()
                .BeAssignableTo<Customer>()
                .Subject
                .CustomerName
                .Should().Be("Acme");
        }
    }
}
