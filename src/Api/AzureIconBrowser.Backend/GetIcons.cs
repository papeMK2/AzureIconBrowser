using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using AzureIconBrowser.Backend.Models;
using System.Linq;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AzureIconBrowser.Backend.Options;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(AzureIconBrowser.Backend.Startup))]

namespace AzureIconBrowser.Backend
{
    public class GetIcons
    {
        private readonly IconStorageOptions _options;
        public GetIcons(IConfiguration configuration)
        {
            _options = new IconStorageOptions
            {
                BaseUrl = configuration["BaseURl"],
                ContainerName = configuration["ContainerName"],
                StorageConnectionString = configuration["StorageConnectionString"]
            };
        }

        [FunctionName("GetIcons")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient(_options.StorageConnectionString, _options.ContainerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            var resultSegment = blobContainerClient.GetBlobsByHierarchyAsync().AsPages();
            var icons = new List<Icon>();
            await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
            {
                foreach (BlobHierarchyItem blobhierarchyItem in blobPage.Values)
                {
                    var blob = blobhierarchyItem.Blob.Name.Split('/');
                    var icon = new Icon
                    {
                        Prefix = blob[0],
                        Name = blob[1],
                        Url = $"{_options.BaseUrl.TrimEnd('/')}/{_options.ContainerName}/{blob[0]}/{blob[1]}"
                    };

                    icons.Add(icon);
                }
            }

            return new OkObjectResult(icons.GroupBy(_ => _.Prefix));
        }
    }
}
