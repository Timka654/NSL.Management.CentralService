namespace NSL.Management.CentralService.Utils.Sync.Data
{
    public class SyncIdentityKeyData
    {
        public Guid ServerId { get; set; }
        
        public string Token { get; set; }

        public DateTime LastGet { get; set; } = DateTime.UtcNow;
    }
}
