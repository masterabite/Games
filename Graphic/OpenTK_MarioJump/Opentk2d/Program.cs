namespace Opentk2d
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(480, 15360, "Mario jump");   
            //game.VSync = OpenTK.VSyncMode.Adaptive;
            game.Run();
        }
    }
}
