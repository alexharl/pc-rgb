using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace PcRGB.Hubs
{
    public class CanvasHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("Client connected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("Client disconnected");
        }
    }
}