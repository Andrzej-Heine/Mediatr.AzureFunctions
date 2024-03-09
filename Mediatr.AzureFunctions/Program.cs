using FluentValidation;
using IsolatedMediatr.DateStore;
using IsolatedMediatr.Models;
using IsolatedMediatr.Requests;
using IsolatedMediatr.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddSingleton<BlobDataStore>();
        services.AddSingleton<QueueMessageRequest>();
        services.AddSingleton<AbstractValidator<Person>, PersonValidator>();
    })
    .Build();

host.Run(); 