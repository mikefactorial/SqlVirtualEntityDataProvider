using MarkMpn.Sql4Cds.Engine;
using MarkMpn.Sql4Cds.Engine.FetchXml;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MikeFactorial.Xrm.Plugins.DataProviders.Mappers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MikeFactorial.Xrm.Plugins.DataProviders
{
    /// <summary>
    /// Virtual Entity Data Provider for SQL
    /// </summary>
    public class SqlVirtualEntityDataProvider : PluginBase
    {
        /// <summary>
        /// Handles the entity retrieve message.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void HandleRetrieveMessage(PluginExecutionContext context)
        {
            base.HandleRetrieveMessage(context);
            var mapper = new GenericMapper(context);
            Entity entity = new Entity(context.PluginContext.PrimaryEntityName);

            if (mapper != null)
            {
                string sql = $"SELECT * FROM {context.PluginContext.PrimaryEntityName} WITH(NOLOCK) WHERE {mapper.PrimaryEntityMetadata.PrimaryIdAttribute} = '{mapper.MapToVirtualEntityValue(mapper.PrimaryEntityMetadata.PrimaryIdAttribute, context.PluginContext.PrimaryEntityId)}'";
                sql = mapper.MapVirtualEntityAttributes(sql);

                var entities = this.GetEntitiesFromSql(context, mapper, sql, 1, 1);
                if (entities.Entities != null && entities.Entities.Count > 0)
                {
                    entity = entities.Entities[0];
                }
            }

            // Set output parameter
            context.PluginContext.OutputParameters["BusinessEntity"] = entity;
        }

        /// <summary>
        /// Handles the entity retrieve multiple message.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void HandleRetrieveMultipleMessage(PluginExecutionContext context)
        {
            base.HandleRetrieveMultipleMessage(context);

            var query = context.PluginContext.InputParameters["Query"];
            if (query != null)
            {
                var mapper = new GenericMapper(context);

                EntityCollection collection = new EntityCollection();
                string fetchXml = string.Empty;
                if (query is QueryExpression qe)
                {
                    var convertRequest = new QueryExpressionToFetchXmlRequest();
                    convertRequest.Query = (QueryExpression)qe;
                    var response = (QueryExpressionToFetchXmlResponse)context.Service.Execute(convertRequest);
                    fetchXml = response.FetchXml;
                }
                else if(query is FetchExpression fe)
                {
                    fetchXml = fe.Query;
                }

                if(!string.IsNullOrEmpty(fetchXml))
                {
                    context.Trace($"Pre FetchXML: {fetchXml}");

                    var metadata = new AttributeMetadataCache(context.Service);
                    var fetch = Deserialize(fetchXml);
                    mapper.MapFetchXml(fetch);

                    //Store page info before converting
                    int page = -1;
                    int count = -1;
                    if (!string.IsNullOrEmpty(fetch.page))
                    {
                        page = Int32.Parse(fetch.page);
                        fetch.page = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(fetch.count))
                    {
                        count = Int32.Parse(fetch.count);
                        fetch.count = string.Empty;
                    }

                    var sql = FetchXml2Sql.Convert(metadata, fetch, new FetchXml2SqlOptions { PreserveFetchXmlOperatorsAsFunctions = false }, out _);

                    sql = mapper.MapVirtualEntityAttributes(sql);
                    context.Trace($"SQL: {sql}");

                    if (page != -1 && count != -1)
                    {
                        collection = this.GetEntitiesFromSql(context, mapper, sql, count, page);
                    }
                    else
                    {
                        collection = this.GetEntitiesFromSql(context, mapper, sql, -1, 1);
                    }
                }
                context.Trace($"Records Returned: {collection.Entities.Count}");
                context.PluginContext.OutputParameters["BusinessEntityCollection"] = collection;
            }
        }

        private EntityCollection GetEntitiesFromSql(PluginExecutionContext context, GenericMapper mapper, string sql, int pageSize, int pageNumber)
        {
            context.Trace($"SQL: {sql}");
            EntityCollection collection = new EntityCollection();
            using (SqlConnection sqlConnection = new SqlConnection(context.GetSqlConnectionString()))
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sql, sqlConnection);
                DataSet dataSet = new DataSet();
                sqlConnection.Open();
                sqlDataAdapter.Fill(dataSet, "SqlData");
                sqlConnection.Close();
                context.Trace($"Records Retrieved: {dataSet.Tables[0].Rows.Count}", Array.Empty<object>());
                collection = mapper.CreateEntities(dataSet, pageSize, pageNumber);
            }

            return collection;
        }

        /// <summary>
        /// Deserializes the fetch XML.
        /// </summary>
        /// <param name="fetchXml">The fetch XML.</param>
        /// <returns>Fetch Object for the FetchXML string</returns>
        private static FetchType Deserialize(string fetchXml)
        {
            var serializer = new XmlSerializer(typeof(FetchType));
            object result;
            using (TextReader reader = new StringReader(fetchXml))
            {
                result = serializer.Deserialize(reader);
            }

            return result as FetchType;
        }
    }
}