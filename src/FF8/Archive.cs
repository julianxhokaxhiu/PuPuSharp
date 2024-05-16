using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PuPuSharp.FF8
{
    internal class Archive
    {
        public Dictionary<string, Dictionary<string, MemoryStream>> items = new();

        public Archive(Stream fiData, Stream flData, Stream fsData)
        {
            var streamReader = new StreamReader(flData, Encoding.UTF8, true);
            string filePath = string.Empty;
            while ((filePath = streamReader.ReadLine()) != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                if (!items.ContainsKey(fileName)) items[fileName] = new();

                items[fileName][filePath] = getData(fileName, fiData, fsData);
            }
        }

        private MemoryStream getData(string fileName, Stream fiData, Stream fsData)
        {
            FIFileHeader header = fiData.ReadStruct<FIFileHeader>();
            int fileSize;

            // Fetch file size
            if (header.fileCompression == 1)
            {
                fsData.Seek(header.fileLocationOffset, SeekOrigin.Begin);
                fileSize = fsData.ReadAs<int>();
            }
            else
                fileSize = header.fileSizeUncompressed;

            // Fetch file data
            MemoryStream dataStream = new MemoryStream(fileSize);
            fsData.CopyExactly(dataStream, dataStream.Capacity);

            // Decompress if needed
            if (header.fileCompression == 1)
            {
                MemoryStream uncompressedStream = new(header.fileSizeUncompressed);

                Lzs.Decode(dataStream, uncompressedStream);

                // Safety check, if the decompressed data does not match the expected uncompressed size, alert the user
                if (uncompressedStream.Length != header.fileSizeUncompressed)
                    MessageBox.Show($"Something went wrong while decompressing {fileName}. Please make sure your files are not corrupted and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Ensure the cursor is set at the beginning for the next iteration if we're going to read this stream
                uncompressedStream.Seek(0, SeekOrigin.Begin);

                return uncompressedStream;
            }
            else
                return dataStream;
        }
    }
}
