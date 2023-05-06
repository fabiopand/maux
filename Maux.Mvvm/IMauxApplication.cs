namespace Maux;

/// <summary>
/// Represents a cross-platform mobile application.
/// </summary>
/// <remarks>
/// Extend <see cref="Application"/> by providing access to <see cref="Application.OnResume"/> and <see cref="Application.OnSleep"/>
/// through <see cref="Resuming"/> and <see cref="EnteringSleep"/> events.
/// </remarks>
public interface IMauxApplication : IApplication
{
    /// <summary>
    /// Invoked when your app resumes
    /// </summary>
    /// <remarks>
    /// This event is managed by <see cref="WeakEventManager"/> so you can attach you handler without worrying about memory leaks
    /// </remarks>
    event EventHandler Resuming;

    /// <summary>
    /// Invoked when your app sleeps
    /// </summary>
    /// <remarks>
    /// This event is managed by <see cref="WeakEventManager"/> so you can attach you handler without worrying about memory leaks
    /// </remarks>
    event EventHandler EnteringSleep;
    
    /// <summary>
    /// Provides access to the application's service provider
    /// </summary>
    IServiceProvider Services { get; }
}