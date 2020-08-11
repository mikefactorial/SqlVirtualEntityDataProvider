using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using MikeFactorial.Xrm.Plugins.DataProviders.Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MikeFactorial.Xrm.Plugins.DataProviders
{
    /// <summary>
    /// Virtual Entity Data Provider for SQL
    /// </summary>
    public class SqlVirtualEntityDataProvider : EntityPluginBase
    {
        /// <summary>
        /// Handles the entity retrieve message.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void HandleEntityRetrieveMessage(PluginExecutionContext context)
        {
            base.HandleEntityRetrieveMessage(context);
            var mapper = new GenericMapper(context);
            Entity entity = new Entity(context.PluginContext.PrimaryEntityName);

            if (mapper != null)
            {
                string sql = $"SELECT * FROM {context.PluginContext.PrimaryEntityName} WITH(NOLOCK) WHERE {mapper.PrimaryEntityMetadata.PrimaryIdAttribute} = '{mapper.MapToVirtualEntityValue(mapper.PrimaryEntityMetadata.PrimaryIdAttribute, context.PluginContext.PrimaryEntityId)}'";
                sql = mapper.MapVirtualEntityAttributes(sql);

                using (SqlConnection sqlConnection = new SqlConnection(context.GetSqlConnectionString()))
                {
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sql, sqlConnection);
                    DataSet dataSet = new DataSet();
                    sqlConnection.Open();
                    sqlDataAdapter.Fill(dataSet, "DxtrData");
                    sqlConnection.Close();
                    context.Trace($"Records Retrieved: {dataSet.Tables[0].Rows.Count}", Array.Empty<object>());

                    var collection = mapper.CreateEntities(dataSet, 1, 1);
                    if (collection.Entities != null && collection.Entities.Count > 0)
                    {
                        entity = collection.Entities[0];
                    }
                }
            }

            // Set output parameter
            context.PluginContext.OutputParameters["BusinessEntity"] = entity;
        }

        /// <summary>
        /// Handles the entity retrieve multiple message.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void HandleEntityRetrieveMultipleMessage(PluginExecutionContext context)
        {
            base.HandleEntityRetrieveMultipleMessage(context);

            var qe = (QueryExpression)context.PluginContext.InputParameters["Query"];
            var mapper = new GenericMapper(context);

            EntityCollection collection = new EntityCollection();
            QueryExpressionToFetchXmlRequest convertRequest = new QueryExpressionToFetchXmlRequest();
            convertRequest.Query = qe;
            var response = (QueryExpressionToFetchXmlResponse)context.Service.Execute(convertRequest);
            context.Trace($"FetchXML: {response.FetchXml}");
            FetchToSqlTranslator translator = new FetchToSqlTranslator(context.Service, mapper);
            var fetch = FetchType.Deserialize(response.FetchXml);
            string sql = translator.GetSQLQuery(fetch);

            sql = mapper.MapVirtualEntityAttributes(sql);

            context.Trace($"SQL: {sql}");
            using (SqlConnection sqlConnection = new SqlConnection(context.GetSqlConnectionString()))
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sql, sqlConnection);
                DataSet dataSet = new DataSet();
                sqlConnection.Open();
                sqlDataAdapter.Fill(dataSet, "SqlData");
                sqlConnection.Close();
                context.Trace($"Records Retrieved: {dataSet.Tables[0].Rows.Count}", Array.Empty<object>());
                if(Int32.TryParse(fetch.page, out int page) && Int32.TryParse(fetch.count, out int count))
                {
                    collection = mapper.CreateEntities(dataSet, count, page);
                }
                else
                {
                    collection = mapper.CreateEntities(dataSet, -1, 1);
                }
            }

            context.Trace($"Records Returned: {collection.Entities.Count}");
            context.PluginContext.OutputParameters["BusinessEntityCollection"] = collection;
        }
    }
}