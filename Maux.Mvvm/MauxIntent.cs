namespace Maux;

/// <summary>
/// Defines a navigation intent, to be used together with <see cref="MauxPageModel.PrepareAsync"/>
/// </summary>
[PublicAPI]
public static class MauxIntent
{
    /// <summary>
    /// The parameter name used to store the intent during shell navigation
    /// </summary>
    public const string ParameterName = "{Intent}";

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> with a message indicating the intent is invalid
    /// </summary>
    /// <param name="intent"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void ThrowInvalidIntent(object? intent) =>
        throw new InvalidOperationException($"Invalid intent {intent?.GetType().FullName}");
    
    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> with a message indicating the intent is invalid
    /// </summary>
    /// <param name="intent"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>
    /// Example usage:
    /// <code>
    /// public override Task PrepareAsync(object? intent = null)
    ///      => intent switch
    ///         {
    ///             MyIntent myIntent => DoSomethingAsync(myIntent),
    ///             _ => MauxIntent.ThrowInvalidIntentAsync(intent)
    ///         };
    /// </code>
    /// </remarks>
    [DoesNotReturn]
    public static Task ThrowInvalidIntentAsync(object? intent) =>
        throw new InvalidOperationException($"Invalid intent {intent?.GetType().FullName}");
}
