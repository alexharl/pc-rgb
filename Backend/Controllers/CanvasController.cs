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
        public Renderer GetCanvas()
        {
            return _renderService.Renderer;
        }

        [HttpPost]
        public Renderer Update()
        {
            _renderService.Renderer.Update();
            return _renderService.Renderer;
        }

        [HttpPost("render")]
        public Renderer ToggleAutoRender()
        {
            _ = _renderService.Renderer.Animate();
            return _renderService.Renderer;
        }

        [HttpPost("step")]
        public Renderer Step()
        {
            _renderService.Renderer.Next();
            return _renderService.Renderer;
        }

        [HttpGet("components")]
        public IEnumerable<Model.Render.Controller> GetComponents()
        {
            return _renderService.Components;
        }

        [HttpGet("layer/{id}/visible/{visible}")]
        public Renderer SetLayerVisiblility([FromRoute] string id, [FromRoute] int visible)
        {
            return _renderService.SetLayerVisiblility(id, visible == 1);
        }

        [HttpPost("layer/{id}/draw")]
        public Renderer SetPixel([FromRoute] string id, [FromQuery] int x, [FromQuery] int y)
        {
            return _renderService.SetPixel(id, x, y);
        }
    }
}