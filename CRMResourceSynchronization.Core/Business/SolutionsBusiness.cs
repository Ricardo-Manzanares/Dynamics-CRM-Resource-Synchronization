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
    public class SolutionsBusiness
    {
        private CRMClient _CRMClient;
        private SettingsModel _Settings;
        public SolutionsBusiness(CRMClient CRMClient, SettingsModel Settings)
        {
            this._CRMClient = CRMClient;
            this._Settings = Settings;
        }
        
        /// <summary>
        /// Get solutions managed from CRM
        /// </summary>
        /// <returns></returns>
        public List<SolutionModel> GetSolutionsManaged()
        {
            try
            {
                List<SolutionModel> solutiones = new List<SolutionModel>();

                if (this._CRMClient == null)
                    throw new Exception("The connection to CRM is not configured, it is necessary before connecting to CRM");

                var queryExpresion = new QueryExpression
                {
                    EntityName = EntityNames.Soluciones,
                    ColumnSet = new ColumnSet("solutionid", "friendlyname"),
                    Distinct = true,
                    NoLock = true
                };

                queryExpresion.Criteria.AddCondition(new ConditionExpression("ismanaged", ConditionOperator.Equal, "0"));

                var response = this._CRMClient._Client.RetrieveMultiple(queryExpresion);
                foreach (dynamic item in response.Entities)
                {
                    SolutionModel parseSolution = EntityToModel(item);
                    if(parseSolution != null)
                    solutiones.Add(parseSolution);
                }

                return solutiones;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Mapper model solution to entity CRM
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        private Entity MapperToEntity(SolutionModel usuario)
        {
            Entity entity = new Entity(EntityNames.Soluciones, Guid.Parse(usuario.solutionid));
            entity["friendlyname"] = usuario.friendlyname;
            return entity;
        }

        /// <summary>
        /// Mapper entity CRM to model solution
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        private SolutionModel EntityToModel(Entity usuario)
        {
            SolutionModel model = new SolutionModel();
            Guid parseSolutionId = Guid.Empty;
            if (usuario.Attributes["solutionid"] != null && Guid.TryParse(usuario.Attributes["solutionid"].ToString(), out parseSolutionId))
            {
                model.solutionid = parseSolutionId.ToString();
                model.friendlyname = EntityCollectionExtension.ExistProperty(usuario, "friendlyname") ? usuario["friendlyname"].ToString() : "--";
                return model;
            }
            else
            {
                return null;
            }            
        }
    }
}
