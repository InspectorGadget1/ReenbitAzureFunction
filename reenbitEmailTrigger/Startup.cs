using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using reenbitEmailTrigger.Services;

[assembly: FunctionsStartup(typeof(reenbitEmailTrigger.Startup))]
namespace reenbitEmailTrigger
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ISmtpService,SmtpService>();
            builder.Services.AddSingleton<ISasTokenService,SasTokenService>();
        }
    }
}
