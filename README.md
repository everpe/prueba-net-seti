# ACME Pedidos API

API REST en ASP.NET 8 que actúa como **gateway** entre el cliente y el servicio SOAP de envío de pedidos de ACME.

## Arquitectura

```
Cliente (JSON)
     │
     ▼
┌─────────────────────────┐
│   PedidosController     │  ← Recibe JSON, valida, responde JSON
│   POST /api/pedidos/    │
│         enviar          │
└──────────┬──────────────┘
           │
           ▼
┌─────────────────────────┐
│   SoapProxyService      │  ← Transforma JSON ↔ XML (SOAP)
└──────────┬──────────────┘
           │  HTTP POST (text/xml)
           ▼
┌─────────────────────────┐
│  Beeceptor (SOAP Mock)  │
│  smb2b095807450.free    │
│  .beeceptor.com         │
└─────────────────────────┘
```

## Mapeo de campos

| JSON (entrada) | SOAP XML (salida) |
|---|---|
| numPedido | pedido |
| cantidadPedido | Cantidad |
| codigoEAN | EAN |
| nombreProducto | Producto |
| numDocumento | Cedula |
| direccion | Direccion |

| SOAP XML (respuesta) | JSON (respuesta) |
|---|---|
| Codigo | codigoEnvio |
| Mensaje | estado |

## Ejecutar con Docker (recomendado)

```bash
# Clonar el repositorio
git clone <repo-url>
cd AcmePedidosAPI

# Levantar con docker-compose
docker-compose up --build

# La API queda disponible en:
# http://localhost:8081  → Swagger UI
```

## Ejecutar localmente

```bash
# Requisito: .NET 8 SDK instalado
dotnet restore
dotnet run

# Disponible en: http://localhost:5000
```

## Probar el endpoint

### Request
```bash
curl -X POST http://localhost:8080/api/pedidos/enviar \
  -H "Content-Type: application/json" \
  -d '{
    "enviarPedido": {
      "numPedido": "75630275",
      "cantidadPedido": "1",
      "codigoEAN": "00110000765191002104587",
      "nombreProducto": "Armario INVAL",
      "numDocumento": "1113987400",
      "direccion": "CR 72B 45 12 APT 301"
    }
  }'
```

### Response esperado
```json
{
  "enviarPedidoRespuesta": {
    "codigoEnvio": "80375472",
    "estado": "Entregado exitosamente al cliente"
  }
}
```

## Endpoints disponibles

| Método | Ruta | Descripción |
|---|---|---|
| POST | /api/pedidos/enviar | Enviar pedido (JSON → SOAP → JSON) |
| GET | /api/pedidos/health | Health check |
| GET | / | Swagger UI |

## Estructura del proyecto

```
AcmePedidosAPI/
├── Controllers/
│   └── PedidosController.cs   ← Endpoint REST
├── Models/
│   └── PedidoModels.cs        ← DTOs Request/Response
├── Services/
│   └── SoapProxyService.cs    ← Lógica JSON↔XML y llamada SOAP
├── Program.cs                 ← Configuración DI y pipeline
├── appsettings.json           ← Configuración (endpoint SOAP)
├── Dockerfile                 ← Multi-stage build
└── docker-compose.yml         ← Orquestación
```

## Configuración

El endpoint SOAP se puede cambiar en `appsettings.json` o como variable de entorno Docker:

```json
{
  "SoapEndpoint": "https://smb2b095807450.free.beeceptor.com"
}
```

```bash
# Variable de entorno
docker run -e SoapEndpoint=https://otro-endpoint.com acme-pedidos-api
```
