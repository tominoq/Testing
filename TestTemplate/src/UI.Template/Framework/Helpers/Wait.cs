using System.Globalization;

namespace UI.Template.Framework.Helpers;

/// <summary>
/// Contains Wait methods. It's based on selenium implementation with small changes (it doesn't need WebDriver instance).
/// </summary>
public class Wait
{
    private readonly List<Type> _ignoredExceptions = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Wait"/> class.
    /// </summary>
    /// <param name="timeout">How long (seconds) to wait for the condition to be true.</param>
    /// <param name="sleepInterval">How long (milliseconds) each iteration takes.</param>
    /// <param name="message">Message to be displayed when timeout is reached.</param>
    /// <param name="exceptionsToBeIgnored">Exceptions that will be ignored if any of these raise.</param>>
    public Wait(uint timeout = 60, int sleepInterval = -1, string message = "", params Type[] exceptionsToBeIgnored)
    {
        Timeout = TimeSpan.FromSeconds(timeout);

        if (sleepInterval > -1)
        {
            SleepInterval = TimeSpan.FromMilliseconds(sleepInterval);
        }

        Message = message;

        if (exceptionsToBeIgnored?.Length > 0)
        {
            SetIgnoreExceptionTypes(exceptionsToBeIgnored);
        }
    }

    /// <summary>
    /// Gets or sets how long to wait for the evaluated condition to be true. The default timeout is 500 milliseconds.
    /// </summary>
    public TimeSpan Timeout { get; set; }

    /// <summary>
    /// Gets or sets how often the condition should be evaluated. The default timeout is 500 milliseconds.
    /// </summary>
    public TimeSpan SleepInterval { get; set; } = DefaultSleepTimeout;

    /// <summary>
    /// Gets or sets the message to be displayed when time expires.
    /// </summary>
    public string Message { get; set; }

    private static TimeSpan DefaultSleepTimeout => TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Method sets the 'Message' property of current <see cref="Wait"/> object with custom value.
    /// </summary>
    /// <param name="message">Message to be displayed when timeout is reached.</param>
    /// <returns><see cref="Wait"/></returns>
    public Wait SetTimeoutMessage(string message)
    {
        Message = message;
        return this;
    }

    /// <summary>
    /// Configures this instance to ignore specific types of exceptions while waiting for a condition.
    /// Any exceptions not whitelisted will be allowed to propagate, terminating the wait.
    /// </summary>
    /// <param name="exceptionTypes">The types of exceptions to ignore.</param>
    /// <returns><see cref="Wait"/></returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="exceptionTypes"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any type in <paramref name="exceptionTypes"/> does not derive from <see cref="Exception"/>.</exception>
    public Wait SetIgnoreExceptionTypes(params Type[] exceptionTypes)
    {
        if (exceptionTypes == null)
        {
            throw new ArgumentNullException(nameof(exceptionTypes), "exceptionTypes cannot be null");
        }

        foreach (Type exceptionType in exceptionTypes)
        {
            if (!typeof(Exception).IsAssignableFrom(exceptionType))
            {
                throw new ArgumentException("All types to be ignored must derive from System.Exception", nameof(exceptionTypes));
            }
        }

        _ignoredExceptions.AddRange(exceptionTypes);
        return this;
    }

    /// <summary>
    /// Repeatedly applies this instance's input value to the given function until one of the following
    /// occurs:
    /// <para>
    /// <list type="bullet">
    /// <item>the function returns neither null nor false</item>
    /// <item>the function throws an exception that is not in the list of ignored exception types</item>
    /// <item>the timeout expires</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The delegate's expected return type.</typeparam>
    /// <param name="condition">A delegate taking an object of type T as its parameter, and returning a TResult.</param>
    /// <returns>The delegate's return value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="condition"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">The type of <paramref name="condition"/> is not a valid return type.</exception>
    public virtual TResult Until<TResult>(Func<TResult> condition)
    {
        if (condition == null)
        {
            throw new ArgumentNullException(nameof(condition), "Condition cannot be null.");
        }

        var resultType = typeof(TResult);
        if ((resultType.IsValueType && resultType != typeof(bool)) || !typeof(object).IsAssignableFrom(resultType))
        {
            throw new ArgumentException("Can only wait on an object or boolean response, tried to use type: " + resultType, nameof(condition));
        }

        Exception? lastException = null;
        var endTime = DateTime.Now.Add(Timeout);
        while (true)
        {
            try
            {
                var result = condition();
                if (resultType == typeof(bool))
                {
                    var boolResult = result as bool?;
                    if (boolResult == true)
                    {
                        return result;
                    }
                }
                else
                {
                    if (result is not null)
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex) when (IsIgnoredException(ex))
            {
                lastException = ex;
            }

            // Check the timeout after evaluating the function to ensure conditions
            // with a zero timeout can succeed.
            if (DateTime.Now > endTime)
            {
                string timeoutMessage = string.Format(CultureInfo.InvariantCulture, $"Timed out after {Timeout.TotalSeconds} seconds");
                if (!string.IsNullOrEmpty(Message))
                {
                    timeoutMessage += ": " + Message;
                }

                ThrowTimeoutException(timeoutMessage, lastException!);
            }

            Thread.Sleep(SleepInterval);
        }
    }

    /// <summary>
    /// Throws a <see cref="TimeoutException"/> with the given message.
    /// </summary>
    /// <param name="exceptionMessage">The message of the exception.</param>
    /// <param name="lastException">The last exception thrown by the condition.</param>
    /// <remarks>This method may be overridden to throw an exception that is
    /// idiomatic for a particular test infrastructure.</remarks>
    /// <exception cref="TimeoutException">Thrown when the wait times out.</exception>
    protected virtual void ThrowTimeoutException(string exceptionMessage, Exception lastException)
    {
        throw new TimeoutException(exceptionMessage, lastException);
    }

    private bool IsIgnoredException(Exception exception)
    {
        return _ignoredExceptions.Exists(type => type.IsAssignableFrom(exception.GetType()));
    }
}
