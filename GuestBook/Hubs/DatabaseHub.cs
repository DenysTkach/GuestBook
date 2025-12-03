using Microsoft.AspNetCore.SignalR;

namespace GuestBook.Hubs;

public class DatabaseHub : Hub
{
    // This hub is used to notify clients when the database file has changed.
    // Clients can subscribe to receive notifications and automatically refresh their pages.
}
