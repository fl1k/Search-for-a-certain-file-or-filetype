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


        /////////////////////////////////////////////////////////////////
        // Code is very messy because I'm lazy and I don't like forms. //
        /////////////////////////////////////////////////////////////////


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
        private void pathlbcheck()
        {
            bool contains = false;
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    if (listBox1.Items.Count == 0)
                        listBox1.Items.Add(folderDialog.SelectedPath);
                    else
                    {
                        //

                        // Add listBox filter

                        //
                        for (int i = 0; i < listBox1.Items.Count; i++)
                        {
                            if (folderDialog.SelectedPath.Contains(listBox1.Items[i].ToString()))
                                contains = true;
                        }
                        if (contains == false)
                            listBox1.Items.Add(folderDialog.SelectedPath);
                        else
                            MessageBox.Show("Path already included", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            pathlbcheck();
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

        private static void SearchDirectories(string path, string term, string copypath, string k)
        {
            List<string> files = new List<string>();
            if (k == "filename")
            {
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
            }
            else if (k == "filetype")
            {
                foreach (string file in Directory.EnumerateFiles(path).Where(x => x.EndsWith(term)))
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
            }
            foreach (string subdir in Directory.EnumerateDirectories(path))
            {
                try
                {
                    SearchDirectories(subdir, term, copypath, k);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.Message);
                }
            }

            foreach (string file in files)
            {
                string dest = Path.Combine(copypath, Path.GetFileName(file));
                File.Copy(file, dest, true);
                Console.WriteLine($"Copied {file}");
            }
            Console.ResetColor();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            CheckForFormUpdates();
            radioBtnEverything.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0 || textBox2.TextLength == 0 || textBox1.TextLength == 0 || !(Directory.Exists(textBox1.Text)))
            {
                MessageBox.Show("Please check input boxes for mistakes. \n(Copy path isn't valid / No term / No search path)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_SHOW);
                foreach (string file in listBox1.Items)
                {
                    if (radioButton2.Checked)
                        SearchDirectories(file, textBox2.Text, textBox1.Text, "filename");
                    else if (radioButton1.Checked)
                        SearchDirectories(file, textBox2.Text, textBox1.Text, "filetype");
                }

                Console.WriteLine("Done..");
                Console.WriteLine("Closing the console will also close the form, if you want to close it go to File > Console > Hide");
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                    textBox1.Text = folderDialog.SelectedPath;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pathlbcheck();
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
                    textBox1.Text = folderDialog.SelectedPath;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
            this.Close();
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

        private void madeByFl1kToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/fl1k");
        }
    }
}