using Microsoft.AspNetCore.Mvc;

namespace ApiClientes.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController(ILogger<HealthController> logger) : ControllerBase
{
    private readonly ILogger<HealthController> _logger = logger;

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Health check executado");
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}