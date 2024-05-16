using System.IO;

namespace PuPuSharp.FF8
{
    internal class Sprite
    {
        public int ID = 0;
        public int Type = 0;
        //Offsets
        public int o_sprite = 0;
        public int o_X = 0;
        public int o_Y = 0;
        public int o_Z = 0;
        public int o_page = 0;
        public int o_ZZ1 = 0;
        public int o_depth = 0;
        public int o_ZZ2 = 0;
        public int o_ZZ3 = 0;
        public int o_pal = 0;
        public int o_ZZ4 = 0;
        public int o_SrcX = 0;
        public int o_SrcY = 0;
        public int o_ZZ5 = 0;
        public int o_layerid = 0;
        public int o_blendmode = 0;
        public int o_animid = 0;
        public int o_animstate = 0;
        public short X = 0;
        public short Y = 0;
        public ushort Z = 0;
        public byte Page = 0;
        public byte ZZ1 = 0;
        public byte Depth = 0;
        public byte ZZ2 = 0;
        public byte ZZ3 = 0;
        public byte Pal = 0;
        public byte ZZ4 = 0;
        public short SrcX = 0;
        public short SrcY = 0;
        public byte ZZ5 = 0;
        public byte Layerid = 0;
        public byte Blendmode = 0;
        public byte Animid = 0;
        public byte Animstate = 0;

