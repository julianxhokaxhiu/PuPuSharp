using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PuPuSharp
{
    public partial class Entry : Form
    {
        public Entry()
        {
            InitializeComponent();

            this.Text = $"{Application.ProductName} v{Application.ProductVersion} (FF8 Field Importer/Exporter)";
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
    }
}
