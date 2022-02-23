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
        public Layer Render()
        {
            return _renderService.Render();
        }

        [HttpPost]
        public Layer Update()
        {
            return _renderService.Update();
        }

        [HttpGet("components")]
        public IEnumerable<Component> GetComponents()
        {
            return _renderService.Components;
        }
    }
}