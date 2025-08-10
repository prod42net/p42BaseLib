using p42ReportingLib.Models;

namespace p42ReportingAPI.Reports;

public class BaseStore : IReportStore
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

    public virtual Task<ReportModel?> Get(string id,string? prefix = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task<List<ReportModel>> GetAll(string id, string? prefix = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task<ReportModel?> Add(ReportModel model,string? prefix = null)
    {
        throw new NotImplementedException();
    }

    public virtual bool Delete(string id,string? prefix = null)
    {
        throw new NotImplementedException();
    }

    public virtual bool Update(string id, ReportModel model,string? prefix = null)
    {
        throw new NotImplementedException();
    }
}