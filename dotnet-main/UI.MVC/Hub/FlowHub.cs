using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using PIP.BL.IManagers;

namespace UI.MVC.Hub
{
    public class FlowHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IFlowSessionManager _flowSessionManager;
        private static readonly ConcurrentDictionary<int, int> ReadyCounts = new ConcurrentDictionary<int, int>();
        private static readonly ConcurrentDictionary<int, string> SessionHosts = new ConcurrentDictionary<int, string>();

        public FlowHub(IFlowSessionManager flowSessionManager)
        {
            _flowSessionManager = flowSessionManager;
        }

        public async Task JoinGroup(int flowSessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, flowSessionId.ToString());
            Console.WriteLine($"Added {Context.ConnectionId} to group {flowSessionId}");
        }

        public async Task SetHost(int flowSessionId)
        {
            // Logic to set the host
            SessionHosts[flowSessionId] = Context.ConnectionId;
            await Clients.Caller.SendAsync("HostSet", true); // Confirm host set successfully
            Console.WriteLine($"Host set for session {flowSessionId} by {Context.ConnectionId}");
        }


        // Called by each client when they are ready
        public async Task ReadyToBegin(int flowSessionId)
        {
            var flowSession = _flowSessionManager.GetFlowSession(flowSessionId);
            int newCount = ReadyCounts.AddOrUpdate(flowSessionId, 1, (_, count) => count + 1);
            Console.WriteLine($"User {Context.ConnectionId} ready in session {flowSessionId}, count: {newCount}");

            if (newCount >= flowSession.ExpectedUsers)
            {
                // Check if the connection ID matches the host set for this session
                if (SessionHosts.TryGetValue(flowSessionId, out var hostConnectionId))
                {
                    // Notify only the host to enable the 'Move to Next Step' button
                    await Clients.Client(hostConnectionId).SendAsync("EnableNextStep");
                    Console.WriteLine($"All users ready. Notified host {hostConnectionId} to enable moving to the next step for group {flowSessionId}");
                }
                ReadyCounts.AddOrUpdate(flowSessionId, 0, (_, _) => 0);
            }
        }
        public async Task SendCurrentStep(int flowSessionId)
        {
            if (SessionHosts.TryGetValue(flowSessionId, out var hostConnectionId) && Context.ConnectionId == hostConnectionId)
            {
                await Clients.Group(flowSessionId.ToString()).SendAsync("ShowCurrentStep", flowSessionId.ToString());
                Console.WriteLine($"Host {Context.ConnectionId} sent the current step to group {flowSessionId}");
            }
            else
            {
                Console.WriteLine($"Invalid attempt: {Context.ConnectionId} tried to send the current step for session {flowSessionId}");
            }
        }

        public async Task SendThankYou(int flowSessionId)
        {
            if (SessionHosts.TryGetValue(flowSessionId, out var hostConnectionId) && Context.ConnectionId == hostConnectionId)
            {
                await Clients.Group(flowSessionId.ToString()).SendAsync("SendThankYou", flowSessionId.ToString());
                Console.WriteLine($"Host {Context.ConnectionId} sent the thank you screen to group {flowSessionId}");
            }
            else
            {
                Console.WriteLine($"Invalid attempt: {Context.ConnectionId} tried to send the thank you for session {flowSessionId}");
            }
        }

        // This should be invoked by the host only
        public async Task MoveToNextStep(int flowSessionId)
        {
            if (SessionHosts.TryGetValue(flowSessionId, out var hostConnectionId) && Context.ConnectionId == hostConnectionId)
            {
                await Clients.Group(flowSessionId.ToString()).SendAsync("MoveToNextStep", flowSessionId.ToString());
                Console.WriteLine($"Host {Context.ConnectionId} triggered MoveToNextStep to group {flowSessionId}");
            }
            else
            {
                Console.WriteLine($"Invalid host attempt: {Context.ConnectionId} tried to move to next step for session {flowSessionId}");
            }
        }
        
        public async Task ClientReady(int flowSessionId)
        {
            var flowSession = _flowSessionManager.GetFlowSession(flowSessionId);
            int newCount = ReadyCounts.AddOrUpdate(flowSessionId, 1, (_, count) => count + 1);
            Console.WriteLine($"Client {Context.ConnectionId} signaled readiness in session {flowSessionId}, total ready: {newCount}");

            if (newCount >= flowSession.ExpectedUsers)
            {
                if (SessionHosts.TryGetValue(flowSessionId, out var hostConnectionId))
                {
                    await Clients.Client(hostConnectionId).SendAsync("AllClientsReady");
                    Console.WriteLine($"All clients are ready. Notified host {hostConnectionId} to enable moving to the next step for group {flowSessionId}");
                    
                }
                ReadyCounts.AddOrUpdate(flowSessionId, 0, (_, _) => 0);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Handle potential cleanup and notify other clients if necessary
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"Client disconnected: {Context.ConnectionId} with exception {exception?.Message}");
        }

        public async Task ResetTimer(int flowSessionId)
        {
     
            await Clients.Group(flowSessionId.ToString()).SendAsync("ResetTimer");
            Console.WriteLine($"ResetTimer message sent to all clients in group {flowSessionId}");
        }
        public async Task PauseTimer(int flowSessionId)
        {
            await Clients.Group(flowSessionId.ToString()).SendAsync("PauseTimer");
            Console.WriteLine($"PauseTimer message sent to all clients in group {flowSessionId}");
        }
        public async Task ResumeTimer(int flowSessionId)
        {
            await Clients.Group(flowSessionId.ToString()).SendAsync("ResumeTimer");
            Console.WriteLine($"ResumeTimer message sent to all clients in group {flowSessionId}");
        }
        
        public async Task RemoveQrCode(int flowSessionId)
        {
            await Clients.Group(flowSessionId.ToString()).SendAsync("RemoveQrCode");
            Console.WriteLine($"RemoveQrCode message sent to all clients in group {flowSessionId}");
        }
        public async Task ShowQrCode(int flowSessionId)
        {
            await Clients.Group(flowSessionId.ToString()).SendAsync("ShowQrCode");
            Console.WriteLine($"ShowQrCode message sent to all clients in group {flowSessionId}");
        }
    }
}
