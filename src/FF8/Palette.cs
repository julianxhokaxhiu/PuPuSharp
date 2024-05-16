using System.IO;

namespace PuPuSharp.FF8
{
    internal class Palette
    {
        private Stream mimData = null;

        public int NumberOfColors = 0;
        public int NumberOfPalettes = 0;

        public Palette(Stream mimData, int type)
        {
            this.mimData = mimData;

            NumberOfColors = 256;

            switch (type)
            {
                case 1:
                case 4:
                    NumberOfPalettes = 24;
                    break;
                case 2:
                case 3:
                    NumberOfPalettes = 16;
                    break;
            }
        }

        public ushort GetPixel(Sprite sprite, int x, int y)
        {
            int add = sprite.Type == 1 || sprite.Type == 4 ? 128 : 0;
            int colorIndex, pixelOffset;
            int mimSprite = sprite.GetSource();

            if (sprite.Depth < 4) //4-bit indexed
            {
                mimSprite -= sprite.SrcX / 2;
                pixelOffset = (x * (1536 + add)) + (y / 2); //1536 for type 2 and 3, 1664 for type 1 and 4
                if (y % 2 == 1)
                {
                    colorIndex = (mimData.ReadAs<byte>(mimSprite + pixelOffset) & 0xF0) >> 4;
                }
                else
                {
                    colorIndex = mimData.ReadAs<byte>(mimSprite + pixelOffset) & 0x0F;
                }
            }
            else //8-bit indexed
            {
                pixelOffset = (x * (1536 + add)) + y;
                colorIndex = mimData.ReadAs<byte>(mimSprite + pixelOffset);
            }

            ushort palColor = mimData.ReadAs<ushort>((sprite.Pal * NumberOfColors * 2) + (colorIndex * 2));
            
            ushort ret = 0x0000; //Alpha is 0, everything black transparent by default
            ret = (ushort)(ret + ((palColor & 0x001F) << 10)); //R
            ret = (ushort)(ret + ((palColor & 0x03E0) << 0));  //G
            ret = (ushort)(ret + ((palColor & 0x7C00) >> 10)); //B
            if (palColor != 0) ret += 0x8000;	               //A

            return ret;
        }
    }
}
