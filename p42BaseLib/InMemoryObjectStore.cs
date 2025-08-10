using System.Collections.Concurrent;
using p42ReportingLib.Models;

namespace p42BaseLib;

public class InMemoryObjectStore : BaseStore
{
    ConcurrentDictionary<string, ReportModel?> _reports = new ConcurrentDictionary<string, ReportModel?>();

    public override int TotalNumber(string? prefix = null)
    {
        return _reports.Count;       
    }

    public override async Task<ReportModel?> Get(string id,string? prefix = null)
    {
        if (string.IsNullOrEmpty(id)) return null;
        _reports.TryGetValue(id, out ReportModel? model);
        return model;
    }

    public override async Task<ReportModel?> Add(ReportModel model,string? prefix = null)
    {
        if (model == null) return null;
        model.Id = Guid.NewGuid().ToString();
        if (_reports.TryAdd(model.Id, model))
        {
            return model;
        }
        return null;
    }

    public override bool Delete(string id,string? prefix = null)
    {
        return _reports.TryRemove(id, out _);
    }

    public override bool Update(string id, ReportModel model,string? prefix = null)
    {
        if (string.IsNullOrEmpty(id) || model == null) return false;
        if (!_reports.ContainsKey(id)) return false;
        model.Id = id;
        _reports[id] = model;
        return true;
    }
}