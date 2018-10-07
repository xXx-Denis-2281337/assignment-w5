# Loyalty Web API

[![Join the chat at https://gitter.im/kmaooad18/assignment-w5](https://badges.gitter.im/kmaooad18/assignment-w5.svg)](https://gitter.im/kmaooad18/assignment-w5?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

This task is built on top of the previous one (see week 4). Logic remains the same, but shape of the app changes. This time you have to implement loyalty calculations as an ASP.NET Core Web API application.

## ASP.NET Core

One of the goals of this task is getting familiar with basics of ASP.NET Core. So the logic of the app (loyalty calculation) remains the same as in the previous task, so you wouldn't need to distract on that part too much and could concentrate on other important topics.

You are provided with the starter code for Web API application. Files you need to modify for this task are `Startup.cs`, `LoyaltyAdminController.cs` and `LoyaltyCustomerController.cs`. Latter two are like `LoyaltyClient` in the previous task. These two controllers are the entry points for HTTP requests coming to your app. You will need to take you loyalty logic from old client and put it in proper places in controllers. IMPORTANT: Don't modify API endpoints (URL paths, parameters) that are defined in controllers - otherwise tests will not be able to run properly and will fail. See [routing documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-2.1) to get basic idea about how ASP.NET Core finds controller and method for incoming HTTP request.

### DTOs

One special change comparing to old loyalty client is that now controller methods accept parameters shaped in separate classes, e.g. `ProductDto`, `OrderDto` etc. These parameters are automatically deserialized by ASP.NET Core when you make and HTTP request to some URL, but you have to provide data for requests in body of POST request in JSON form.

### Swagger

Swagger is a tool that generates standard specification for API and shows it in human-friendly UI. Run your application with `dotnet run` command and by default web server will be started at http://localhost:5000. Now go to http://localhost:5000/swagger and see how API UI looks like. You can easily test your API from this page.

## Dependency Injection

One of main goals of this task is showing you how to use DI. ASP.NET Core comes with a built-in DI container. Classes that DI container can operate are configured in `ConfigureServices` method in `Startup.cs`. Check [DI documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1) and see how to add your `LoyaltyContext` to DI container. After that you will need to add constructors to controllers and put `LoyaltyContext` as parameter in those constructors. This will be your _constructor injection_. Assign passed `LoyaltyContext` to some private field and then you will be able to use it in your methods to work with DB.