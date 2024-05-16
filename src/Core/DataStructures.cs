using System.Runtime.InteropServices;

namespace PuPuSharp
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FIFileHeader
    {
        public int fileSizeUncompressed;
        public int fileLocationOffset;
        public int fileCompression; // 0 = None, 1 = LZS
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FieldImageInfo
    {
        public int ID;
        public int Xmin;
        public int Xmax;
        public int Ymin;
        public int Ymax;
        public int Width;
        public int Height;
    }
}
