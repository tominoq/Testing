namespace UI.Template.Framework.Exceptions;

/// <summary>
/// Represents an exception that is thrown when count doesn't meet some criteria.
/// E.g. list expects 5 items at least but it's empty.
/// </summary>
public class UnexpectedCountException : Exception
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public UnexpectedCountException()
    {
    }

    /// <summary>
    /// Constructor with message.
    /// </summary>
    /// <param name="message">Message to be printed in exception.</param>
    public UnexpectedCountException(string message) : base(message)
    {
    }

    /// <summary>
    /// Constructor with message.
    /// </summary>
    /// <param name="message">Message to be printed in exception.</param>
    /// <param name="innerException">Exception from <see cref="Exception.InnerException"/></param>
    public UnexpectedCountException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
