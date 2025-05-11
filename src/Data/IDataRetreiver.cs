namespace ORBIT9000.Data
{
    internal interface IDataRetreiver
    {
        object FetchData(string query);
    }

    internal interface IDataProcessor
    {
        object ProcessData(object data);
    }

    internal interface IDataUpdater
    {
        bool UpdateData(object data);
    }
}
