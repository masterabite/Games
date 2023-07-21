using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System;

namespace Opentk2d
{

    //базовый класс, описывающий общие признаки объектов игры
    internal class Object
    {
        protected Game game;                        //каждый объект привязан к игре
                                        
        protected int vertexBufferId;               //необходимая память для отрисовки
        protected float[] vertexData;

        protected Texture2D texture;                //текущая текстура (спрайт) ообъекта
        protected Vector2 drawSize;                 //размеры отрисовки текстуры

        protected Vector2 position { get; set; }    //координатное расположение в игре 
        
        protected Rectangle collisionMask;          //прямоугольник, показывающий какая маска для столкновений к объекта (относительно его position)

        //самый простой конструктор с пустым спрайтом
        public Object(Game _game, Vector2 _position)
        {
            game = _game;
            texture = new Texture2D(@"Game\empty.png");                 //пустая текстура
            position = _position;   
            drawSize.X = texture.W;                                     //картинку рисуем в ее оригинальных размерах
            drawSize.Y = texture.H;

            collisionMask = new Rectangle(0, 0, texture.W, texture.H);  //маска для столкновений по умолчанию распространяется на размеры картинки
            vertexBufferId = GL.GenBuffer();                            //выделяем буфер для отрисовки 
            SetBuffer();                                                //заполняем его
        }

        //конструктор по аналогии с предыдущим, только добавляется параметр, задающий размеры картинки объекта
        public Object(Game _game, Vector2 _position, Vector2 _size)
        {
            game = _game;
            texture = new Texture2D(@"Game\empty.png");
            position = _position;
            drawSize = _size;

            collisionMask = new Rectangle(0, 0, Convert.ToInt32(drawSize.X), Convert.ToInt32(drawSize.Y));

            vertexBufferId = GL.GenBuffer();
            SetBuffer();
        }

        //конструктор по аналогии с предыдущим, только добавляется путь к файлу картинки
        public Object(Game _game, Vector2 _position, Vector2 _size, string filename)
        {
            game = _game;
            texture = new Texture2D(filename);
            position = _position;
            drawSize = _size;

            collisionMask = new Rectangle(0, 0, Convert.ToInt32(drawSize.X), Convert.ToInt32(drawSize.Y));

            vertexBufferId = GL.GenBuffer();
            SetBuffer();
        }

        //конструктор по аналогии с предыдущим, только добавляется параметр, задающий маску для столкновений
        public Object(Game _game, Vector2 _position, Vector2 _size, string filename, Rectangle _collisionMask)
        {
            game = _game;
            texture = new Texture2D(filename);
            position = _position;
            drawSize = _size;

            collisionMask = _collisionMask;

            vertexBufferId = GL.GenBuffer();
            SetBuffer();
        }

        public Object(Game _game, Vector2 _position, Bitmap bitmap)
        {
            game = _game;
            texture = new Texture2D(bitmap);                 //пустая текстура
            position = _position;
            drawSize.X = texture.W;                                     //картинку рисуем в ее оригинальных размерах
            drawSize.Y = texture.H;

            collisionMask = new Rectangle(0, 0, texture.W, texture.H);  //маска для столкновений по умолчанию распространяется на размеры картинки
            vertexBufferId = GL.GenBuffer();                            //выделяем буфер для отрисовки 
            SetBuffer();                                                //заполняем его
        }

        //функция которая возвращает текущее положение маски столкновения объекта
        public Rectangle GetCollisionMask()
        {
            //т.е. мы возвращаем прямоугольник маски, который отсчитывается от текущей position объекта
            return new Rectangle(
                Convert.ToInt32(position.X) +  collisionMask.X,
                Convert.ToInt32(position.Y) + collisionMask.Y,
                collisionMask.Width,
                collisionMask.Height);
        }

        public void SetPosition(Vector2 pos)
        {
            position = pos;
            SetBuffer();
        }

        //Основная функция задающая изменение позиции объекта на вектор
        public void Move(Vector2 direction)
        {
            //System.Console.WriteLine(game.UpdateTime + " " + game.RenderTime);
            if (game.GetFPS() != 0)
            {
                position += direction * game.GetVSK();
            }
            SetBuffer();    //незабываем учитывать это в буфере отрисовки
        }

        //метод возвращающий текущую позицию объекта
        public Vector2 GetPosition()
        {
            return position;
        }

        //переобпределяемая функция, которая срабатывает каждый шаг игры
        public virtual void Step()
        {

        }

        //функция которая добавляет наш отрисовочный буфер к общей картине игры
        public void Draw()
        {

            //обозначаем начало и конец текстуры объекта
            texture.Bind();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);
            GL.DrawArrays(PrimitiveType.Quads, 0, vertexData.Length);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            texture.Unbind();
        }

        //функция заполняет отрисовочный буфер в соответствии с нашей картинкой
        protected void SetBuffer()
        {
            //определяем в каких границах игры нужно отрисовать текстуру объекта
            vertexData = new float[]
            {
                0.0f+position.X, 0.0f+position.Y,0.0f,
                drawSize.X+position.X, 0.0f+position.Y,0.0f,
                drawSize.X+position.X, drawSize.Y+position.Y,0.0f,
                0.0f+position.X, drawSize.Y+position.Y,0.0f
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId); //номером буфера обозначаем его начало
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw); //заполняем следующее пространство информацией о нашей текстуре
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //обозначается конец цепочки информации о нашей текстуры

        }

        //функция нужная для удаления отрисовочного буфера из игры
        public void Dispose()
        {
            texture?.Dispose();
            GL.DeleteBuffer(vertexBufferId);
        }
    }
}
