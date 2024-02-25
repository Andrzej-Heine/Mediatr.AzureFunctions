using IsolatedMediatr.DateStore;
using IsolatedMediatr.Requests;
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
        //services.AddValidatorsFromAssemblyContaining<PersonValidator>();
    })
    .Build();

host.Run();