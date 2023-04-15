using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Savonia.xUnit.Helpers.Infrastructure;

public class WebApplicationFactoryFixture<T> : WebApplicationFactory<T> where T : class
{
    public WebApplicationFactoryFixture()
    {
    }

    public string ServerUrl { get; private set; } = "http://localhost:7048";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseUrls(ServerUrl);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // need to create a plain host that we can return.
        var dummyHost = builder.Build();

        // configure and start the actual host.
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());

        var host = builder.Build();
        host.Start();

        return dummyHost;
    }
}
