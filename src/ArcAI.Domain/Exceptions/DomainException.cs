namespace ArcAI.Domain.Exceptions;

/// <summary>
/// Exception thrown when a domain rule or invariant is violated.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Gets the error code associated with this exception.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the DomainException class.
    /// </summary>
    public DomainException()
        : base() { }

    /// <summary>
    /// Initializes a new instance of the DomainException class with a message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public DomainException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the DomainException class with a message and error code.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="errorCode">The error code.</param>
    public DomainException(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the DomainException class with a message and inner exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the DomainException class with full details.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="innerException">The inner exception.</param>
    public DomainException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}