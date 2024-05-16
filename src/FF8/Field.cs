using System.Collections.Generic;
using System.IO;

namespace PuPuSharp.FF8
{
    internal class Field
    {
        public int Type = 0;
        public Palette Palette = null;
        public int NumFISprites = 0;
        public List<Sprite> Sprites = new();
        public List<int> IDs = new();

        public Field(string mimName, Stream mimData, Stream mapData)
        {
            // Detect Field type
            switch(mimName)
            {
                case "logo.mim":
                    Type = 3;
                    break;
                case "test10.mim":
                case "test11.mim":
                case "test12.mim":
                    Type = 4;
                    break;
                default:
                    if (mimData.Length >= 0x6B000)
                        Type = 1;
                    else
                        Type = 2;
                    break;
            }

            // Create Palette
            Palette = new Palette(mimData, Type);
            NumFISprites = (int)((mapData.Length / 16) - 1);
            for (int i = 0; i < NumFISprites; i++)  //Build sprites
            {
                Sprites.Add(new Sprite(mapData, i, Type));
            }
            for (int i = 0; i < NumFISprites; i++)  //Separate overlapping layers
            {
                for (int j = i + 1; j < NumFISprites; j++)
                {
                    if ((Sprites[i].X == Sprites[j].X) && (Sprites[i].Y == Sprites[j].Y))
                    {
                        if ((Sprites[i].ID & 0xFFFFFFF0) == (Sprites[j].ID & 0xFFFFFFF0))
                        {
                            Sprites[j].ID = Sprites[i].ID + 1;
                        }
                    }
                }
            }
            for (int i = 0; i < NumFISprites; i++)  //Populate IDs
            {
                if (!IDs.Contains(Sprites[i].ID))
                {
                    IDs.Add(Sprites[i].ID);
                }
            }
            IDs.Sort();
        }
    }
}
