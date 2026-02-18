namespace UI.Template.Framework.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an unsupported operation is used.
/// </summary>
public class UnsupportedOperationException : Exception
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public UnsupportedOperationException()
    {
    }

    /// <summary>
    /// Constructor with message.
    /// </summary>
    /// <param name="message">Message to be printed in exception.</param>
    public UnsupportedOperationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Constructor with message.
    /// </summary>
    /// <param name="message">Message to be printed in exception.</param>
    /// <param name="innerException">Exception from <see cref="Exception.InnerException"/></param>
    public UnsupportedOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
