using Microsoft.AspNetCore.Mvc;

namespace Parqueadero.Controllers
{
    public class VehiculoController : Controller
    {
        private readonly VehiculoService _vehiculoService;

        public VehiculoController(VehiculoService vehiculoService)
        {
            _vehiculoService = vehiculoService;
        }

        [HttpGet]
        public IActionResult Ingreso()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ingreso(string placa)
        {
            try
            {
                _vehiculoService.RegistrarIngreso(placa);
                ViewData["Message"] = "Vehículo registrado correctamente.";
            }
            catch (Exception ex)
            {
                ViewData["Message"] = $"Error al registrar el ingreso: {ex.Message}";
            }
            return View();
        }

        [HttpGet]
        public IActionResult Salida()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Salida(string placa)
        {
            try
            {
                var mensaje = _vehiculoService.RegistrarSalida(placa);
                ViewData["Message"] = mensaje;
            }
            catch (Exception ex)
            {
                ViewData["Message"] = $"Error al registrar la salida: {ex.Message}";
            }
            return View();
        }
    }
}
