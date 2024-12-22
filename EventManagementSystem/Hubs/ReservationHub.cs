using Microsoft.AspNetCore.SignalR;

namespace EventManagementSystem.Hubs
{
    public class ReservationHub : Hub
    {
        // Method to send seat update notifications to all connected clients
        public async Task SendSeatUpdate(int seatId, bool isReserved)
        {
            await Clients.All.SendAsync("ReceiveSeatUpdate", seatId, isReserved);
        }

        // Method to send price update notifications to all connected clients
        public async Task SendPriceUpdate(decimal newPrice)
        {
            await Clients.All.SendAsync("ReceivePriceUpdate", newPrice);
        }

        // Method called when a client connects to the hub
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        // Method called when a client disconnects from the hub
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Disconnected: " + Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}

