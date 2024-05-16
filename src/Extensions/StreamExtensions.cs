using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PuPuSharp
{
    public static class StreamExtensions
    {
        public static T ReadStruct<T>(this Stream stream) where T : struct
        {
            var sz = Marshal.SizeOf(typeof(T));
            var buffer = new byte[sz];
            stream.Read(buffer, 0, sz);
            var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var structure = (T)Marshal.PtrToStructure(
                pinnedBuffer.AddrOfPinnedObject(), typeof(T));
            pinnedBuffer.Free();
            return structure;
        }

        public static void WriteStruct<T>(this Stream stream, T data) where T : struct
        {
            var sz = Marshal.SizeOf(data);
            IntPtr pStruct = Marshal.AllocHGlobal(sz);
            Marshal.StructureToPtr(data, pStruct, false);
            var buffer = Array.CreateInstance(typeof(byte), sz) as byte[];
            Marshal.Copy(pStruct, buffer, 0, buffer.Length);
            stream.Write(buffer, 0, sz);
            Marshal.FreeHGlobal(pStruct);
        }

        public static long SeekStruct<T>(this Stream stream) where T : struct
        {
            var sz = Marshal.SizeOf(typeof(T));
            return stream.Seek(sz, SeekOrigin.Current);
        }

        public static unsafe T ReadAs<T>(this Stream stream) where T : unmanaged
        {
            T value;
            stream.ReadExactly(new Span<byte>(&value, sizeof(T)));
            return value;
        }

        public static unsafe T ReadAs<T>(this Stream stream, long offset, SeekOrigin origin = SeekOrigin.Begin) where T : unmanaged
        {
            stream.Seek(offset, origin);
            return ReadAs<T>(stream);
        }

        public static void CopyExactly(this Stream stream, MemoryStream ms, int count)
        {
            ms.SetLength(ms.Length + count);
            ms.TryGetBuffer(out ArraySegment<byte> segment);
            stream.ReadExactly(segment.AsSpan(0, count));
        }
    }
}
