using Microsoft.AspNetCore.Mvc;
using PcRGB.Services;
using PcRGB.Model.Render;

namespace PcRGB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CanvasController : ControllerBase
    {
        private readonly SerialService _serialService;
        private readonly RenderService _renderService;

        public CanvasController(SerialService serialService, RenderService renderService)
        {
            _serialService = serialService;
            _renderService = renderService;
        }

        [HttpGet]
        public Layer GetCanvas()
        {
            return _renderService.canvas.Render();
        }

        [HttpPost]
        public Layer UpdateCanvas()
        {
            _renderService.canvas.Update();
            return _renderService.canvas.Render();
        }
    }
}