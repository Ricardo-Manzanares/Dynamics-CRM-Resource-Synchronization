using Microsoft.Xrm.Sdk.Query;
using CRMResourceSynchronization.Core.Dynamics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRMResourceSynchronization.Core.Business.Models;
using Microsoft.Xrm.Sdk;
using CRMResourceSynchronization.Core.Dynamics.Extensions;
using System.IO;
using System.Text;
using System.Linq;

namespace CRMResourceSynchronization.Core.Business
{
    public class ResourcesBusiness
    {
        private CRMClient _CRMClient;

        public ResourcesBusiness(CRMClient CRMClient)
        {
            this._CRMClient = CRMClient;
        }
        
        /// <summary>
        /// Get resources from solution parameter
        /// </summary>
        /// <returns></returns>
        public List<ResourceModel> GetResourcesFromSolution(Guid solution)
        {
            try
            {
                List<ResourceModel> resources = new List<ResourceModel>();

                if (this._CRMClient == null)
                    throw new Exception("The connection to CRM is not configured, it is necessary before connecting to CRM");

                var queryExpresion = new QueryExpression
                {
                    EntityName = EntityNames.Recursos,
                    ColumnSet = new ColumnSet("webresourceid"),
                    Distinct = true,
                    NoLock = true
                };

                LinkEntity le_resources = new LinkEntity("webresource", "solutioncomponent", "webresourceid", "objectid", JoinOperator.Inner);
                le_resources.LinkCriteria.AddCondition("solutionid", ConditionOperator.Equal, solution);

                queryExpresion.LinkEntities.Add(le_resources);

                var response = this._CRMClient._Client.RetrieveMultiple(queryExpresion);

                foreach (dynamic item in response.Entities)
                {
                    var responseResource = this._CRMClient._Client.Retrieve(EntityNames.Recursos, Guid.Parse(item.Attributes["webresourceid"].ToString()), new ColumnSet("webresourceid", "name", "createdon", "modifiedon", "content", "webresourcetype"));
                    resources.Add(EntityToModel(responseResource));
                }

                return resources;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Mapper model resource to resource entity
        /// </summary>
        /// <param name="recurso"></param>
        /// <returns></returns>
        private dynamic MapperToEntity(ResourceModel recurso)
        {
            Entity entity = new Entity(EntityNames.Recursos, recurso.resourceid);
            entity["name"] = recurso.name;
            entity["createdon"] = recurso.createdon;
            entity["modifiedon"] = recurso.modifiedon;
            entity["content"] = recurso.contentCRM;
            entity["webresourcetype"] = recurso.webresourcetype;
            return entity;
        }

        /// <summary>
        /// Mapper resource entity to model resource
        /// </summary>
        /// <param name="recurso"></param>
        /// <returns></returns>
        private ResourceModel EntityToModel(Entity recurso)
        {
            ResourceModel model = new ResourceModel();
            model.name = recurso.Attributes["name"] != null ? recurso.Attributes["name"].ToString() : "--";
            model.resourceid = Guid.Parse(recurso.Attributes["webresourceid"].ToString());
            model.createdon = EntityCollectionExtension.ExistProperty(recurso, "createdon") ? recurso.Attributes["createdon"].ToString() : "--";
            model.modifiedon = EntityCollectionExtension.ExistProperty(recurso, "modifiedon") ? recurso.Attributes["modifiedon"].ToString() : "--";
            model.resourcesStatus = ResourceContentStatus.Equals;
            model.webresourcetype = EntityCollectionExtension.ExistProperty(recurso, "webresourcetype") ? ((OptionSetValue)recurso.Attributes["webresourcetype"]).Value : 0;
            model.contentCRM = EntityCollectionExtension.ExistProperty(recurso, "content") ? recurso.Attributes["content"].ToString() : "";

            if (!string.IsNullOrEmpty(model.contentCRM))
                model.contentRowsCRM = getRowsFromContent(model.contentCRM);

            return model;
        }

        private List<ResourceContentModel> getRowsFromContent (string content)
        {
            List<ResourceContentModel> rowsResource = new List<ResourceContentModel>();

            MemoryStream ms = new MemoryStream(Convert.FromBase64String(content));
            using (var reader = new StreamReader(ms, Encoding.UTF8))
            {
                //string[] result = reader.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                List<string> rows = reader.ReadToEnd().Split(Environment.NewLine.ToCharArray()).ToList();

                for (int i = 0; i < rows.Count; i++)
                {
                    ResourceContentModel resourceContent = new ResourceContentModel();
                    resourceContent.numRow = i + 1;
                    resourceContent.textRow = rows[i];
                    resourceContent.statusRow = (i % 2 == 0) ? ResourceContentRowStatus.Difference : ResourceContentRowStatus.Delete;
                    rowsResource.Add(resourceContent);
                }
            }

            return rowsResource;
        }
    }
}
