namespace Skyline.DataMiner.SDM.SatOps.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The exception that is thrown when an invalid input is provided to a script.
    /// </summary>
    [Serializable]
    public class InvalidScriptInputException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScriptInputException"/> class.
        /// </summary>
        public InvalidScriptInputException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScriptInputException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidScriptInputException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScriptInputException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidScriptInputException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidScriptInputException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected InvalidScriptInputException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
