using Microsoft.Xrm.Sdk;

namespace MikeFactorial.Xrm.Plugins.DataProviders
{
    public static class EnvironmentSpecificExtensions
    {
        private const string dataSourceSqlConnectionStringAttribute = "mf_sqlconnectionstring";
        public static string GetSqlConnectionString(this PluginExecutionContext context)
        {
            var retrieverService = (IEntityDataSourceRetrieverService)context.ServiceProvider.GetService(typeof(IEntityDataSourceRetrieverService));
            var sourceEntity = retrieverService.RetrieveEntityDataSource();
            return sourceEntity[dataSourceSqlConnectionStringAttribute].ToString();
        }
    }
}
