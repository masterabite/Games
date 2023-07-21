using OpenTK.Graphics.OpenGL;
using System;

namespace Opentk2d
{

    //класс для фонового изображения по аналогии с текстурой
    class Background: IDisposable   
    {
        private int vertexBufferId;
        private float[] vertexData;
        private Texture2D texture;

        public Background(int width, int height, string filename)
        {
            texture=new Texture2D(filename);
            vertexBufferId = GL.GenBuffer();
            Resize(width,height);
        }

        //функция изменяет размеры фона
        public void Resize(int width, int height)
        {
            vertexData = new float[]
            {
                0.0f, 0.0f,0.0f,
                width, 0.0f,0.0f,
                width, height,0.0f,
                0.0f, height,0.0f
            };
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        //функция отрисовывает буфер в игре
        public void Draw()
        {
            //закрепляем текстуру в отрисовочном массиве
            texture.Bind();
            GL.BindBuffer(BufferTarget.ArrayBuffer,vertexBufferId);

            GL.VertexPointer(3,VertexPointerType.Float,0,0);            //определяем тип массива для отрисовочного движка

            GL.DrawArrays(PrimitiveType.Quads, 0,vertexData.Length);    //рендерим все содержащее массива
            GL.BindBuffer(BufferTarget.ArrayBuffer,0);                  //закрепляем массив в общем буфере
            texture.Unbind();                                           //открепляем текстуру фона
        }

        //функция очистки памяти
        public void Dispose()
        {
            texture?.Dispose();
            GL.DeleteBuffer(vertexBufferId);
        }
    }
}
