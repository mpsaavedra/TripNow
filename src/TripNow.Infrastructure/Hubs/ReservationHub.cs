using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using TripNow.Domain.Entities;
using TripNow.Domain.Enums;

namespace TripNow.Infrastructure.Hubs;

public class ReservationHub : Hub
{
    public async Task SendReservationUpdate(Reservation reservation)
    {
        await Clients.All.SendAsync("ReceiveReservationUpdate", reservation);
    }

    public async Task NotifyStatusChange(Guid id, ReservationStatus oldStatus, ReservationStatus newStatus)
    {
        await Clients.All.SendAsync("ReservationStatusChange", new 
        {
            Id = id,
            OldStatus = oldStatus.ToString(),
            NewStatus = newStatus.ToString(),
            Timestamp = DateTime.UtcNow
        });
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", "Successfully connected to ReservationHub");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Caller.SendAsync("Disconnected", "Disconnected from ReservationHub");
        await base.OnDisconnectedAsync(exception);
    }
}
