namespace Jogin.Models
{
    public class JogadoresModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int posX = 0;

        public int posY = 0;

        public int size = 25;

        public JogadoresModel()
        {
            Random random = new Random();
            this.posX = random.Next(0, 1000);
            this.posY = random.Next(0, 1000);
        }
        
    }
}
