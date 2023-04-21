using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Savonia.xUnit.Helpers.Infrastructure;

/// <summary>
/// Web application fixture class. When <see cref="HostUrl"/> is set then Kestrel is used as the test server on defined url (host and port).
/// By default (<see cref="HostUrl"/> is not set) uses <see href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.testhost.testserver">TestServer</see>
/// </summary>
/// <typeparam name="T"></typeparam>
public class WebApplicationFactoryFixture<T> : WebApplicationFactory<T> where T : class
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public WebApplicationFactoryFixture()
    {
    }
    private string hostUrl = string.Empty;
    /// <summary>
    /// Get or set test server url. When this is set then Kestrel server is used as a test server.
    /// </summary>
    public string HostUrl
    {
        get => hostUrl ?? base.Server.BaseAddress.ToString();
        set
        {
            hostUrl = value;
        }
    }

    /// <summary>
    /// Configure web host to use the set port.
    /// </summary>
    /// <param name="builder"></param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (string.IsNullOrEmpty(hostUrl))
        {
            base.ConfigureWebHost(builder);
        }
        else
        {
            builder.UseUrls(hostUrl);
        }
    }

    /// <summary>
    /// Create IHost
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        if (string.IsNullOrEmpty(hostUrl))
        {
            return base.CreateHost(builder);
        }
        else
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
}
