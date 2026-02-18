namespace UI.Template.Framework.Exceptions;

/// <summary>
/// Represents an exception that is thrown when specific value cannot be found.
/// </summary>
public class NoSuchValueException : Exception
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public NoSuchValueException()
    {
    }

    /// <summary>
    /// Constructor with message.
    /// </summary>
    /// <param name="message">Message to be printed in exception.</param>
    public NoSuchValueException(string message) : base(message)
    {
    }

    /// <summary>
    /// Constructor with message.
    /// </summary>
    /// <param name="message">Message to be printed in exception.</param>
    /// <param name="innerException">Exception from <see cref="Exception.InnerException"/></param>
    public NoSuchValueException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
