using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opentk2d
{

    //класс для противника моба
    class EnemyFly: Object
    {
        private float xSpeed;       //горизонтальная скорость
        private float ySpeed;
        private float moveSpeed;

        //конструктор для противника
        public EnemyFly(Game game, Vector2 position) :
            base(game,
                position,
                new Vector2(80, 64),
                @"Game\enemyFly.png",
                new Rectangle(10, 10, 64, 64))
        {
            moveSpeed = 2.0f;
            xSpeed = moveSpeed;
            ySpeed = 0;
        }

        //функция вызывается при убийстве моба
        public void Kill()
        {
            xSpeed = 0.0f;
            ySpeed = 0.64f;
        }

        //переопределяется поведение моба
        public override void Step()
        {
            base.Step();
            Move(new Vector2(xSpeed, ySpeed)); //моб перемещается со своей скоростью

            //при столкновении с границей экранов по горизонтали моб меняет направление
            if ((position.X >= game.GetGameWidth()-drawSize.X && xSpeed > 0) || (position.X <= 0 && xSpeed < 0))
            {
                xSpeed *= -1;
            }
            
        }
    }
}
