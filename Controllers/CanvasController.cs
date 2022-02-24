using Microsoft.AspNetCore.Mvc;
using PcRGB.Services;
using PcRGB.Model.Render;
using System.Collections.Generic;

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
            return _renderService.Renderer;
        }

        [HttpPost]
        public Layer Update()
        {
            _renderService.Renderer.Update();
            return _renderService.Renderer;
        }

        [HttpPost("render")]
        public Layer ToggleAutoRender()
        {
            _ = _renderService.Renderer.Animate();
            return _renderService.Renderer;
        }

        [HttpGet("components")]
        public IEnumerable<Component> GetComponents()
        {
            return _renderService.Components;
        }

        [HttpGet("layer/{id}/visible/{visible}")]
        public Layer SetLayerVisiblility([FromRoute] string id, [FromRoute] int visible)
        {
            return _renderService.SetLayerVisiblility(id, visible == 1);
        }
    }
}