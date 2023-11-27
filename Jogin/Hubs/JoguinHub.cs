using Jogin.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Drawing;

namespace Jogin.Hubs
{
    public class JoguinHub : Hub
    {
        private static List<JogadoresModel>? Jogadores { get; set; }
        private static List<DotsModel>? DotsList { get; set; }
        public JoguinHub() 
        {
            if(Jogadores == null)
            {
                Jogadores = new List<JogadoresModel>();
            }

            if(DotsList == null)
            {
                DotsList = new List<DotsModel>();

            }

            if(DotsList.Count < 10)
            {
                for (int i = 0; i < 15; i++)
                {
                    DotsList.Add(new DotsModel());
                }
            }
            
        }
        
        public override async Task<Task> OnConnectedAsync()
        {
            await Dots();

            await MyID();

            await SendMessage();
            
            return base.OnConnectedAsync();
        }

        public async Task MyID()
        {
            string clientId = Context.ConnectionId ?? "";

            Jogadores.Add(new JogadoresModel() { Id = Jogadores.Count, ConnectionID = clientId, Name = Context.GetHttpContext().Session.GetString("username") });

            await Clients.Client(clientId).SendAsync("MyID", clientId);
        }
        public async Task SendMessage()
        {
            await Clients.All.SendAsync("ReceiveMessage", JsonConvert.SerializeObject(Jogadores));
        }

        public async Task PlayerPos(string id, int posX, int posY)
        {
            int index = Jogadores.FindIndex(x => x.ConnectionID == id);
            Jogadores[index].posY = posY;
            Jogadores[index].posX = posX;

            var playerRect = new Rectangle(
                Jogadores[index].posX,
                Jogadores[index].posY,
                Jogadores[index].size,
                Jogadores[index].size 
            );

            for(int i = 0; i < DotsList!.Count; i++)
            {
                var dotRect = new Rectangle(
                    DotsList[i].posX,
                    DotsList[i].posY,
                    DotsList[i].size,
                    DotsList[i].size
                );

                bool interSect = dotRect.IntersectsWith( playerRect );

                if(interSect)
                {
                    await EliminateDot(id, DotsList[i].Guid.ToString());
                    break;

                }
            }


            await EnemiesPosition(id);
        }

        public async Task EnemiesPosition(string player)
        {
            List<string> clients = Jogadores.Where(x => x.ConnectionID != player).Select(x => x.ConnectionID).ToList();

            await Clients.Clients(clients).SendAsync("EnemyPos", JsonConvert.SerializeObject(Jogadores));
        }

        public async Task EliminatePlayer(string id, string enemyId)
        {
            Jogadores!.RemoveAll(x => x.ConnectionID.Equals(enemyId));

            int index = Jogadores.FindIndex(x => x.ConnectionID == id);

            Jogadores[index].size += 5;

            await Clients.All.SendAsync("RemovePlayer", enemyId);

            await EnemiesPosition(id);

            await ILost(enemyId);

            //JogadoresModel? enemy = Jogadores?.FirstOrDefault(x => x.ConnectionID == enemyId);
            //JogadoresModel? player = Jogadores?.FirstOrDefault(x => x.ConnectionID == id);

            //int enemySize = enemy!.size;
            //int playerIndex = Jogadores.IndexOf(player);

            //Jogadores[playerIndex].size += enemySize;

            //Jogadores.RemoveAt(Jogadores.IndexOf(enemy));

            //await Clients.Client(id).SendAsync("IncreaseSize", Jogadores[playerIndex].size);
            //await Clients.All.SendAsync("RemovePlayer", enemyId);
        }

        public async Task ILost(string id)
        {
            await Clients.Client(id).SendAsync("ILost", "Perdceui");
        }

        public async Task Dots()
        {
            await Clients.All.SendAsync("DotsPosition", JsonConvert.SerializeObject(DotsList));
        }

        public async Task EliminateDot(string playerId, string dotId)
        {
            await Clients.All.SendAsync("RemoveDot", dotId);

            DotsList!.RemoveAll(x => x.Guid.ToString() == dotId);

            int index = Jogadores!.FindIndex(x => x.ConnectionID == playerId);

            Jogadores[index].size += 5;

            if (DotsList.Count < 10)
            {
                DotsModel dt = new DotsModel();
                DotsList.Add(dt);

                await Clients.All.SendAsync("SingleDot", JsonConvert.SerializeObject(dt));
            }

            await Clients.All.SendAsync("IncreaseSize", JsonConvert.SerializeObject(Jogadores[index]));
            
        }
    }
}
