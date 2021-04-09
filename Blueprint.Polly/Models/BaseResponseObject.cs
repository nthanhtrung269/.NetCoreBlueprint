using System;

namespace Blueprint.Polly
{
    public class BaseResponseObject
    {
        /// <summary>
        /// Unique Identifier used by logging.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// The Status.
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// The ErrorCode.
        /// </summary>
        public ResponseErrorCode ErrorCode { get; set; }

        /// <summary>
        /// The Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The StackTrace.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// The Data.
        /// </summary>
        public object Data { get; set; }
    }
}
