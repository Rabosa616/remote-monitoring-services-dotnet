// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.IotHubManager.Services;
using Microsoft.Azure.IoTSolutions.IotHubManager.Services.Exceptions;
using Microsoft.Azure.IoTSolutions.IotHubManager.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.IotHubManager.WebService.v1.Models;

namespace Microsoft.Azure.IoTSolutions.IotHubManager.WebService.v1.Controllers
{
    [Route(Version.PATH + "/[controller]"), ExceptionsFilter]
    public class DeploymentsController : Controller
    {
        private readonly IDeployments deployments;

        public DeploymentsController(IDeployments deployments)
        {
            this.deployments = deployments;
        }

        /// <summary>Create a deployment targeting a particular group</summary>
        /// <param name="deployment">Deployment information</param>
        /// <returns>Deployment information and initial success metrics</returns>
        [HttpPost]
        public async Task<DeploymentApiModel> PostAsync([FromBody] DeploymentApiModel deployment)
        {
            if (string.IsNullOrWhiteSpace(deployment.DeviceGroupId))
            {
                throw new InvalidInputException("DeviceGroupId must be provided");
            }

            if (string.IsNullOrWhiteSpace(deployment.Name))
            {
                throw new InvalidInputException("Name must be provided");
            }

            if (string.IsNullOrWhiteSpace(deployment.PackageId))
            {
                throw new InvalidInputException("PackageId must be provided");
            }

            if (deployment.Priority < 0)
            {
                throw new InvalidInputException($"Invalid priority provided of {deployment.Priority}. " +
                                                "It must be non-negative");
            }

            return new DeploymentApiModel(await this.deployments.CreateAsync(deployment.ToServiceModel()));
        }

        [HttpGet]
        public async Task<DeploymentListApiModel> GetAsync()
        {
            return new DeploymentListApiModel(await this.deployments.ListAsync());
        }

        /// <summary>Get one deployment</summary>
        /// <param name="id">Deployment id</param>
        /// <returns>Deployment information with metrics</returns>
        [HttpGet("{id}")]
        public async Task<DeploymentApiModel> GetAsync(string id)
        {
            return new DeploymentApiModel(await this.deployments.GetAsync(id));
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            await this.deployments.DeleteAsync(id);
        }
    }
}
