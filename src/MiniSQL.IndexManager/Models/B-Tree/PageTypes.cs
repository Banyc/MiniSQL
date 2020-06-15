namespace MiniSQL.IndexManager.Models
{
    public enum PageTypes
    {
        InternalTablePage = 0x05,
        LeafTablePage = 0x0D,
        InternalIndexPage = 0x02,
        LeafIndexPage = 0x0A,
    }
}
