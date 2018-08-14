using System;
using System.IO;
using System.Threading.Tasks;
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
            //Arrange
            var repo = new Mock<IRepository<ICustomer>>();
            repo.Setup(x => x.GetById(1)).Returns(() =>
            {
                        return new Customer{Id = 1, CustomerName = "Acme", CreatedOn = DateTime.Now};

            });

            var mockClientProxy = new Mock<IClientProxy>();            

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            var hub = new Mock<IHubContext<NotificationHub>>();
            hub.Setup(x => x.Clients).Returns(() => mockClients.Object);

            var p = new ServiceCollection()
             .AddSingleton <IRepository<ICustomer>> (repo.Object)
             .AddTransient<HomeController, HomeController>()
             .AddSingleton<IHubContext<NotificationHub>>(hub.Object)
             .BuildServiceProvider();

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
                .Should()
                .Be("Acme");
        }
        [Fact]
        public async Task TestToUpdateCustomer()
        {
            var repo = new Mock<IRepository<ICustomer>>();
            repo.Setup(x => x.GetById(1)).Returns(() =>
            {
                return new Customer{Id = 1, CustomerName = "Acme", CreatedOn = DateTime.Now};
            });

            var mockClientProxy = new Mock<IClientProxy>();            

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            var hub = new Mock<IHubContext<NotificationHub>>();
            hub.Setup(x => x.Clients).Returns(() => mockClients.Object);  

            var provider = new ServiceCollection()
             .AddSingleton <IRepository<ICustomer>> (repo.Object)
             .AddTransient<HomeController, HomeController>()
             .AddSingleton<IHubContext<NotificationHub>>(hub.Object)
             .BuildServiceProvider();

            var customer = new Customer { CustomerName = "Acme", Id=1, CreatedOn = DateTime.Now };
            var controller = provider.GetService<HomeController>();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Headers.Add("Cookie", "userid=mchammer;recordno=2");

            //Act
            var albumc = await controller.UpdateCustomer(customer);

            //Assert
            albumc
                .Should()
                .BeAssignableTo<OkObjectResult>()
                .Subject.Value
                .Should()
                .BeAssignableTo<Customer>()
                .Subject
                .CustomerName
                .Should()
                .Be("Acme");
        }
    }
}