        public Sprite(Stream mapData, int i, int type)
        {
            this.Type = type;

            if (type == 1)
            {
                o_sprite = i * 16;//Each sprite is 16 bytes long (128 bits)
                
                //Offsets
                o_X = o_sprite;//s16, Target X
                o_Y = o_X + 2;//s16, Target Y
                o_Z = o_Y + 2;//u16, Z-coord (/4096->float)
                o_page = o_Z + 2;//u4, Texture page #, x128 + SrcX
                o_ZZ1 = o_page;//u1, Unknown, always 1?
                o_depth = o_page;//u3, <4 = 4-bit color, else 8-bit
                o_ZZ2 = o_page + 1;//u8, Unknown, always 0?
                o_ZZ3 = o_page + 2;//u6, Unknown, always 15? (backwards from wiki)
                o_pal = o_page + 2;//u4, Palette, +8
                o_ZZ4 = o_pal;//u6, Unknown, always 0?
                o_SrcX = o_pal + 2;//u8, Source X
                o_SrcY = o_SrcX + 1;//u8, Source Y
                o_ZZ5 = o_SrcY + 1;//u1, Unknown, always 0?
                o_layerid = o_SrcY + 1;//u7, Layer id	(backwards from wiki)
                o_blendmode = o_layerid + 1;//u8, Blend mode, 1=add, 2=sub, 3=+25%, 4=none(default), 0=unknown
                o_animid = o_blendmode + 1;//u8, Anim id
                o_animstate = o_animid + 1;//u8, Anim state
                
                //Values
                X = (short)mapData.ReadAs<ushort>(o_X);
                Y = (short)mapData.ReadAs<ushort>(o_Y);
                Z = mapData.ReadAs<ushort>(o_Z);
                Page = (byte)(mapData.ReadAs<byte>(o_page) & 0x0F);
                ZZ1 = (byte)((mapData.ReadAs<byte>(o_ZZ1) & 0x10) >> 4);
                Depth = (byte)(mapData.ReadAs<byte>(o_depth) >> 5);
                ZZ2 = mapData.ReadAs<byte>(o_ZZ2);
                ZZ3 = (byte)((mapData.ReadAs<ushort>(o_ZZ3) & 0xFC00) >> 10);
                Pal = (byte)((mapData.ReadAs<ushort>(o_pal) & 0x03C0) >> 6);
                Pal += 8;   //Must add 8 if type 1
                ZZ4 = (byte)(mapData.ReadAs<ushort>(o_ZZ4) & 0x003C);
                SrcX = mapData.ReadAs<byte>(o_SrcX);
                SrcY = mapData.ReadAs<byte>(o_SrcY);
                ZZ5 = (byte)(mapData.ReadAs<byte>(o_ZZ5) & 0x01);
                Layerid = (byte)(mapData.ReadAs<byte>(o_layerid) >> 1);
                Blendmode = mapData.ReadAs<byte>(o_blendmode);
                Animid = mapData.ReadAs<byte>(o_animid);
                Animstate = mapData.ReadAs<byte>(o_animstate);
            }
            else if (type == 2 || type == 4)
            {
                o_sprite = i * 16;//Each sprite is 16 bytes long (128 bits)
                
                //Offsets
                o_X = o_sprite;//s16, Target X
                o_Y = o_X + 2;//s16, Target Y
                o_SrcX = o_Y + 2;//s16, Source X
                o_SrcY = o_SrcX + 2;//s16, Source Y
                o_Z = o_SrcY + 2;//u16, Z-coord (/4096->float)
                o_page = o_Z + 2;//u4, Texture page #, x128 + SrcX
                o_ZZ1 = o_page;//u1, Unknown, always 1?
                o_depth = o_page;//u3, <4 = 4-bit color, else 8-bit
                o_ZZ2 = o_page + 1;//u8, Unknown, always 0?
                o_ZZ3 = o_page + 2;//u6, Unknown, always 15?
                o_pal = o_page + 2;//u4, Palette, +8
                o_ZZ4 = o_pal;//u6, Unknown, always 0?
                o_animid = o_pal + 2;//u8, Anim id
                o_animstate = o_animid + 1;//u8, Anim state
                
                //Values
                X = (short)(mapData.ReadAs<ushort>(o_X));
                Y = (short)(mapData.ReadAs<ushort>(o_Y));
                SrcX = (short)(mapData.ReadAs<ushort>(o_SrcX));
                SrcY = (short)(mapData.ReadAs<ushort>(o_SrcY));
                Z = mapData.ReadAs<ushort>(o_Z);
                Page = (byte)(mapData.ReadAs<byte>(o_page) & 0x0F);
                ZZ1 = (byte)((mapData.ReadAs<byte>(o_ZZ1) & 0x10) >> 4);
                Depth = (byte)(mapData.ReadAs<byte>(o_depth) >> 5);
                ZZ2 = mapData.ReadAs<byte>(o_ZZ2);
                ZZ3 = (byte)((mapData.ReadAs<ushort>(o_ZZ3) & 0xFC00) >> 10);
                Pal = (byte)((mapData.ReadAs<ushort>(o_pal) & 0x03C0) >> 6);
                if (type == 4) Pal += 8;    //Must add 8 if type 4
                ZZ4 = (byte)(mapData.ReadAs<ushort>(o_ZZ4) & 0x003C);
                Animid = mapData.ReadAs<byte>(o_animid);
                Animstate = mapData.ReadAs<byte>(o_animstate);

                ZZ5 = 0;
                Layerid = 0;
                Blendmode = 4;
            }
            else if (type == 3)
            {
                o_sprite = i * 14;//Each sprite is 14 bytes long (112 bits)
                
                //Offsets
                o_X = o_sprite;//s16, Target X
                o_Y = o_X + 2;//s16, Target Y
                o_SrcX = o_Y + 2;//s16, Source X
                o_SrcY = o_SrcX + 2;//s16, Source Y
                o_Z = o_SrcY + 2;//u16, Z-coord (/4096->float)
                o_page = o_Z + 2;//u4, Texture page #, x128 + SrcX
                o_ZZ1 = o_page;//u1, Unknown, always 1?
                o_depth = o_page;//u3, <4 = 4-bit color, else 8-bit
                o_ZZ2 = o_page + 1;//u8, Unknown, always 0?
                o_ZZ3 = o_page + 2;//u6, Unknown, always 15?
                o_pal = o_page + 2;//u4, Palette, +8
                o_ZZ4 = o_pal;//u6, Unknown, always 0?
                
                //Values
                X = (short)(mapData.ReadAs<ushort>(o_X));
                Y = (short)(mapData.ReadAs<ushort>(o_Y));
                SrcX = (short)(mapData.ReadAs<ushort>(o_SrcX));
                SrcY = (short)(mapData.ReadAs<ushort>(o_SrcY));
                Z = mapData.ReadAs<ushort>(o_Z);
                Page = (byte)(mapData.ReadAs<byte>(o_page) & 0x0F);
                ZZ1 = (byte)((mapData.ReadAs<byte>(o_ZZ1) & 0x10) >> 4);
                Depth = (byte)(mapData.ReadAs<byte>(o_depth) >> 5);
                ZZ2 = mapData.ReadAs<byte>(o_ZZ2);
                ZZ3 = (byte)((mapData.ReadAs<ushort>(o_ZZ3) & 0xFC00) >> 10);
                Pal = (byte)((mapData.ReadAs<ushort>(o_pal) & 0x03C0) >> 6);
                ZZ4 = (byte)(mapData.ReadAs<ushort>(o_ZZ4) & 0x003C);

                Animid = 0xFF;
                Animstate = 0x00;
                ZZ5 = 0;
                Layerid = 0;
                Blendmode = 4;
            }

            ID = 0x00000000;    //ID identifies the sprites composing an output file, an ID is 1:1 with a PNG
            ID += (Layerid << 24);
            ID += (Blendmode << 20);
            ID += (Animid << 12);
            ID += (Animstate << 4);	//Bottom 3 bits store overlapping sprites
        }

        public int GetSource()
        {
            switch(Type)
            {
                case 1:
                case 4:
                    return 0x3000 + (SrcY * 1664) + (Page * 128) + SrcX;
                case 2:
                case 3:
                    return 0x2000 + (SrcY * 1536) + (Page * 128) + SrcX;
                default:
                    return -1;
            }
        }

        public int GetTarget(int LWidth, int LHeight, int Xmin, int Ymin)
        {
            return ((Y - Ymin) * LWidth) + (X - Xmin);
        }
    }
}
