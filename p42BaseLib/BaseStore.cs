using p42BaseLib;
using p42ReportingAPI.Reports;

namespace p42BaseLib;

public class BaseStore : IObjectModelStore
{
    public Dictionary<string, object> Properties { get; }

    protected string getPath(string name, string ext = "", string? prefix = null)
    {
        string path = !string.IsNullOrWhiteSpace(prefix) ?$"{prefix}/":"";
        string extention = !string.IsNullOrWhiteSpace(ext) ? $".{ext}" : "";
        
        return $"{path}{name}{extention}";
    }
    public virtual int TotalNumber(string? prefix = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task<T?> Get<T>(string id,string? prefix = null) where T : class
    {
        throw new NotImplementedException();
    }

    public virtual Task<List<T>> GetAll<T>(string id, string? prefix = null) where T : class
    {
        throw new NotImplementedException();
    }

    public virtual Task<T?> Add<T>(T model,string? prefix = null) where T : class
    {
        throw new NotImplementedException();
    }

    public virtual bool Delete(string id,string? prefix = null)
    {
        throw new NotImplementedException();
    }

    public virtual bool Update<T>(string id, T model,string? prefix = null)
    {
        throw new NotImplementedException();
    }
}