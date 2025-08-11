using p42BaseLib;
using p42ReportingAPI.Reports;

namespace p42BaseLib;

public class BaseStore : IObjectModelStore
{
    public Dictionary<string, object> Properties { get; }

    protected static string GetPath(string name, string ext = "", string? prefix = null)
    {
        string path = !String.IsNullOrWhiteSpace(prefix) ? $"{prefix}" : "";
        string extension = !String.IsNullOrWhiteSpace(ext) ? $".{ext}" : "";
        if (!String.IsNullOrWhiteSpace(path) && !path.EndsWith("/"))
            path += "/";
        return $"{path}{name}{extension}";
    }

    public virtual int NumberOfObject(string? prefix = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task<T?> Get<T>(string name, string? prefix = null) where T : class
    {
        throw new NotImplementedException();
    }

    public virtual Task<List<T>> GetAll<T>(string name = "", string? prefix = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task<T?> Add<T>(T? model, string name, string? prefix = null) where T : class
    {
        throw new NotImplementedException();
    }

    public virtual bool Delete(string name, string? prefix = null)
    {
        throw new NotImplementedException();
    }

    public virtual bool Update<T>(string name, T model, string? prefix = null)
    {
        throw new NotImplementedException();
    }
}