using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Opentk2d
{
    //класс игрового окна
    class Game : GameWindow
    {
        private List<Object> objects;       //массив всех объектов
        private Background bkg;             //фон игры

        private Matrix4 ortho;              //матрица для работы с отрисовкой изображения в окнец
        private int viewObjectId;           //индекс объекта в массив, от которового определяется движение камеры

        //размер карты игры
        private int gameWidth;              
        private int gameHeight;

        //максимальная высота, к которой закреплен вид
        private int minHeight;

        float frameTime;
        int fps;
        int frms;

        //конструктор игрового окна
        public Game(int width, int height, string title)
        {
            //определяем размеры игры
            gameWidth = width;
            gameHeight = height;
            Title = title;
            Width = gameWidth;  //длина окна определяется длиной игровой карты, высота по умолчанию

            //настраиваем параметры отрисовки
            GL.Enable(EnableCap.Texture2D); //включаем отрисовку текстур

            //настраиваем отрисовочный движом в режим отрисовки текстур в виде массива координатных точек
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.Enable(EnableCap.Blend);     //чтобы текстуры отображались с прозрачностью
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha); //настройка функции прозрачности

            objects = new List<Object>();                                   //инициализируем массив объектов
            bkg = new Background(gameWidth*2, gameHeight, @"Game\Fon.png"); //инициализируем фон
            viewObjectId = 0;                                               //прикрепляем изменение вида к объекту игрока, у него индекс 0
            minHeight = gameHeight-Height/2;                                //сначал вид в максимально нижней точке
            frameTime = 0.0f;
            fps = 64;
            frms = 0;
        }

        //фунцкия доступа к массиву объектов
        public List<Object> GetObjects()
        {
            return objects;
        }

        public float GetVSK()
        {
            return (float)(UpdateTime + RenderTime)/ (0.008f);
        }

        //функция устанавливает вид в соответствие с наибольшей высотой 
        public void SetView()
        {
            GL.Viewport(
                0,
                minHeight - gameHeight + Height / 2,
                gameWidth, gameHeight);
        }

        public float GetFPS()
        {
            return fps;
        }

        //функция перезапускает игру
        public void Restart()
        {
            objects.Clear();                        //удаляем объекты
            minHeight = gameHeight - Height / 2;    //возвращаем вид в нижнюю точку
            SetView();                              //возвращаем вид в нижнюю точку
            GenerateLevel();                        //генерируем уровень заного
        }

        //функция вызывается при изменении размеров окна
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetView();
            ortho = Matrix4.CreateOrthographicOffCenter(0, gameWidth, gameHeight, 0, -1, 0);    //создаем матричную проекцию для нашего двухмерного вида
            GL.MatrixMode(MatrixMode.Projection);   //устанавливаем режим для загрузки проекции
            GL.LoadMatrix(ref ortho);               //загружаем нашу матрицу по ссылке
            GL.MatrixMode(MatrixMode.Modelview);    //устанавливаем режим просмотра
            GL.LoadIdentity();                      //приводим матрицу к виду для нормального отображения
            bkg.Resize(gameWidth, gameHeight);      //растягиваем фон по всей карте
     
        }

        //функция доступа к размеру игры
        public int GetGameWidth()
        {
            return gameWidth;
        }

        //функция вызывается при запуске окна, устанавливает параметры отрисовки и генерирует уровень
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GenerateLevel();
        }

        //функция вызывается при обновлении кадра отрисовки окна
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            frameTime += (float)e.Time;
            frms++;
            if (frameTime >= 1.0f)
            {
                fps = frms;
                ((Text)objects[objects.Count - 2]).SetText($"FPS: {fps}"); //обновляем счёт
                frms = 0;
                frameTime -= 1.0f;
            }

            //если объект, к которому привязан вид, поднимается выше наибольшей тотчки, нужно ее обновить и перерисовать вид
            if (objects[viewObjectId].GetPosition().Y < minHeight && objects[viewObjectId].GetPosition().Y > Height/2)
            {
                ((Text)objects[objects.Count-1]).SetText($"Счёт: {gameHeight - minHeight + Height / 2}"); //обновляем счёт
 
                minHeight = Convert.ToInt32(objects[viewObjectId].GetPosition().Y);
                GL.Viewport(
                0,
                minHeight-gameHeight+Height/2,
                gameWidth, gameHeight);
            }

            objects[objects.Count - 1].SetPosition(new Vector2(10, minHeight - Height / 2));         //двигаем счет 
            objects[objects.Count - 2].SetPosition(new Vector2(10, minHeight - Height / 2+24));         //двигаем счет 

            //для каждого объекта нужно вызвать функцию шага
            for (int i = 0; i < objects.Count; ++i) {
                objects[i].Step();

                //если объекты опускаются ниже окна отрисовки, удаляем их
                if (objects[i].GetPosition().Y > minHeight+Height/2)
                {
                    if (i != 0) //если объект не игрок, то он просто удаляется
                    {
                        objects.Remove(objects[i]);
                        --i;
                    }
                    else //для игрока особый случай
                    {
                        if (!((Player)objects[i]).isDead()) { //если игрок падает вниз первый раз
                            ((Player)objects[i]).GameOver();    //игра завершается
                        } else
                        {
                            Restart();  //если игрок упал при том что уже проиграл, игра перезапускается
                        }
                    }
                }
            } 
        }

        //функция отрисовывает текущее положение игры в окне
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            //очищаем экран и устанавливаем фоновый цвет
            GL.Clear(ClearBufferMask.ColorBufferBit|ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color4.Gray);
            
            bkg.Draw(); //рисуем фон

            foreach (Object obj in objects)
            {
                obj.Draw(); //рисуем все объекты
            }

            //для нормального отображения нужно поменять порядок буферов
            SwapBuffers();
        }

        //функция проверяет, сталкивается ли объект me с объектом класса classType
        public Object PlaceMeeting(Object me, Type classType)
        {
            Rectangle m_mask = me.GetCollisionMask();  //определяем маску столкновения для объекта me

            //перебираем все отличные от me объекты класса classType
            foreach (Object obj in objects)
            {
                if (obj.GetType() != classType || obj == me) continue;

                //если маска объекта пересекается с объектом me, возвращаем его
                if (obj.GetCollisionMask().IntersectsWith(m_mask))
                {
                    return obj;
                }
            }
            return null;    //если таких объектов нет, возвращаем null
        }

        //функция проверяет, происходит ли столкновение с какими-то объектами в точке position
        public bool PlaceFree(Vector2 position)
        {
            Point p = new Point(Convert.ToInt32(position.X), Convert.ToInt32(position.Y));
            foreach (Object obj in objects)
            {
                if (obj.GetCollisionMask().Contains(p))
                {
                    return false;
                }
            }
            return true;
        }

        //функция проверяет, не сталкивается ли объект me с какими-то другими объектами
        public bool PlaceFree(Object me)
        {
            Rectangle m_mask = me.GetCollisionMask();
            foreach (Object obj in objects)
            {
                if (obj == me) continue;

                if (obj.GetCollisionMask().IntersectsWith(m_mask))
                {
                    return false;
                }
            }
            return true;
        }


        //функция проверяет, не сталкивается ли объект me с какими-то другими объектами, если поместить его в точку position
        public bool PlaceFree(Vector2 position, Object me)
        {
            Rectangle m_mask = new Rectangle(new Point(Convert.ToInt32(position.X), Convert.ToInt32(position.Y)), me.GetCollisionMask().Size);
            foreach (Object obj in objects)
            {
                if (obj == me) continue;

                if (obj.GetCollisionMask().IntersectsWith(m_mask))
                {
                    return false;
                }
            }
            return true;
        }

        //функция проверяет, происходит ли столкновение с какими-то объектами в точке position кроме объекта me
        public bool PositionFree(Vector2 position, Object me)
        {
            Point point = new Point(Convert.ToInt32(position.X), Convert.ToInt32(position.Y));
            foreach (Object obj in objects)
            {
                if (obj == me) continue;

                if (obj.GetCollisionMask().Contains(point))
                {
                    return false;
                }
            }
            return true;
        }

        //функция генерирует уровень
        public void GenerateLevel()
        {
            Random rnd = new Random();  //создаем рандомайзер

            
            objects.Add(new Player(this, new Vector2(gameWidth / 2, gameHeight)));      //добавляем игрока
            objects.Add(new Block(this, new Vector2(gameWidth / 2-10, gameHeight-20))); //добавляем под него платформу

            //далее на протижении всей карты генерируем платформы, пружины и мобом 
            for (int j = gameHeight-240; j > 0; j -= 138+(rnd.Next()%64))
            {
                Vector2 platformPosition = new Vector2(rnd.Next() % (gameWidth - 64), j);

                //с вероятностью 20%
                if (rnd.Next() % 100 > 80)
                {
                    //добавляем моба
                    objects.Add(new EnemyFly(this, new Vector2(rnd.Next() % (gameWidth - 82), j) + new Vector2(0, -64)));
                }

                //с вероятностью 14%
                if (rnd.Next() % 100 > 86)
                {
                    //добавляем пружину
                    objects.Add(new Spring(this, platformPosition + new Vector2(32, -8)));
                    j -= 256;
                }

                //добавляем платформу
                objects.Add(new Block(this, platformPosition));
            }

            objects.Add(new Text(this, new Vector2(10, gameHeight - Height+24), "FPS: 0")); //текст со счётом
            objects.Add(new Text(this, new Vector2(10, gameHeight - Height), "Счёт: 0")); //текст со счётом
        }
    }
}
