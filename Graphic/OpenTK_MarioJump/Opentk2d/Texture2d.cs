using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL;


namespace Opentk2d
{

    //класс описывающий текстуру
    class Texture2D: IDisposable
    {
        public int ID;
        public int W;                 //длина картинки
        public int H;                 //высота
        public int BufferID;           //идентификационный номер буфера текстуры
        public float[] Coordinate;     //обозначение геометрической формы отрисовки картинки

        //конструктор с путем до файла картинки
        public Texture2D(string fileName)
        {

            //определяем изображение в виде матрицы и пытаемся туда загрузить саму картинку
            Bitmap bmp = new Bitmap(1, 1);
            if (File.Exists(fileName))
            {
                bmp = (Bitmap)Image.FromFile(fileName);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }

            //определяем высоту и ширину текстуры
            W = bmp.Width;
            H = bmp.Height;

            DefineBitmapData(bmp);
        }

        //конструктор с путем до файла картинки
        public Texture2D(Bitmap bmp)
        {
            DefineBitmapData(bmp);
        }

        public void DefineBitmapData(Bitmap bmp)
        {
            //определяем высоту и ширину текстуры
            W = bmp.Width;
            H = bmp.Height;

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            ID = GL.GenTexture();

            //выбираем эту текстуру как цель для отрисовки
            GL.BindTexture(TextureTarget.Texture2D, ID);

            //настраиваем параметры текстуры, также учитываем двумерное пространство
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (uint)TextureMinFilter.Linear);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                data.Width,
                data.Height,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            //нужно указать координаты угловых точек, в нашем случае картинки ограничены прямогугольником поэтому нам нужны четыре угловые точки
            Coordinate = new[]
            {
                0f, 1f,
                1f, 1f,
                1f, 0f,
                0f, 0f
            };

            //создаем буфер чтобы хранить текстуру, сразу определяем его номер
            BufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, BufferID);   //номером буфера обозначаем его начало
            GL.BufferData(BufferTarget.ArrayBuffer, Coordinate.Length * sizeof(float), Coordinate, BufferUsageHint.StaticDraw); //заполняем следующее пространство информацией о нашей текстуре
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //обозначается конец цепочки информации о нашей текстуры
        }

        //функция определяет текстуру и буфер для ее отрисовки при общем рендере
        public void Bind()
        {

            //обозначаем начало текстуры и ее буфера их идентификационными номерами
            GL.BindTexture(TextureTarget.Texture2D,ID);         
            GL.BindBuffer(BufferTarget.ArrayBuffer,BufferID);   
            GL.TexCoordPointer(2,TexCoordPointerType.Float,0,0);
        }

        //функция которая определяет конец текстуры и ее буфера 
        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        //функция чистит память занимаемую текстурой
        public void Dispose()
        {
            GL.DeleteBuffer(BufferID);
            GL.DeleteTexture(ID);
        }
    }
}
