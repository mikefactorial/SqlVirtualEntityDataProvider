using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using MikeFactorial.Xrm.Plugins.DataProviders.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MikeFactorial.Xrm.Plugins.DataProviders
{
    /// <summary>
    /// Converts FetchXml to SQL
    /// </summary>
    public class FetchToSqlVisitor
    {
        private Dictionary<string, string> aliasmap;
        private List<string> selectcols;
        private List<string> ordercols;

        private IOrganizationService organizationService;
        private GenericMapper mapper;

        public FetchToSqlVisitor(IOrganizationService service, GenericMapper mapper)
        {
            this.organizationService = service;
            this.mapper = mapper;
        }

        public string Visit(FetchType fetch)
        {
            aliasmap = new Dictionary<string, string>();
            var sql = new StringBuilder();
            var entity = fetch.Items.FirstOrDefault(i => i is FetchEntityType) as FetchEntityType;

            sql.Append("SELECT ");
            if (fetch.distinctSpecified && fetch.distinct)
            {
                sql.Append("DISTINCT ");
            }

            if (!string.IsNullOrEmpty(fetch.top))
            {
                sql.Append($"TOP {fetch.top} ");
            }

            if (entity.Items != null)
            {
                selectcols = GetSelect(entity);
                ordercols = GetOrder(entity.Items.Where(i => i is FetchOrderType).ToList(), string.Empty);
                var join = GetJoin(entity.Items.Where(i => i is FetchLinkEntityType).ToList(), entity.name);
                var where = GetWhere(entity.name, string.Empty, entity.Items.Where(i => i is filter && ((filter)i).Items != null && ((filter)i).Items.Length > 0).ToList());
                sql.AppendLine(string.Join(", ", selectcols));
                sql.AppendLine($" FROM {entity.name} WITH(NOLOCK)");
                if (join != null && join.Count > 0)
                {
                    sql.AppendLine(string.Join("\n", join));
                }

                if (!string.IsNullOrEmpty(where))
                {
                    sql.AppendLine($" WHERE {where}");
                }

                if (ordercols != null && ordercols.Count > 0)
                {
                    sql.Append(" ORDER BY ");
                    sql.AppendLine(string.Join(", ", ordercols));
                }
                else
                {
                    sql.Append($" ORDER BY {mapper.PrimaryEntityMetadata.PrimaryIdAttribute}");
                }

                /* Might come back to this later and do all paging server side
                if (Int32.TryParse(fetch.page, out int page) && Int32.TryParse(fetch.count, out int count))
                {
                    sql.AppendLine($" OFFSET ({page}) * {count} ROWS FETCH NEXT {count} ROWS ONLY");
                }
                */
            }

            return sql.ToString();
        }

        /// <summary>
        /// Gets the select.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Gets the select statement for the query</returns>
        private List<string> GetSelect(FetchEntityType entity)
        {
            var result = new List<string>();
            var attributeitems = entity.Items.Where(i => i is FetchAttributeType && ((FetchAttributeType)i).name != null).ToList();
            if (attributeitems.Count > 0)
            {
                foreach (FetchAttributeType attributeitem in attributeitems)
                {
                    result.Add(attributeitem.name);
                }
            }
            else
            {
                result.Add("*");
            }

            return result;
        }

        /// <summary>
        /// Gets the join.
        /// </summary>
        /// <param name="linkentities">The linkentities.</param>
        /// <param name="entityalias">The entityalias.</param>
        /// <param name="organizationService">The organization service.</param>
        /// <returns>Join statement as a list of strings</returns>
        private List<string> GetJoin(List<object> linkentities, string entityalias)
        {
            var joinList = new List<string>();
            foreach (FetchLinkEntityType linkitem in linkentities)
            {
                var join = new StringBuilder();
                if (linkitem.linktype == "outer")
                {
                    join.Append("LEFT OUTER ");
                }

                var linkalias = string.IsNullOrEmpty(linkitem.alias) ? linkitem.name : linkitem.alias;
                if (linkalias != linkitem.name)
                {
                    aliasmap.Add(linkalias, linkitem.name);
                }

                join.Append($"JOIN {linkitem.name} {linkalias} ON {linkalias}.{linkitem.from} = {entityalias}.{linkitem.to}");
                if (linkitem.Items != null)
                {
                    var linkwhere = GetWhere(linkitem.name, linkalias, linkitem.Items.Where(i => i is filter && ((filter)i).Items != null && ((filter)i).Items.Length > 0).ToList());
                    if (!string.IsNullOrEmpty(linkwhere))
                    {
                        join.Append($" AND {linkwhere} ");
                    }
                }

                joinList.Add(join.ToString().Trim());
                if (linkitem.Items != null)
                {
                    selectcols.AddRange(GetExpandedSelect(linkitem, linkalias));
                    ordercols.AddRange(GetOrder(linkitem.Items.Where(i => i is FetchOrderType).ToList(), linkalias));
                    joinList.AddRange(GetJoin(linkitem.Items.Where(i => i is FetchLinkEntityType).ToList(), linkalias));
                }
            }

            return joinList;
        }

        /// <summary>
        /// Gets the expanded select.
        /// </summary>
        /// <param name="linkitem">The linkitem.</param>
        /// <param name="relation">The relation.</param>
        /// <returns>Returns the select statement.</returns>
        /// <exception cref="Exception">Invalid M:M-relation definition for OData</exception>
        private List<string> GetExpandedSelect(FetchLinkEntityType linkitem, string relation)
        {
            var resultList = new List<string>();
            if (linkitem.Items != null)
            {
                var attributeitems = linkitem.Items.Where(i => i is FetchAttributeType && ((FetchAttributeType)i).name != null).ToList();
                if (linkitem.intersect)
                {
                    var linkitems = linkitem.Items.Where(i => i is FetchLinkEntityType).ToList();

                    if (linkitems.Count == 1)
                    {
                        var nextlink = (FetchLinkEntityType)linkitems[0];
                        attributeitems = nextlink.Items.Where(i => i is FetchAttributeType && ((FetchAttributeType)i).name != null).ToList();
                    }
                }

                if (attributeitems.Count > 0)
                {
                    foreach (FetchAttributeType attributeitem in attributeitems)
                    {
                        resultList.Add(relation + "." + attributeitem.name);
                    }
                }
            }

            return resultList;
        }

        /// <summary>
        /// Gets the where.
        /// </summary>
        /// <param name="entityname">The entityname.</param>
        /// <param name="entityalias">The entityalias.</param>
        /// <param name="filteritems">The filteritems.</param>
        /// <param name="organizationService">The organization service.</param>
        /// <returns>Returns the where statement for the query</returns>
        private string GetWhere(string entityname, string entityalias, List<object> filteritems)
        {
            var resultList = new StringBuilder();
            if (filteritems.Count > 0)
            {
                foreach (filter filteritem in filteritems)
                {
                    resultList.Append(GetFilter(entityname, entityalias, filteritem, organizationService));
                }
            }

            return resultList.ToString();
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <param name="entityname">The entityname.</param>
        /// <param name="entityalias">The entityalias.</param>
        /// <param name="filteritem">The filteritem.</param>
        /// <param name="organizationService">The organization service.</param>
        /// <returns>Returns a filter statement for the query</returns>
        private string GetFilter(string entityname, string entityalias, filter filteritem, IOrganizationService organizationService)
        {
            var result = new StringBuilder();
            if (filteritem.Items == null || filteritem.Items.Length == 0)
            {
                return string.Empty;
            }

            var logical = filteritem.type == filterType.or ? " OR " : " AND ";
            if (filteritem.Items.Length > 1)
            {
                result.Append("(");
            }

            foreach (var item in filteritem.Items)
            {
                if (item is condition)
                {
                    result.Append(GetCondition(entityname, entityalias, item as condition, organizationService));
                }
                else if (item is filter)
                {
                    result.Append(GetFilter(entityname, entityalias, item as filter, organizationService));
                }

                result.Append(logical);
            }

            if (result.ToString().EndsWith(logical))
            {
                string temp = result.ToString();
                result.Clear();
                result.Append(temp.Substring(0, temp.Length - logical.Length));
            }

            if (filteritem.Items.Length > 1)
            {
                result.Append(")");
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <param name="entityname">The entityname.</param>
        /// <param name="entityalias">The entityalias.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="organizationService">The organization service.</param>
        /// <returns>Condition as string</returns>
        /// <exception cref="Exception">
        /// No metadata for attribute: {entityname}.{condition.attribute}
        /// or
        /// Unsupported SQL condition operator '{condition.@operator}'
        /// </exception>
        private string GetCondition(string entityname, string entityalias, condition condition, IOrganizationService organizationService)
        {
            var result = new StringBuilder();
            if (!string.IsNullOrEmpty(entityalias))
            {
                result.Append(entityalias);
                result.Append(".");
            }

            if (!string.IsNullOrEmpty(condition.attribute))
            {
                if (!string.IsNullOrEmpty(condition.entityname))
                {
                    result.Append($"{condition.entityname}.");
                    if (aliasmap.ContainsKey(condition.entityname))
                    {
                        entityname = aliasmap[condition.entityname];
                    }
                    else
                    {
                        entityname = condition.entityname;
                    }
                }

                result.Append(condition.attribute);

                var req = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityname,
                    LogicalName = condition.attribute,
                    RetrieveAsIfPublished = true
                };
                var retrieveAttributeResponse = (RetrieveAttributeResponse)organizationService.Execute(req);

                var attrMeta = retrieveAttributeResponse.AttributeMetadata;

                switch (condition.@operator)
                {
                    case @operator.eq:
                    case @operator.on:
                        result.Append(" = ");
                        break;
                    case @operator.ne:
                    case @operator.neq:
                        result.Append(" != ");
                        break;
                    case @operator.@in:
                        result.Append(" IN ");
                        break;
                    case @operator.lt:
                        result.Append(" < ");
                        break;
                    case @operator.le:
                    case @operator.onorbefore:
                        result.Append(" <= ");
                        break;
                    case @operator.gt:
                        result.Append(" > ");
                        break;
                    case @operator.ge:
                    case @operator.onorafter:
                        result.Append(" >= ");
                        break;
                    case @operator.@null:
                        result.Append(" IS NULL");
                        break;
                    case @operator.notnull:
                        result.Append(" IS NOT NULL");
                        break;
                    case @operator.like:
                        result.Append(" LIKE ");
                        break;
                    case @operator.notlike:
                        result.Append(" NOT LIKE ");
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(condition.value))
                {
                    switch (attrMeta.AttributeType)
                    {
                        case AttributeTypeCode.Money:
                        case AttributeTypeCode.BigInt:
                        case AttributeTypeCode.Boolean:
                        case AttributeTypeCode.Decimal:
                        case AttributeTypeCode.Double:
                        case AttributeTypeCode.Integer:
                        case AttributeTypeCode.State:
                        case AttributeTypeCode.Status:
                        case AttributeTypeCode.Picklist:
                            result.Append(mapper.MapToVirtualEntityValue(attrMeta, condition.value));
                            break;
                        default:
                            result.Append($"'{mapper.MapToVirtualEntityValue(attrMeta, condition.value)}'");
                            break;
                    }
                }
                else if(condition.Items.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("(");
                    for(int i = 0; i < condition.Items.Length; i++)
                    {
                        if(i > 0)
                        {
                            sb.Append(", ");
                        }
                        sb.Append($"'{mapper.MapToVirtualEntityValue(attrMeta, condition.Items[i].Value) }'");
                    }
                    sb.Append(")");
                    result.Append(sb.ToString());
                }
            }

            return result.ToString();
        }

        private List<string> GetOrder(List<object> orderitems, string entityalias)
        {
            var result = new List<string>();
            foreach (FetchOrderType orderitem in orderitems)
            {
                var order = new StringBuilder();
                if (!string.IsNullOrEmpty(entityalias))
                {
                    order.Append($"{entityalias}.");
                }

                if (!string.IsNullOrEmpty(orderitem.alias))
                {
                    order.Append(orderitem.alias);
                }
                else
                {
                    order.Append(orderitem.attribute);
                }

                if (orderitem.descending)
                {
                    order.Append(" DESC");
                }

                result.Add(order.ToString());
            }

            return result;
        }
    }
}