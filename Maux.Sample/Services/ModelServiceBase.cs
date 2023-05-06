using System.Collections.Immutable;
using Maux.Sample.Models;

namespace Maux.Sample.Services;

public interface IModelService<TModel> where TModel : ModelBase
{
    Task<TModel> GetAsync(string id);
    Task<IReadOnlyList<TModel>> QueryAsync(Func<TModel, bool>? predicate = null);
    Task UpsertAsync(TModel model);
}

public abstract class ModelServiceBase<TModel> : IModelService<TModel> where TModel : ModelBase
{
    protected Dictionary<string, TModel> Data { get; } = new();

    public async Task<TModel> GetAsync(string id)
    {
        await Task.Yield(); // simulate async processing
        return Data[id];
    }

    public async Task<IReadOnlyList<TModel>> QueryAsync(Func<TModel, bool>? predicate = null)
    {
        await Task.Yield(); // simulate async processing
        IEnumerable<TModel> results = Data.Values;
        if (predicate != null)
        {
            results = results.Where(predicate);
        }
        return results.ToImmutableList();
    }

    public async Task UpsertAsync(TModel model)
    {
        await Task.Yield(); // simulate async processing
        Data[model.Id] = model;
    }
}