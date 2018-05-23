using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace Search_HDD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForFormUpdates();
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;


        private void CheckForFormUpdates()
        {
            if (radioBtnEverything.Checked)
            {
                listBox1.Items.Clear();
                btnOpen.Enabled = false;
                foreach (DriveInfo d in DriveInfo.GetDrives().Where(x => x.IsReady))
                {
                    listBox1.Items.Add(d.RootDirectory.FullName);
                }
            }
            else if (radioBtnSelect.Checked)
            {
                listBox1.Items.Clear();
                btnOpen.Enabled = true;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    listBox1.Items.Add(folderDialog.SelectedPath);
                }
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }

        private void radioBtnEverything_CheckedChanged(object sender, EventArgs e)
        {
            CheckForFormUpdates();
        }

        private static void SearchDirectories(string path, string term, string copypath)
        {
            List<string> files = new List<string>();
            foreach (string file in Directory.EnumerateFiles(path).Where(x => x.Contains(term)))
            {
                try
                {
                    files.Add(file);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Added {file}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.Message);
                }
            }

            foreach (string subdir in Directory.EnumerateDirectories(path))
            {
                try
                {
                    SearchDirectories(subdir, term, copypath);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.Message);
                }
            }
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(copypath, name);
                File.Copy(file, dest, true);
            }
            Console.ResetColor();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            CheckForFormUpdates();
            radioBtnEverything.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0 || textBox2.TextLength == 0 || textBox1.TextLength == 0)
            {
                MessageBox.Show("Please fill all input boxes", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_SHOW);

                foreach (string file in listBox1.Items)
                {
                    SearchDirectories(file, textBox2.Text, textBox1.Text);
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    listBox1.Items.Add(folderDialog.SelectedPath);
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            radioBtnSelect.Checked = true;
            listBox1.Items.Clear();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
            this.Close();
        }

        private void hideUnhideConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_SHOW);
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }
    }
}