using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace UI.Template.Framework.Configurations;

/// <summary>
/// Represents errors that occur because of missing configuration values.
/// </summary>
public class ConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException" /> class.
    /// </summary>
    public ConfigurationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException" /> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ConfigurationException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException" /> class
    /// with a specified error message and a reference to the inner exception
    /// that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of this exception.</param>
    public ConfigurationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Throws an exception if <paramref name="parameter" /> is null.
    /// </summary>
    /// <param name="parameter">The parameter to validate.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <exception cref="ConfigurationException">Thrown when the <paramref name="parameter" /> is null.</exception>
    public static void ThrowIfNull([NotNull] object? parameter, [CallerArgumentExpression(nameof(parameter))] string? parameterName = null)
    {
        if (parameter is null)
        {
            throw new ConfigurationException($"The configuration value \"{parameterName}\" cannot be null.");
        }
    }

    /// <summary>
    /// Throws an exception if <paramref name="parameter" /> is null or an empty string.
    /// </summary>
    /// <param name="parameter">The string parameter to validate.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <exception cref="ConfigurationException">Thrown when the <paramref name="parameter" /> is null or an empty string.</exception>
    public static void ThrowIfNullOrEmpty([NotNull] string? parameter, [CallerArgumentExpression(nameof(parameter))] string? parameterName = null)
    {
        if (string.IsNullOrEmpty(parameter))
        {
            throw new ConfigurationException($"The configuration value \"{parameterName}\" cannot be null or an empty string.");
        }
    }

    /// <summary>
    /// Throws an exception if <paramref name="parameter" /> is null, an empty string or consists entirely of whitespace characters.
    /// </summary>
    /// <param name="parameter">The string parameter to validate.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <exception cref="ConfigurationException">Thrown when the <paramref name="parameter" /> is null, an empty string or consists entirely of whitespace characters.</exception>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? parameter, [CallerArgumentExpression(nameof(parameter))] string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(parameter))
        {
            throw new ConfigurationException($"The configuration value \"{parameterName}\" cannot be null, an empty string or consists entirely of whitespace characters.");
        }
    }
}
