namespace Maux;

/// <summary>
/// Syntactic sugar class to provide parameters to shell navigation without the verbosity of
/// <code>new Dictionary&lt;string, object&gt;() { { "key1", "value1" } }</code>
/// </summary>
/// <example>
/// <code> new () { { "key1", value1 }, { "key2", value2 } } </code>
/// <code> new ("key1", value1, "key2", value2) </code>
/// </example>
[PublicAPI]
public sealed class ShellParameters : Dictionary<string, object>
{
    /// <summary>
    /// Syntactic sugar constructor to fluently provide parameters as list
    /// </summary>
    /// <example>
    /// <code>new ShellParameters("key1", value1, "key2", value2, "key3", value3)</code>
    /// </example>
    /// <param name="parameters"></param>
    public ShellParameters(params object[] parameters)
    {
        if (parameters.Length % 2 != 0)
        {
            throw new ArgumentException("Parameter list should be provided as key1,value1,key2,value2,...");
        }
        
        for (var i = 0; i < parameters.Length; i+=2)
        {
            if (parameters[i] is not string key)
            {
                throw new ArgumentException("Parameter name must be string", $"parameters[{i}]");
            }
            Add(key, parameters[i + 1]);
        }
    }
}