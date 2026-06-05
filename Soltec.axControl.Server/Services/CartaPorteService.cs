
using System.Net.WebSockets;

public class CartaPorteService
{
    private readonly IConfiguration _configuration;
    private AfipWebService.WSCPEService ws = new AfipWebService.WSCPEService();
 
    public CartaPorteService(IConfiguration configuration)
    {
        _configuration = configuration;        
        ws.UrlLogin = _configuration["AFIPServices:UrlLogin"];
        ws.UrlServicio = _configuration["AFIPServices:UrlWSCEService"];
        ws.DNDestino = _configuration["AFIPServices:DnDestino"];
        AfipWebService.WebService.EmpresaInfo empresa = new AfipWebService.WebService.EmpresaInfo();
        empresa.Cuit = long.Parse(_configuration["AFIPServices:Cuit"]);        
        empresa.PathCertificado = _configuration["AFIPServices:PathCertificado"];
        ws.Empresa = empresa;
    }
    
    // Aquí puedes agregar métodos para interactuar con el servicio de AFIP
}