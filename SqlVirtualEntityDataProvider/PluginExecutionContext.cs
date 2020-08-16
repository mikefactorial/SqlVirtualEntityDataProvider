using Microsoft.Xrm.Sdk;
using System;

namespace MikeFactorial.Xrm.Plugins.DataProviders
{
    public sealed class PluginExecutionContext : BaseExecutionContext, IDisposable, ITracingService
    {
        public PluginExecutionContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            Service = serviceFactory.CreateOrganizationService(Context.InitiatingUserId);
            Init();
        }

        public IPluginExecutionContext PluginContext => Context as IPluginExecutionContext;


        public void Dispose()
        {
        }
    }
}
