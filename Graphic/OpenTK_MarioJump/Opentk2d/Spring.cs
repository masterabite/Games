using OpenTK;

namespace Opentk2d
{
    //класс пружины
    class Spring: Object
    {
        private Texture2D textureOpen = new Texture2D(@"Game\springOpen.png");      //дополнительная текстура для раскрытого состояния

        public Spring(Game game, Vector2 position) :
    base(
        game,
        position,
        new Vector2(64, 24),
        @"Game\springClose.png")
        {}


        //функция вызывается при раскрытии пружины
        public void Use()
        {
            texture = textureOpen;
            Move(new Vector2(0, -10));
        }
    }
}
