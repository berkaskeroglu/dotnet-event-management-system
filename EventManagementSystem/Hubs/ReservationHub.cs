using Microsoft.AspNetCore.SignalR;


namespace EventManagementSystem.Hubs
{
    public class ReservationHub : Hub
    {
        public async Task SendSeatUpdate(int seatId, bool isReserved)
        {
            await Clients.All.SendAsync("ReceiveSeatUpdate", seatId, isReserved);
        }

        public async Task SendPriceUpdate(decimal newPrice)
        {
            await Clients.All.SendAsync("ReceivePriceUpdate", newPrice);
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Disconnected: " + Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
