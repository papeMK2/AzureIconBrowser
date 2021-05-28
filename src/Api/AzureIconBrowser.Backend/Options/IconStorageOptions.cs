using System;
using System.Collections.Generic;
using System.Text;

namespace AzureIconBrowser.Backend.Options
{
    public class IconStorageOptions
    {
        public string StorageConnectionString { get; set; }
        public string BaseUrl { get; set; }
        public string ContainerName { get; set; }
    }
}
