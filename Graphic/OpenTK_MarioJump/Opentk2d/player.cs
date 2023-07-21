using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Opentk2d
{

    //класс объекта игрок
    class Player : Object, IDisposable
    {

        private float xSpeed;       //текущая горизонтальная скорость
        private float ySpeed;       //текущая вертикальная скорость
        private float xSpeedMax;    //максимальная горизонтальная скорость
        private float jumpSpeed;    //скорость при прыжке
        private float moveSpeed;    //прирост горизонтальной скорости
        private float g = 0.08f;   //аналог гравитационной постоянной

        private Texture2D textureLeft = new Texture2D(@"Game\player1.png");     //картинка при движении влево
        private Texture2D textureRight = new Texture2D(@"Game\player.png");     //картинка при движении вправо
        private Texture2D textureDead = new Texture2D(@"Game\playerDead.png");  //картинка при проигрыше


        //конструктор для игрока по аналогии с родительским классом
        public Player(Game game, Vector2 position) : 
            base(game, 
                position, 
                new Vector2(64, 80), 
                @"Game\player.png",
                new Rectangle(10, 78, 54, 2))    //макса для столкновений игрока нужна только у ног
        {

            //устанавливаем параметры передвижения
            xSpeedMax = 4f;   
            jumpSpeed = 7.0f;
            moveSpeed = 0.14f;
            xSpeed = .0f;
            ySpeed = -jumpSpeed;
        }


        //Функция которая вызывается когда игрок проиграл
        public void GameOver()
        {
            texture = textureDead;      //устанавливаем нужную текстуру
            ySpeed = -jumpSpeed; //игрок подпрыгивает как игре марио
            xSpeed = 0;                 //игрок останавливается по горизонтали
        }

        //функция проверяет не завершилась ли игра
        public bool isDead()
        {
            //будем считать, что игра закончится когда игок поменят картинку на соответствующую
            return texture == textureDead;
        }

        //для управления игроком нужно переопределить функцию шага
        public override void Step()
        {
            float vsk = game.GetVSK();

            //получаем информацию о нажатых клавишах
            KeyboardState kb = Keyboard.GetState();

            //запоминаем предыдущую скорость, чтобы знать, когда направление игрока изментся и поменять картинку
            float prevXSpeed = xSpeed;

            //гравитация постоянно меняет вертикальную скорость
            ySpeed += g*vsk;

            //если игра окончена, игрок просто двигается со своей скоростью
            if (texture == textureDead)
            {
                Move(new Vector2(xSpeed, ySpeed));
                return;
            }

            //если игра не окончена
            //проверяем нажатость клавиш клавиатуры
            if (kb.IsKeyDown(Key.A) && xSpeed > -xSpeedMax)
            {
                xSpeed -= moveSpeed*vsk;
            }

            if (kb.IsKeyDown(Key.D) && xSpeed < xSpeedMax)
            {
                xSpeed += moveSpeed*vsk;
            }

            //если направление игрока изменилось, меняем картинку на соответствующую
            if (xSpeed * prevXSpeed <= 0)
            {
                texture = (xSpeed > 0 ? textureRight : textureLeft);
            }


            //проверяем есть ли объект противника, с которым сталкивается наш игрок
            EnemyFly enemy = (EnemyFly) game.PlaceMeeting(this, typeof(EnemyFly));
            if (enemy != null) //если есть такой противник
            {

                if (position.Y+drawSize.Y/2 <= enemy.GetPosition().Y)   //если мы "напрыгиваем" на него, то он умирает
                {
                    enemy.Kill();
                }
                else //иначе умирает игрок
                {
                    GameOver();
                }
            }

            //определяем вектор скорости игрока
            Vector2 speedVector = new Vector2(xSpeed, ySpeed);

            //если игрок не сталкивается с другими объектами или он движется вверх
            if (game.PlaceFree(this) || ySpeed <= 0)
            {
                //свободно перемещается по вектору скорости
                Move(speedVector);

                //следующий блок кода перености игрока в другой конец экрана при выходе за границы по горизонтали
                //если игрок перемещается влево за экран
                if (position.X < -drawSize.X)
                {
                    position = new Vector2(game.GetGameWidth(), position.Y);    //переносим его вправо
                } else if (position.X > game.GetGameWidth()) //если выходит за пределы экрана вправо
                {
                    position = new Vector2(-drawSize.X, position.Y);            //переносим игрока влево
                }
            } 
            else if (!game.PlaceFree(this))     //если происходит столкновение с чем-то
            {
                //проверяем, является ли это пружина
                Spring spring = (Spring)game.PlaceMeeting(this, typeof(Spring));
                if (spring != null)     //если столкнулись с пружиной
                {
                    ySpeed = -jumpSpeed * 1.8f;     //подкидываем игрока вверх с почти двойной скоростью прыжка
                    spring.Use();                   //также не забываем использовать пружину
                } else
                {
                    ySpeed = -jumpSpeed;            //иначе игрок просто прыгает вверх
                }
            }
        }
    }
}
