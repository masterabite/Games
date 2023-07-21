using OpenTK;
using System.Drawing;

namespace Opentk2d
{
    //класс описыващий платформу
    class Block : Object
    {
        //содержит только конструктор с маской столковения
        public Block(Game game, Vector2 position) : 
            base(
                game, 
                position,
                new Vector2(128, 32), 
                @"Game\Platform.png",
                new Rectangle(20, 0, 90, 24))
        {

        }
    }
}
