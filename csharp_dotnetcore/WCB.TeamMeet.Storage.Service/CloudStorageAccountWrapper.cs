using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WCB.TeamMeet.Storage.Service
{
    public class CloudStorageAccountWrapper : ICloudStorageAccountWrapper
    {
        private readonly CloudStorageAccount _cloudStorageAccount;

        public CloudStorageAccountWrapper()
        {
            _cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=teamsstroage;AccountKey=FubNw9wMxsha5cvzgPKEmWis5QX8qyFZRmal3yWMvMEYy6EBR+4nLxQXPys0G+W9W0jeGyM3wTSoOQ5qqUy2zQ==;EndpointSuffix=core.windows.net");
            //_cloudStorageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage = true");
        }

        public CloudTableClient GetTableClient => _cloudStorageAccount.CreateCloudTableClient();
    }
}