using Xenko.Engine;

namespace Xenko_GameOff2018
{
    class Xenko_GameOff2018App
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
