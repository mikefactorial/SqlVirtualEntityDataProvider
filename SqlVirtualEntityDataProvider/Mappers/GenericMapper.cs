using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikeFactorial.Xrm.Plugins.DataProviders.Mappers
{
    public class GenericMapper
    {
        protected PluginExecutionContext context { get; set; }
        private EntityMetadata primaryEntityMetadata = null;

        public GenericMapper(PluginExecutionContext context)
        {
            this.context = context;
        }
        public EntityMetadata PrimaryEntityMetadata
        {
            get
            {
                if(primaryEntityMetadata == null)
                {
                    //Create RetrieveEntityRequest
                    RetrieveEntityRequest retrievesEntityRequest = new RetrieveEntityRequest
                    {
                        EntityFilters = EntityFilters.Entity | EntityFilters.Attributes,
                        LogicalName = context.PluginContext.PrimaryEntityName
                    };

                    //Execute Request
                    RetrieveEntityResponse retrieveEntityResponse = (RetrieveEntityResponse)context.Service.Execute(retrievesEntityRequest);
                    primaryEntityMetadata = retrieveEntityResponse.EntityMetadata;
                }

                return primaryEntityMetadata;
            }
        }

        public virtual string MapVirtualEntityAttributes(string sql)
        {
            var iEnum = this.GetCustomMappings().GetEnumerator();
            while (iEnum.MoveNext())
            {
                sql = sql.Replace(iEnum.Current.Key, iEnum.Current.Value);
            }

            return sql;
        }

        public virtual EntityCollection CreateEntities(DataSet dataSet, int pageSize, int pageNumber)
        {
            var collection = new EntityCollection();
            collection.TotalRecordCount = dataSet.Tables[0].Rows.Count;
            collection.MoreRecords = (collection.TotalRecordCount > (pageSize * pageNumber)) || pageSize == -1;
            if(dataSet != null && dataSet.Tables.Count > 0)
            {
                var rows = (pageSize > -1) ? dataSet.Tables[0].AsEnumerable().Skip(pageSize * (pageNumber - 1)).Take(pageSize) : dataSet.Tables[0].AsEnumerable();
                foreach (DataRow row in rows)
                {
                    Entity entity = new Entity(context.PluginContext.PrimaryEntityName);
                    foreach(DataColumn col in dataSet.Tables[0].Columns)
                    {
                        if(row[col] != null && row[col] != DBNull.Value)
                        {
                            var entityAttribute = this.PrimaryEntityMetadata.Attributes.FirstOrDefault(a => a.ExternalName == col.ColumnName);
                            if(entityAttribute != null)
                            {
                                entity[entityAttribute.LogicalName] = MapToVirtualEntityValue(entityAttribute, row[col]);
                            }
                        }
                    }
                    collection.Entities.Add(entity);
                }
            }
            return collection;
        }

        public virtual Dictionary<string, string> GetCustomMappings()
        {
            Dictionary<string, string> mappings = new Dictionary<string, string>();

            foreach (var att in PrimaryEntityMetadata.Attributes)
            {
                if(!string.IsNullOrEmpty(att.ExternalName))
                {
                    mappings.Add(att.LogicalName, att.ExternalName);
                }
            }
            mappings.Add(PrimaryEntityMetadata.LogicalName, PrimaryEntityMetadata.ExternalName);

            return mappings;
        }

        public virtual object MapToVirtualEntityValue(string attributeName, object value)
        {
            var att = this.PrimaryEntityMetadata.Attributes.FirstOrDefault(a => a.LogicalName == attributeName);
            return MapToVirtualEntityValue(att, value);
        }

        public virtual object MapToVirtualEntityValue(AttributeMetadata entityAttribute, object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            else if(entityAttribute.LogicalName == this.PrimaryEntityMetadata.PrimaryIdAttribute && Int32.TryParse(value.ToString(), out int keyInt))
            {
                //This is a generic method of creating a guid from an int value if no guid is available in the database
                return new Guid(keyInt.ToString().PadLeft(32, 'a'));
            }
            else if (entityAttribute is LookupAttributeMetadata && Int32.TryParse(value.ToString(), out int lookupInt))
            {
                var lookup = new EntityReference(((LookupAttributeMetadata)entityAttribute).Targets[0], new Guid(lookupInt.ToString().PadLeft(32, 'a')));
                return lookup;
            }
            else if (entityAttribute is PicklistAttributeMetadata && Int32.TryParse(value.ToString(), out int picklistInt))
            {
                return new OptionSetValue(picklistInt);
            }
            else if (Int32.TryParse(value.ToString().Replace("{", string.Empty).Replace("}", string.Empty).Replace("a", string.Empty).Replace("A", string.Empty).Replace("-", string.Empty), out int intValue))
            {
                //This converts the generated guid back to an int. 
                return intValue.ToString();
            }
            else
            {
                return value;
            }
        }
    }
}
