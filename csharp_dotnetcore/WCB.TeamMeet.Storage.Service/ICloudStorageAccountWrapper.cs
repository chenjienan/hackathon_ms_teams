using Microsoft.WindowsAzure.Storage.Table;

namespace WCB.TeamMeet.Storage.Service
{
    public interface ICloudStorageAccountWrapper
    {
        CloudTableClient GetTableClient { get; }
    }
}