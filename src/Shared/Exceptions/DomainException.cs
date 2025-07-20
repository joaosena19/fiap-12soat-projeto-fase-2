using System.Net;

namespace Shared.Exceptions
{
    /// <summary>
    /// Exception customizada que carrega informações sobre o status HTTP a ser retornado
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Status code HTTP que deve ser retornado
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Construtor com valores padrão
        /// </summary>
        /// <param name="message">Mensagem de erro (padrão: "Bad Request")</param>
        /// <param name="statusCode">Código de status HTTP (padrão: 400 - BadRequest)</param>
        public DomainException(string message = "Bad Request", HttpStatusCode statusCode = HttpStatusCode.BadRequest) 
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Construtor com exception interna
        /// </summary>
        /// <param name="message">Mensagem de erro</param>
        /// <param name="innerException">Exception interna</param>
        /// <param name="statusCode">Código de status HTTP (padrão: 400 - BadRequest)</param>
        public DomainException(string message, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.BadRequest) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
