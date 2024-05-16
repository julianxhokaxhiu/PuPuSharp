using Microsoft.Win32;
using PuPuSharp.FF8;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PuPuSharp
{
    public partial class Entry : Form
    {
        // Internal
        private static readonly string[] FIELDS_TO_EXCLUDE = { "mapdata", "ec", "te", "main_chr" };
        private string FF8Dir = string.Empty;

        // FS
        private Archive archiveFs = null;
        private Dictionary<string, Archive> fieldsFs = new();

        // Rendering
        private int DefaultID = 0;
        private Field field = null;
        private nint renderingImageBuffer = nint.Zero;
        private int fieldImportedMultiplier = 1;
        private Dictionary<int, Bitmap> fieldHDTextures = new();

        public Entry()
        {
            InitializeComponent();

            this.Text = $"{Application.ProductName} v{Application.ProductVersion} (FF8 Field Importer/Exporter)";

            RegistryKey ret = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"Software\Square Soft, Inc\FINAL FANTASY VIII\1.00");
            if (ret != null)
            {
                FF8Dir = ret.GetValue("AppPath").ToString();
                ret.Close();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("https://github.com/julianxhokaxhiu/PuPuSharp")
            {
                UseShellExecute = true,
            };
            Process.Start(startInfo);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "FF8 Field file (field.fs)|field.fs|All files (*.*)|*.*";
            openFileDialog.DefaultExt = "fs";
            openFileDialog.FileName = "field.fs";
            if (FF8Dir != string.Empty) openFileDialog.InitialDirectory = Path.Combine(FF8Dir, "Data");

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fsFile = File.OpenRead(openFileDialog.FileName);
                FileStream fiFile = File.OpenRead(Path.ChangeExtension(openFileDialog.FileName, "fi"));
                FileStream flFile = File.OpenRead(Path.ChangeExtension(openFileDialog.FileName, "fl"));

                archiveFs = new Archive(fiFile, flFile, fsFile);

                lstFields.Items.Clear();
                foreach (string item in archiveFs.items.Keys.Order())
                {
                    if (!FIELDS_TO_EXCLUDE.Contains(item)) lstFields.Items.Add(item);
                }

                flFile.Close();
                fiFile.Close();
                fsFile.Close();
            }
        }

        private void lstFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (archiveFs != null)
            {
                string selected = lstFields.SelectedItem.ToString();

                MemoryStream fi = archiveFs.items[selected].Where(item => item.Key.EndsWith(".fi")).Select(item => item.Value).First();
                MemoryStream fl = archiveFs.items[selected].Where(item => item.Key.EndsWith(".fl")).Select(item => item.Value).First();
                MemoryStream fs = archiveFs.items[selected].Where(item => item.Key.EndsWith(".fs")).Select(item => item.Value).First();

                if (!fieldsFs.ContainsKey(selected)) fieldsFs[selected] = new Archive(fi, fl, fs);

                try
                {
                    string mimFilename = Path.GetFileName(fieldsFs[selected].items[selected].Where(item => item.Key.EndsWith(".mim")).Select(item => item.Key).First());
                    MemoryStream mim = fieldsFs[selected].items[selected].Where(item => item.Key.EndsWith(".mim")).Select(item => item.Value).First();
                    MemoryStream map = fieldsFs[selected].items[selected].Where(item => item.Key.EndsWith(".map")).Select(item => item.Value).First();
                    field = new Field(mimFilename, mim, map);

                    // Clear cache
                    fieldHDTextures.Clear();
                    fieldImportedMultiplier = 1;

                    // Fill ID box
                    lstLayers.Items.Clear();
                    for (int i = 0; i < field.IDs.Count; i++)
                    {
                        int ID = field.IDs[i];
                        lstLayers.Items.Add(ID.ToString("X8"));
                    }
                    if (lstLayers.Items.Contains("004FF000"))
                    {
                        DefaultID = 0x004FF000;
                    }
                    else if (lstLayers.Items.Contains("00400000"))
                    {
                        DefaultID = 0x00400000;
                    }
                    else
                    {
                        DefaultID = Convert.ToInt32(lstLayers.Items[0].ToString(), 16);
                    }
                    for (int i = 0; i < field.IDs.Count; i++)
                    {
                        int ID = field.IDs[i];
                        if (ID == DefaultID) lstLayers.SelectedItem = lstLayers.Items[i];
                    }

                    DrawSelectedLayer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void lstLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawSelectedLayer();
        }

        private void mnuExportLayers_Click(object sender, EventArgs e)
        {
            if (archiveFs != null)
            {
                FolderBrowserDialog exportDialog = new FolderBrowserDialog();
                exportDialog.Description = "Select Destination Folder";
                if (exportDialog.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < lstFields.SelectedItems.Count; i++)
                    {
                        nint tmpBuffer = nint.Zero;

                        for (int j = 0; j < lstLayers.Items.Count; j++)
                        {
                            int imgID = Convert.ToInt32(lstLayers.Items[j].ToString(), 16);
                            string fieldOutputPath = GetFieldOutputPath();
                            string outPath = Path.Combine(exportDialog.SelectedPath, fieldOutputPath + "_" + lstLayers.Items[j].ToString() + ".png");

                            //Create dir if it doesn't exist
                            FileInfo tempInfo = new FileInfo(outPath);
                            tempInfo.Directory.Create();

                            // Prepare image
                            FieldImageInfo info = GetImageInfo(imgID);
                            Image image = GetImage(info, tmpBuffer);
                            image.Save(outPath, ImageFormat.Png);
                        }

                        Marshal.FreeHGlobal(tmpBuffer);
                        tmpBuffer = nint.Zero;
                    }

                    if (MessageBox.Show("PNGs exported successfully. Do you want to open the folder in explorer?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        Process.Start(new ProcessStartInfo(exportDialog.SelectedPath) { UseShellExecute = true });
                }
            }
        }

        private void mnuImportLayers_Click(object sender, EventArgs e)
        {
            if (archiveFs != null)
            {
                FolderBrowserDialog importFromDialog = new FolderBrowserDialog();
                if (importFromDialog.ShowDialog() == DialogResult.OK)
                {
                    string fieldOutputPath = GetFieldOutputPath();

                    // Calc multiplier for later
                    FieldImageInfo info = GetImageInfo(field.Sprites[0].ID);
                    string filePath = Path.Combine(importFromDialog.SelectedPath, fieldOutputPath + "_" + field.Sprites[0].ID.ToString("X8") + ".png");
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show($"Error: Missing required import file {filePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Image checkImg = Image.FromFile(filePath);
                    fieldImportedMultiplier = checkImg.Width / info.Width; //Don't need height

                    for (int j = 0; j < lstLayers.Items.Count; j++)
                    {
                        int imgID = Convert.ToInt32((String)lstLayers.Items[j], 16);
                        string imgPath = Path.Combine(importFromDialog.SelectedPath, fieldOutputPath + "_" + lstLayers.Items[j].ToString() + ".png");
                        fieldHDTextures[imgID] = new Bitmap(Image.FromFile(imgPath));
                    }

                    DrawSelectedLayer();
                }
            }
        }

        private void mnuExportModpathTextures_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog exportDialog = new FolderBrowserDialog();
            if (FF8Dir != string.Empty) exportDialog.InitialDirectory = Path.Combine(FF8Dir, "mods", "Textures", "field");
            if (exportDialog.ShowDialog() == DialogResult.OK)
            {
                string fieldOutputPath = GetFieldOutputPath();

                int numpages = GetNumPages();
                int[] depths = GetDepths(numpages);
                nint[] out_buffers = new nint[numpages];
                nint tmpBuffer = nint.Zero;

                // Init swizzled buffers
                for (int i = 0; i < numpages; i++)
                {
                    int out_length = (256 * fieldImportedMultiplier) * (256 * fieldImportedMultiplier) * 2; //W * depth * H * Pixel Depth (ARGB1555=2)
                    out_buffers[i] = Marshal.AllocHGlobal(out_length);
                    for (int j = 0; j < out_length; j++) Marshal.WriteByte(out_buffers[i], j, 0);
                }

                // Swizzle buffers
                for (int j = 0; j < lstLayers.Items.Count; j++)
                {
                    int imgID = Convert.ToInt32((String)lstLayers.Items[j], 16);
                    FieldImageInfo info = GetImageInfo(imgID);
                    Bitmap inBmp = fieldHDTextures.ContainsKey(imgID) ? fieldHDTextures[imgID] : GetImage(info, tmpBuffer);
                    BitmapData bmpData = inBmp.LockBits(new Rectangle(0, 0, inBmp.Width, inBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format16bppArgb1555);
                    MapImportBuffers(imgID, bmpData.Scan0, out_buffers, fieldImportedMultiplier);
                    inBmp.UnlockBits(bmpData);
                }

                if (tmpBuffer != nint.Zero)
                {
                    Marshal.FreeHGlobal(tmpBuffer);
                    tmpBuffer = nint.Zero;
                }

                //Save buffers
                for (int i = 0; i < GetNumPages(); i++)
                {
                    if (out_buffers[i] != nint.Zero && depths[i] != 0)
                    {
                        string outPath = Path.Combine(exportDialog.SelectedPath, fieldOutputPath + "_" + i + ".png"); //filename with page suffix, note *10 for 4-bit indexed
                                                                                                                      //Create dir if it doesn't exist
                        FileInfo tempInfo = new FileInfo(outPath);
                        tempInfo.Directory.Create();
                        //Save image
                        Image tempImg = new Bitmap(256 * fieldImportedMultiplier, 256 * fieldImportedMultiplier, 256 * fieldImportedMultiplier * 2, PixelFormat.Format16bppArgb1555, out_buffers[i]);
                        tempImg.Save(outPath, ImageFormat.Png);
                        Marshal.FreeHGlobal(out_buffers[i]);
                    }
                }

                if (MessageBox.Show("Modpath textures exported successfully. Do you want to open the folder in explorer?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Process.Start(new ProcessStartInfo(exportDialog.SelectedPath) { UseShellExecute = true });
            }
        }

        private void DrawSelectedLayer()
        {
            if (archiveFs != null)
            {
                int ID = Convert.ToInt32(lstLayers.SelectedItem.ToString(), 16);

                if (fieldHDTextures.ContainsKey(ID))
                {
                    pbImage.Image = fieldHDTextures[ID];
                }
                else
                {
                    FieldImageInfo info = GetImageInfo(ID);
                    pbImage.Image = GetImage(info, renderingImageBuffer);
                }

            }
        }

        private string GetFieldOutputPath()
        {
            string fieldInternalPath = archiveFs.items[lstFields.SelectedItem.ToString()].Where(item => item.Key.EndsWith(".fs")).Select(item => item.Key).First().Replace(".fs", "");
            fieldInternalPath = Path.Combine(fieldInternalPath.Substring(fieldInternalPath.IndexOf("mapdata")), Path.GetFileNameWithoutExtension(fieldInternalPath));

            return fieldInternalPath;
        }

        private int GetNumPages()
        {
            if (field.Type == 1 || field.Type == 4) return 13; //types 1, 4
            else return 12; //types 2,3
        }

        private int[] GetDepths(int numpages)
        {
            int[] depths = new int[numpages];
            for (int i = 0; i < numpages; i++) //one image per page
            {
                int fourflag = 0;
                int eightflag = 0;
                for (int j = 0; j < field.NumFISprites; j++)
                {
                    if (field.Sprites[j].Page == i) //find depth for each page
                    {
                        if (field.Sprites[j].Depth < 4)
                        {
                            fourflag = 2;
                        }
                        else
                        {
                            eightflag = 1;
                        }
                    }
                }
                depths[i] = fourflag + eightflag;
            } //only depths that have values that are non-negative are the pages to write

            return depths;
        }

        private FieldImageInfo GetImageInfo(int ID)
        {
            int Xmin = 0, Xmax = 0, Ymin = 0, Ymax = 0;

            for (int i = 0; i < field.NumFISprites; i++)
            {
                if ((field.Sprites[i].ID & 0xFF000000) == (ID & 0xFF000000))
                {
                    Xmin = Math.Min(Xmin, field.Sprites[i].X);
                    Xmax = Math.Max(Xmax, field.Sprites[i].X);
                    Ymin = Math.Min(Ymin, field.Sprites[i].Y);
                    Ymax = Math.Max(Ymax, field.Sprites[i].Y);
                }
            }

            return new FieldImageInfo
            {
                ID = ID,
                Xmin = Xmin,
                Xmax = Xmax,
                Ymin = Ymin,
                Ymax = Ymax,
                Width = Xmax - Xmin + 16,
                Height = Ymax - Ymin + 16
            };
        }

        private unsafe Bitmap GetImage(FieldImageInfo info, nint imageBuffer)
        {
            if (imageBuffer != 0)
            {
                Marshal.FreeHGlobal(imageBuffer);
                imageBuffer = nint.Zero;
            }

            int size = info.Width * info.Height * 2; //W * H * Pixel Depth (RGB=3)
            imageBuffer = Marshal.AllocHGlobal(size);
            for (int i = 0; i < size; i++) Marshal.WriteByte(imageBuffer, i, 0);
            for (int i = 0; i < field.NumFISprites; i++)
            {
                if (field.Sprites[i].ID == info.ID)
                {
                    int target = field.Sprites[i].GetTarget(info.Width, info.Height, info.Xmin, info.Ymin);
                    for (int x = 0; x < 16; x++)
                    {
                        for (int y = 0; y < 16; y++)
                        {
                            ushort pixel = field.Palette.GetPixel(field.Sprites[i], x, y);
                            BinaryPrimitives.WriteUInt16LittleEndian(
                                new Span<byte>(
                                    nint.Add(imageBuffer, (target + ((x * info.Width) + y)) * 2).ToPointer(),
                                    2
                                ),
                                pixel
                            );
                        }
                    }
                }
            }

            return new Bitmap(info.Width, info.Height, info.Width * 2, PixelFormat.Format16bppArgb1555, imageBuffer);
        }

        private unsafe void MapImportBuffers(int ID, nint in_buffer, nint[] out_buffers, int mult)
        {
            FieldImageInfo info = GetImageInfo(ID);

            for (int i = 0; i < field.NumFISprites; i++)
            {
                if (field.Sprites[i].ID == ID)
                {
                    int source = (((field.Sprites[i].Y - info.Ymin) * mult) * (info.Width * mult)) + ((field.Sprites[i].X - info.Xmin) * mult);
                    int target = ((field.Sprites[i].SrcY * mult) * (256 * mult)) + (field.Sprites[i].SrcX * mult);

                    for (int j = 0; j < (16 * mult); j++)
                    {
                        for (int k = 0; k < (16 * mult); k++)
                        {
                            new Span<byte>(
                                nint.Add(in_buffer, (source + ((j * (info.Width * mult)) + k)) * 2).ToPointer(),
                                2
                            ).CopyTo(
                                new Span<byte>(
                                    nint.Add(out_buffers[field.Sprites[i].Page], (target + ((j * (256 * mult)) + k)) * 2).ToPointer(),
                                    2
                                )
                            );
                        }
                    }
                }
            }
        }
    }
}
