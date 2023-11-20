using Jogin.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Jogin.Hubs
{
    public class JoguinHub : Hub
    {
        private static List<JogadoresModel> Jogadores { get; set; }

        public JoguinHub() 
        {
            if(Jogadores == null)
            {
                Jogadores = new List<JogadoresModel>();
            }
            
        }
        

        public override async Task<Task> OnConnectedAsync()
        {
            await MyID();

            await SendMessage();
            
            return base.OnConnectedAsync();
        }

        public async Task MyID()
        {
            string clientId = Context.ConnectionId ?? "";

            Jogadores.Add(new JogadoresModel() { Id = Jogadores.Count, Name = clientId });

            await Clients.Client(clientId).SendAsync("MyID", clientId);
        }
        public async Task SendMessage()
        {
            await Clients.All.SendAsync("ReceiveMessage", JsonConvert.SerializeObject(Jogadores));
        }

        public async Task PlayerPos(string id, int posX, int posY)
        {
            int index = Jogadores.FindIndex(x => x.Name == id);
            Jogadores[index].posY = posY;
            Jogadores[index].posX = posX;

            await EnemiesPosition(id);
        }

        public async Task EnemiesPosition(string player)
        {
            List<string> clients = Jogadores.Where(x => x.Name != player).Select(x => x.Name).ToList();

            await Clients.Clients(clients).SendAsync("EnemyPos", JsonConvert.SerializeObject(Jogadores));
        }

        public async Task EliminatePlayer(string id, string enemyId)
        {
            Jogadores.RemoveAll(x => x.Name.Equals(enemyId));

            int index = Jogadores.FindIndex(x => x.Name == id);

            Jogadores[index].size += 5;

            await EnemiesPosition(id);

            await ILost(enemyId);
        }

        public async Task ILost(string id)
        {
            await Clients.Client(id).SendAsync("ILost", "Perdceui");
        }
    }
}
