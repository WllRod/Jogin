namespace Jogin.Models
{
    public class DotsModel
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        public int size { get; set; } = 5;

        public int posX { get; set; }
        public int posY { get; set; }

        public DotsModel() 
        {
            Random rd = new Random();
            this.posX = rd.Next(0, 1800);
            this.posY = rd.Next(0, 900);
        }

    }
}
