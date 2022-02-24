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
            return _renderService.Canvas;
        }

        [HttpPost]
        public Layer Update()
        {
            return _renderService.Update();
        }

        [HttpPost("render")]
        public bool ToggleAutoRender()
        {
            _ = _renderService.AutoRender();
            return _renderService.token.IsCancellationRequested;
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