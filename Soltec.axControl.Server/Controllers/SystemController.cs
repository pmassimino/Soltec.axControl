using Microsoft.AspNetCore.Mvc;

namespace Soltec.axControl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    // Endpoint para verificar que el servidor de Soltec responde
    public record HabilitarTagRequest(int IdSector, string TagRFID);
    public record RegistrarPesadaRequest(int IdSector,int idTransaccion, string Peso);
    public record ReqTagCP(string TAG, string CartaPorte);
    public record AsociarTagCPRequest(List<ReqTagCP> reqTagsJSON);
    public record PedidoRechazoRequest(int IdSector, int IdTransaccion, string Tag);
    
    [HttpPost("HabilitarTag")]
    public IActionResult HabilitarTag([FromBody] HabilitarTagRequest request)
    
    {
        return Ok(new 
        { 
            esCorrecto = true, 
            mensaje = "Operación realizada correctamente", 
            dato = ""
             });
    }
    [HttpPost("RegistrarPesada")]
    public IActionResult RegistrarPesada([FromBody] RegistrarPesadaRequest request)
    {
        return Ok(new 
        { 
            esCorrecto = true, 
            mensaje = "Operación realizada correctamente", 
            dato = ""
             });
    }
    [HttpPost("AsociarTagCP")]
    public IActionResult AsociarTagCP([FromBody] AsociarTagCPRequest request)
    {
        var axRespuesta = request.reqTagsJSON;

        return Ok(new
        {
            esCorrecto = true,
            mensaje = "Asociación de tag y carta porte con éxito",
            dato = axRespuesta
        });
    }
    [HttpPost("PedidoRechazo")]
    public IActionResult PedidoRechazo([FromBody] PedidoRechazoRequest request)
    {
        var axRespuesta = new
        {
            request.IdSector,
            request.IdTransaccion,
            request.Tag
        };

        return Ok(new
        {
            esCorrecto = true,
            mensaje = "Se rechazó el pedido",
            dato = axRespuesta
        });
    }
}