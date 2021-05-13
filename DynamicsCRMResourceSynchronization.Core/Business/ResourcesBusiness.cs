using Microsoft.Xrm.Sdk.Query;
using DynamicsCRMResourceSynchronization.Core.Dynamics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicsCRMResourceSynchronization.Core.Business.Models;
using Microsoft.Xrm.Sdk;
using DynamicsCRMResourceSynchronization.Core.Dynamics.Extensions;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Dynamic;
using DynamicsCRMResourceSynchronization.Core.DiffPlex.DiffBuilder;
using DynamicsCRMResourceSynchronization.Core.DiffPlex.DiffBuilder.Model;
using System.Security.AccessControl;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;

namespace DynamicsCRMResourceSynchronization.Core.Business
{
    public class ResourcesBusiness
    {
        private CRMClient _CRMClient;
        private SettingsModel _Settings;

        public ResourcesBusiness(CRMClient CRMClient, SettingsModel Settings)
        {
            this._CRMClient = CRMClient;
            this._Settings = Settings;
        }

        /// <summary>
        /// Get resources from solution parameter
        /// </summary>
        /// <returns>List<ResourceModel></returns>
        public List<ResourceModel> GetResourcesFromSolution(Guid solution)
        {
            try
            {
                List<ResourceModel> resources = new List<ResourceModel>();

                if (this._CRMClient == null)
                    throw new Exception("The connection to CRM is not configured, it is necessary before connecting to CRM");

                var queryExpresion = new QueryExpression
                {
                    EntityName = EntityNames.Resources,
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
                    resources.Add(DonwloadResourceFromCRM(Guid.Parse(item.Attributes["webresourceid"].ToString())));
                }

                return resources;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Donwload resource from CRM
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns>ResourceModel</returns>
        public ResourceModel DonwloadResourceFromCRM(Guid resourceId)
        {
            try
            {
                var responseResource = this._CRMClient._Client.Retrieve(EntityNames.Resources, resourceId, new ColumnSet("webresourceid", "name", "createdon", "modifiedon", "content", "webresourcetype"));
                return EntityToModel(responseResource);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Upload resources to CRM environment
        /// </summary>
        /// <param name="solution"></param>
        /// <returns>bool</returns>
        public List<ExecuteMultipleResponseModel> UploadResources(List<ResourceModel> resources)
        {
            List<ExecuteMultipleResponseModel> lst_Response = new List<ExecuteMultipleResponseModel>();

            try
            {
                if (this._CRMClient == null)
                    throw new Exception("The connection to CRM is not configured, it is necessary before connecting to CRM");

                ExecuteMultipleRequest executemultiplerequest = new ExecuteMultipleRequest();
                executemultiplerequest.Settings = new ExecuteMultipleSettings();
                executemultiplerequest.Settings.ContinueOnError = false;
                executemultiplerequest.Settings.ReturnResponses = true;

                executemultiplerequest.Requests = new OrganizationRequestCollection();

                foreach (var resource in resources)
                {
                    Entity resourceUpdate = MapperToEntity(resource);
                    UpdateRequest updateRequest = new UpdateRequest { Target = resourceUpdate };
                    executemultiplerequest.Requests.Add(updateRequest);
                }

                ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)this._CRMClient._Client.Execute(executemultiplerequest);

                lst_Response = ParseResponseExecuteMultiple(executemultiplerequest, responseWithResults);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lst_Response;
        }

        /// <summary>
        /// Upload and publish resources to CRM environment
        /// </summary>
        /// <param name="solution"></param>
        /// <returns>bool</returns>
        public List<ExecuteMultipleResponseModel> UploadAndPublishResources(List<ResourceModel> resources)
        {
            List<ExecuteMultipleResponseModel> lst_Response = new List<ExecuteMultipleResponseModel>();

            try
            {
                if (this._CRMClient == null)
                    throw new Exception("The connection to CRM is not configured, it is necessary before connecting to CRM");

                List<ExecuteMultipleResponseModel> resourcesToPublish = UploadResources(resources);

                if (resourcesToPublish.Where(k => k.error == false).Count() > 0)
                {
                    ExecuteMultipleRequest executemultiplerequest = new ExecuteMultipleRequest();
                    executemultiplerequest.Settings = new ExecuteMultipleSettings();
                    executemultiplerequest.Settings.ContinueOnError = false;
                    executemultiplerequest.Settings.ReturnResponses = true;

                    executemultiplerequest.Requests = new OrganizationRequestCollection();

                    foreach (var resource in resourcesToPublish.Where(k => k.error == false))
                    {
                        PublishAllXmlRequest publishResource = new PublishAllXmlRequest();
                        publishResource.RequestId = resource.id;
                        executemultiplerequest.Requests.Add(publishResource);
                    }

                    ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)this._CRMClient._Client.Execute(executemultiplerequest);

                    lst_Response = ParseResponseExecuteMultiple(executemultiplerequest, responseWithResults);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lst_Response;
        }

        private List<ExecuteMultipleResponseModel> ParseResponseExecuteMultiple (ExecuteMultipleRequest executemultiplerequest, ExecuteMultipleResponse responseWithResults)
        {
            List<ExecuteMultipleResponseModel> lst_Response = new List<ExecuteMultipleResponseModel>();

            try
            {
                foreach (var responseItem in responseWithResults.Responses)
                {
                    // A valid response.
                    if (responseItem.Response != null)
                    {
                        ExecuteMultipleResponseModel resourceResponse = new ExecuteMultipleResponseModel();
                        resourceResponse.id = GetIdTypeResponse(executemultiplerequest, responseItem);
                        resourceResponse.name = executemultiplerequest.Requests[responseItem.RequestIndex].RequestName;
                        resourceResponse.description = responseItem.Response.ResponseName;
                        resourceResponse.error = false;
                        lst_Response.Add(resourceResponse);

                        // An error has occurred.
                    } else if (responseItem.Fault != null) {
                        ExecuteMultipleResponseModel resourceResponse = new ExecuteMultipleResponseModel();
                        resourceResponse.id = GetIdTypeResponse(executemultiplerequest, responseItem);
                        resourceResponse.name = executemultiplerequest.Requests[responseItem.RequestIndex].RequestName;
                        resourceResponse.description = "Error code : " + responseItem.Fault.ErrorCode + " - Error message : " + responseItem.Fault.Message + " - Error details : " + responseItem.Fault.ErrorDetails;
                        resourceResponse.error = true;
                        lst_Response.Add(resourceResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lst_Response;
        }

        private Guid GetIdTypeResponse(ExecuteMultipleRequest requests, ExecuteMultipleResponseItem item)
        {
            Guid id = Guid.Empty;
            var typeOperation = requests.Requests[item.RequestIndex];
            switch (typeOperation.GetType().Name)
            {
                case "UpdateRequest":
                    id = ((UpdateRequest)requests.Requests[item.RequestIndex]).Target.Id;
                    break;
                case "PublishAllXmlRequest":
                    id = ((PublishAllXmlRequest)requests.Requests[item.RequestIndex]).RequestId.Value;
                    break;
                default:
                    break;
            }
            return id;
        }

        /// <summary>
        /// Mapper model resource to resource entity
        /// </summary>
        /// <param name="recurso"></param>
        /// <returns></returns>
        private dynamic MapperToEntity(ResourceModel recurso)
        {
            Entity entity = new Entity(EntityNames.Resources, recurso.resourceid);
            entity["name"] = recurso.name;
            entity["createdon"] = recurso.localcreatedon;
            entity["modifiedon"] = recurso.localmodifiedon;
            entity["content"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(recurso.contentLocal));
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
            model.webresourcetype = EntityCollectionExtension.ExistProperty(recurso, "webresourcetype") ? ((OptionSetValue)recurso.Attributes["webresourcetype"]).Value : 0;
            model.contentCRM = EntityCollectionExtension.ExistProperty(recurso, "content") ? recurso.Attributes["content"].ToString() : "";

            if (!string.IsNullOrEmpty(model.contentCRM))
            {
                model.contentCRM = Encoding.UTF8.GetString(Convert.FromBase64String(model.contentCRM));
                GetResourceLocal(model);
            }          

            return model;
        }

        public void GetResourceLocal (ResourceModel resource)
        {
            string pathResourceLocal = GetPathResourceLocal(resource);
            if (pathResourceLocal != null)
            {
                resource.pathlocal = pathResourceLocal;
                resource.contentLocal = File.ReadAllText(pathResourceLocal, Encoding.UTF8);
                FileInfo fi = new FileInfo(pathResourceLocal);
                resource.localcreatedon = fi.CreationTime.ToString();
                resource.localmodifiedon = fi.LastWriteTimeUtc.ToString();
                resource.resourceCompareStatus = SideBySideDiffBuilder.Diff(resource.contentCRM, resource.contentLocal);
                resource.resourceDifference = (resource.resourceCompareStatus.NewText.HasDifferences || resource.resourceCompareStatus.OldText.HasDifferences) ? ResourceContentStatus.DifferencesExist : ResourceContentStatus.Equal;
            }
            else
            {
                resource.resourceDifference = ResourceContentStatus.LocalResourceMissing;
            }
        }

        private string GetPathResourceLocal (ResourceModel resource)
        {
            PathAndNameResourceModel resourceModel = Utils.getFormatPathAndNameResource(this._Settings, resource.name, resource.webresourcetype);

            try
            {                
                if (!string.IsNullOrEmpty(resourceModel.name) && !string.IsNullOrEmpty(resourceModel.path))
                {
                    if (File.Exists(resourceModel.path + resourceModel.name) && Utils.DirectoryHasPermission(resourceModel.path, FileSystemRights.Read))
                    {
                        resourceModel.path = resourceModel.path + resourceModel.name;
                    }
                    else
                    {
                        resourceModel.path = null;
                    }
                }
                else
                {
                    resourceModel.path = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return resourceModel.path;
        }
    }
}
