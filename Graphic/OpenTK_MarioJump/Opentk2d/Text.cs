using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opentk2d
{

    //класс для отрисовки текста
    class Text: Object
    {

        private string text;
        private Graphics gfx;
        private Bitmap text_bmp;
        private Font font;
        public Text(Game game, Vector2 position, string text): base(game, position, new Vector2(400, 100))
        {
            text_bmp = new Bitmap(400, 100);
            // ! Создаем поверхность рисования GDI+ из картинки
            gfx = Graphics.FromImage(text_bmp);
            // ! Создаем шрифт
            font = new Font(FontFamily.GenericSerif, 11.0f);
            // ! Отрисовываем строку в поверхность рисования (в картинку)
            gfx.DrawString(text, font, Brushes.Black, new PointF(0, 0));
            text_bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            texture.DefineBitmapData(text_bmp);
        }

        public void SetText(string _text)
        {
            text = _text;
            text_bmp = new Bitmap(400, 100);
            // ! Создаем поверхность рисования GDI+ из картинки
            gfx = Graphics.FromImage(text_bmp);
            // ! Отрисовываем строку в поверхность рисования (в картинку)
            gfx.DrawString(text, font, Brushes.Black, new PointF(0, 0));
            text_bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            texture.DefineBitmapData(text_bmp);
        }
    }
}
