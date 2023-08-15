using CommunityToolkit.Mvvm.ComponentModel;

namespace Maux;

/// <summary>
/// Defines a view model for a page
/// </summary>
[PublicAPI]
public interface IMauxPageModel
{
    /// <summary>
    /// Prepares the view model with an optional intent provided to the page
    /// </summary>
    /// <param name="intent"></param>
    /// <returns></returns>
    Task OnPrepareAsync(object? intent = null);
}

/// <summary>
/// Defines a base implementation of a view model to be used in a page
/// </summary>
[PublicAPI]
public abstract class MauxPageModel : ObservableValidator, IMauxPageModel
{
    private SemaphoreSlim _initializeLock = new(1, 1);
    private bool _isPreparing = true;
    private bool _isInitializing = true;
    
    /// <summary>
    /// Determine if the page model is preparing the information to display
    /// </summary>
    public bool IsPreparing
    {
        get => _isPreparing;
        private set => SetProperty(_isPreparing, value, newValue => _isPreparing = newValue);
    }
    
    /// <summary>
    /// Determine if the page model is preparing the information to display for the first time
    /// </summary>
    public bool IsInitializing
    {
        get => _isInitializing;
        private set => SetProperty(_isInitializing, value, newValue => _isInitializing = newValue);
    }

    /// <summary>
    /// Prepares the view model with an optional intent and sets <see cref="IsPreparing"/> to true.
    /// </summary>
    /// <param name="intent"></param>
    /// <returns></returns>
    async Task IMauxPageModel.OnPrepareAsync(object? intent)
    {
        // If PrepareAsync doesn't contain real async code,
        // it'd be better to have the whole Prepare process synchronous
        // ReSharper disable once MethodHasAsyncOverload
        _initializeLock.Wait();
        
        IsPreparing = true;
        try
        {
            await PrepareAsync(intent);
        }
        finally
        {
            IsPreparing = false;
            IsInitializing = false;
            _initializeLock.Release();
        }
    }

    /// <summary>
    /// Prepares the view model with an optional intent provided to the page
    /// </summary>
    /// <param name="intent"></param>
    /// <returns></returns>
    public virtual Task PrepareAsync(object? intent = null) => Task.CompletedTask;
}