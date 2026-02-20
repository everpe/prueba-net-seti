using Microsoft.AspNetCore.Mvc;
using AcmePedidosAPI.Models;
using AcmePedidosAPI.Services;

namespace AcmePedidosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PedidosController : ControllerBase
    {
        private readonly ISoapProxyService _soapProxy;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(ISoapProxyService soapProxy,
                                 ILogger<PedidosController> logger)
        {
            _soapProxy = soapProxy;
            _logger    = logger;
        }

        /// <summary>
        /// Envía un pedido al sistema de despacho de ACME.
        /// Transforma JSON → SOAP → JSON internamente.
        /// </summary>
        /// <param name="wrapper">Datos del pedido</param>
        /// <returns>Código de envío y estado del pedido</returns>
        [HttpPost("enviar")]
        [ProducesResponseType(typeof(EnviarPedidoRespuestaWrapper), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 502)]
        public async Task<IActionResult> EnviarPedido([FromBody] EnviarPedidoWrapper wrapper)
        {
            if (wrapper?.EnviarPedido == null)
                return BadRequest(new ErrorResponse("El cuerpo de la solicitud es inválido."));

            var req = wrapper.EnviarPedido;

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(req.NumPedido))
                return BadRequest(new ErrorResponse("El campo numPedido es requerido."));

            if (string.IsNullOrWhiteSpace(req.NumDocumento))
                return BadRequest(new ErrorResponse("El campo numDocumento es requerido."));

            _logger.LogInformation(
                "Procesando pedido {NumPedido} para documento {NumDoc}",
                req.NumPedido, req.NumDocumento);

            try
            {
                var respuesta = await _soapProxy.EnviarPedidoAsync(req);

                return Ok(new EnviarPedidoRespuestaWrapper
                {
                    EnviarPedidoRespuesta = respuesta
                });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al comunicarse con el servicio SOAP.");
                return StatusCode(502, new ErrorResponse(
                    "Error al comunicarse con el servicio de envío. Intente más tarde."));
            }
        }

        /// <summary>
        /// Health check del servicio.
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health() =>
            Ok(new { status = "OK", timestamp = DateTime.UtcNow });
    }

    public record ErrorResponse(string Error);
}
