namespace AcmePedidosAPI.Models
{
    // ─── JSON REQUEST (entrada del cliente) ───────────────────────────────────

    public class EnviarPedidoWrapper
    {
        public EnviarPedidoRequest EnviarPedido { get; set; } = new();
    }

    public class EnviarPedidoRequest
    {
        public string NumPedido       { get; set; } = string.Empty;
        public string CantidadPedido  { get; set; } = string.Empty;
        public string CodigoEAN       { get; set; } = string.Empty;
        public string NombreProducto  { get; set; } = string.Empty;
        public string NumDocumento    { get; set; } = string.Empty;
        public string Direccion       { get; set; } = string.Empty;
    }

    // ─── JSON RESPONSE (salida al cliente) ────────────────────────────────────

    public class EnviarPedidoRespuestaWrapper
    {
        public EnviarPedidoRespuesta EnviarPedidoRespuesta { get; set; } = new();
    }

    public class EnviarPedidoRespuesta
    {
        public string CodigoEnvio { get; set; } = string.Empty;
        public string Estado      { get; set; } = string.Empty;
    }
}
