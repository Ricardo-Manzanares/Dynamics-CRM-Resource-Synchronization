using Microsoft.Xrm.Sdk.Query;
using DynamicsCRMResourceSynchronization.Core.Dynamics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicsCRMResourceSynchronization.Core.Business.Models;
using Microsoft.Xrm.Sdk;
using DynamicsCRMResourceSynchronization.Core.Dynamics.Extensions;
using Microsoft.Crm.Sdk.Messages;

namespace DynamicsCRMResourceSynchronization.Core.Business
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
                List<SolutionModel> solutions = new List<SolutionModel>();

                if (this._CRMClient == null)
                    throw new Exception("The connection to CRM is not configured, it is necessary before connecting to CRM");

                var queryExpresion = new QueryExpression
                {
                    EntityName = EntityNames.Solution,
                    ColumnSet = new ColumnSet("solutionid", "friendlyname", "uniquename"),
                    Distinct = true,
                    NoLock = true
                };

                queryExpresion.Criteria.AddCondition(new ConditionExpression("ismanaged", ConditionOperator.Equal, "0"));

                var response = this._CRMClient._Client.RetrieveMultiple(queryExpresion);
                foreach (dynamic item in response.Entities)
                {
                    SolutionModel parseSolution = EntityToModel(item);
                    if(parseSolution != null)
                        solutions.Add(parseSolution);
                }

                return solutions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get solutions by default from CRM
        /// </summary>
        /// <returns></returns>
        public SolutionModel GetSolutionDefault()
        {
            try
            {
                SolutionModel solutionDefault = new SolutionModel();

                List<SolutionModel> solutiones = new List<SolutionModel>();

                if (this._CRMClient == null)
                    throw new Exception("The connection to CRM is not configured, it is necessary before connecting to CRM");

                var queryExpresion = new QueryExpression
                {
                    EntityName = EntityNames.Solution,
                    ColumnSet = new ColumnSet("solutionid", "friendlyname", "uniquename"),
                    Distinct = true,
                    NoLock = true
                };

                queryExpresion.Criteria.AddCondition(new ConditionExpression("ismanaged", ConditionOperator.Equal, "0"));
                queryExpresion.Criteria.AddCondition(new ConditionExpression("uniquename", ConditionOperator.Equal, "Default"));

                var response = this._CRMClient._Client.RetrieveMultiple(queryExpresion);

                if(response.Entities.Count == 1)
                    solutionDefault = EntityToModel(response.Entities[0]);

                return solutionDefault;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add resource to solucion CRM
        /// </summary>
        /// <returns></returns>
        public bool AddResourceToSolution(SolutionModel solution, ResourceModel resource)
        {
            try
            {
                bool resourceAdd = false;

                AddSolutionComponentRequest addResource = new AddSolutionComponentRequest();
                addResource.ComponentType = 61;
                addResource.ComponentId = resource.resourceid;
                addResource.SolutionUniqueName = solution.uniquename;

                AddSolutionComponentResponse response = (AddSolutionComponentResponse)this._CRMClient._Client.Execute(addResource);

                if (response.id != Guid.Empty)
                    resourceAdd = true;

                return resourceAdd;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Remove resource to solucion CRM
        /// </summary>
        /// <returns></returns>
        public bool RemoveResourceToSolution(SolutionModel solution, ResourceModel resource)
        {
            try
            {
                bool resourceAdd = false;

                RemoveSolutionComponentRequest addResource = new RemoveSolutionComponentRequest();
                addResource.ComponentType = 61;
                addResource.ComponentId = resource.resourceid;
                addResource.SolutionUniqueName = solution.uniquename;

                RemoveSolutionComponentResponse response = (RemoveSolutionComponentResponse)this._CRMClient._Client.Execute(addResource);

                if (response.id != Guid.Empty)
                    resourceAdd = true;

                return resourceAdd;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Mapper model solution to entity CRM
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        private Entity MapperToEntity(SolutionModel solution)
        {
            Entity entity = new Entity(EntityNames.Solution, Guid.Parse(solution.solutionid));
            entity["friendlyname"] = solution.friendlyname;
            return entity;
        }

        /// <summary>
        /// Mapper entity CRM to model solution
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        private SolutionModel EntityToModel(Entity solution)
        {
            SolutionModel model = new SolutionModel();
            Guid parseSolutionId = Guid.Empty;
            if (solution.Attributes["solutionid"] != null && Guid.TryParse(solution.Attributes["solutionid"].ToString(), out parseSolutionId))
            {
                model.solutionid = parseSolutionId.ToString();
                model.friendlyname = EntityCollectionExtension.ExistProperty(solution, "friendlyname") ? solution["friendlyname"].ToString() : "--";
                model.uniquename = EntityCollectionExtension.ExistProperty(solution, "uniquename") ? solution["uniquename"].ToString() : "--";
                return model;
            }
            else
            {
                return null;
            }            
        }
    }
}
