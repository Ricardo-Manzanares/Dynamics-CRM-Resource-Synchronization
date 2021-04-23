using Microsoft.Xrm.Sdk.Query;
using CRMResourceSynchronization.Core.Dynamics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRMResourceSynchronization.Core.Business.Models;
using Microsoft.Xrm.Sdk;
using CRMResourceSynchronization.Core.Dynamics.Extensions;

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
        /// Obtener los recursos de la solución indicada por parámetro
        /// </summary>
        /// <returns></returns>
        public List<ResourceModel> GetResourcesFromSolution(Guid solution)
        {
            try
            {
                List<ResourceModel> resources = new List<ResourceModel>();

                if (this._CRMClient == null)
                    throw new Exception("La conexion a CRM no esta configurada, es necesaria antes de conectar a CRM");

                var queryExpresion = new QueryExpression
                {
                    EntityName = EntityNames.Recursos,
                    ColumnSet = new ColumnSet("webresourceid", "name", "createdon", "modifiedon", "content", "webresourcetype"),
                    Distinct = true,
                    NoLock = true
                };

                LinkEntity le_resources = new LinkEntity("webresource", "solutioncomponent", "webresourceid", "objectid", JoinOperator.Inner);
                le_resources.LinkCriteria.AddCondition("solutionid", ConditionOperator.Equal, solution);

                queryExpresion.LinkEntities.Add(le_resources);

                var response = this._CRMClient._Client.RetrieveMultiple(queryExpresion);

                foreach (dynamic item in response.Entities)
                {
                    resources.Add(EntityToModel(item));
                }

                return resources;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Obtener el contenido de los recursos indicados por parámetro
        /// </summary>
        /// <returns></returns>
        public List<ResourceModel> GetContentResourcesFromSolution(List<Guid> resources)
        {
            try
            {
                List<ResourceModel> lstResources = new List<ResourceModel>();

                if (this._CRMClient == null)
                    throw new Exception("La conexion a CRM no esta configurada, es necesaria antes de conectar a CRM");

                var queryExpresion = new QueryExpression
                {
                    EntityName = EntityNames.Recursos,
                    ColumnSet = new ColumnSet("webresourceid", "name", "createdon", "modifiedon", "content", "webresourcetype"),
                    Distinct = true,
                    NoLock = true
                };

                LinkEntity le_resources = new LinkEntity("webresource", "solutioncomponent", "webresourceid", "objectid", JoinOperator.Inner);
                le_resources.LinkCriteria.AddCondition("webresourceid", ConditionOperator.Contains, resources);

                queryExpresion.LinkEntities.Add(le_resources);

                var response = this._CRMClient._Client.RetrieveMultiple(queryExpresion);

                foreach (dynamic item in response.Entities)
                {
                    resources.Add(EntityToModel(item));
                }

                return lstResources;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Mapea el model de usuario a entidad de CRM
        /// </summary>
        /// <param name="recurso"></param>
        /// <returns></returns>
        private dynamic MapperToEntity(ResourceModel recurso)
        {
            Entity entity = new Entity(EntityNames.Recursos, recurso.resourceid);
            entity["name"] = recurso.name;
            entity["createdon"] = recurso.createdon;
            entity["modifiedon"] = recurso.modifiedon;
            entity["content"] = recurso.content;
            entity["webresourcetype"] = recurso.webresourcetype;
            return entity;
        }

        /// <summary>
        /// Mapea el usuario de CRM a modelo
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
            model.content = EntityCollectionExtension.ExistProperty(recurso, "content") ? recurso.Attributes["content"].ToString() : null;
            model.webresourcetype = EntityCollectionExtension.ExistProperty(recurso, "webresourcetype") ? ((OptionSetValue)recurso.Attributes["webresourcetype"]).Value : 0;

            return model;
        }
    }
}
