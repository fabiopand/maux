namespace Maux;

/// <inheritdoc cref="IMauxApplication"/>
[PublicAPI]
public abstract class MauxApplication : Application, IMauxApplication
{
    private readonly WeakEventManager _eventManager = new();

    /// <inheritdoc cref="IMauxApplication.Resuming"/>
    event EventHandler IMauxApplication.Resuming
    {
        add => _eventManager.AddEventHandler(value);
        remove => _eventManager.RemoveEventHandler(value);
    }

    /// <inheritdoc cref="IMauxApplication.EnteringSleep"/>
    event EventHandler IMauxApplication.EnteringSleep
    {
        add => _eventManager.AddEventHandler(value);
        remove => _eventManager.RemoveEventHandler(value);
    }

    /// <inheritdoc cref="IMauxApplication.Services"/>
    public IServiceProvider Services => this.Handler!.MauiContext!.Services;

    /// <inheritdoc cref="Application.OnResume"/>
    protected override void OnResume()
    {
        base.OnResume();
        _eventManager.HandleEvent(this, EventArgs.Empty, nameof(IMauxApplication.Resuming));
    }

    /// <inheritdoc cref="Application.OnSleep"/>
    protected override void OnSleep()
    {
        base.OnSleep();
        _eventManager.HandleEvent(this, EventArgs.Empty, nameof(IMauxApplication.EnteringSleep));
    }
}