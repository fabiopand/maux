namespace Maux;

/// <summary>
/// Gives the same functionality as <see cref="MainThread"/> adding support for <see cref="DevicePlatform.Unknown"/> (found in unit tests)
/// </summary>
[PublicAPI]
public static class ExecutingThread
{
    /// <summary>Gets if it is the current main UI thread.</summary>
    /// <value>If main thread.</value>
    /// <remarks>On unknown platforms always returns true.</remarks>
    public static bool IsMainThread => DeviceInfo.Platform == DevicePlatform.Unknown || MainThread.IsMainThread;

    /// <param name="action">Action to execute.</param>
    /// <summary>Invokes an action on the main thread of the application.</summary>
    /// <remarks>On unknown platforms executes immediately.</remarks>
    public static void BeginInvokeOnMainThread(
        #nullable disable
        Action action)
    {
        if (DeviceInfo.Platform == DevicePlatform.Unknown)
        {
            action();
            return;
        }
        
        MainThread.BeginInvokeOnMainThread(action);
    }

    /// <param name="action">Action to invoke</param>
    /// <summary>Invoke the main thread async</summary>
    /// <returns>A task that can be awaited</returns>
    /// <remarks>On unknown platforms executes immediately.</remarks>
    public static Task InvokeOnMainThreadAsync(Action action)
    {
        if (DeviceInfo.Platform == DevicePlatform.Unknown)
        {
            action();
            return Task.CompletedTask;
        }
        
        return MainThread.InvokeOnMainThreadAsync(action);
    }

    /// <typeparam name="T">To be added.</typeparam>
    /// <param name="func">A function to execute</param>
    /// <summary>Invoke the main thread async</summary>
    /// <returns>A task that can be awaited</returns>
    /// <remarks>On unknown platforms executes immediately.</remarks>
    public static Task<T> InvokeOnMainThreadAsync<T>(Func<T> func)
    {
        if (DeviceInfo.Platform == DevicePlatform.Unknown)
        {
            return Task.FromResult(func());
        }
        
        return MainThread.InvokeOnMainThreadAsync(func);
    }

    /// <param name="funcTask">A function task to execute</param>
    /// <summary>Invoke the main thread async</summary>
    /// <returns>A task that can be awaited</returns>
    /// <remarks>On unknown platforms executes immediately.</remarks>
    public static Task InvokeOnMainThreadAsync(Func<Task> funcTask)
    {
        if (DeviceInfo.Platform == DevicePlatform.Unknown)
        {
            return funcTask();
        }
        
        return MainThread.InvokeOnMainThreadAsync(funcTask);
    }

    /// <typeparam name="T">To be added.</typeparam>
    /// <param name="funcTask">A function task to execute</param>
    /// <summary>Invoke the main thread async</summary>
    /// <returns>A task that can be awaited</returns>
    /// <remarks>On unknown platforms executes immediately.</remarks>
    public static Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask)
    {
        if (DeviceInfo.Platform == DevicePlatform.Unknown)
        {
            return funcTask();
        }
        
        return MainThread.InvokeOnMainThreadAsync(funcTask);
    }
    
    /// <summary>
    /// Gets the main thread synchronization context.
    /// </summary>
    /// <returns>The synchronization context for the main thread.</returns>
    /// <remarks>On unknown platforms uses the current context.</remarks>
    public static Task<SynchronizationContext> GetMainThreadSynchronizationContextAsync()
    {
        if (DeviceInfo.Platform == DevicePlatform.Unknown)
        {
            return Task.FromResult(SynchronizationContext.Current);
        }

        return MainThread.GetMainThreadSynchronizationContextAsync();
    }
}

