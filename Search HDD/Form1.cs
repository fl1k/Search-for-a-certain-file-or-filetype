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

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr tMenu, int targetItem, int targetStatus);

        bool cShowWindow = false;
        bool Copy = false;

        SearchDirectories SearchDirectories = new SearchDirectories();
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
                        for (int j = 0; j < 2; j++)
                        {
                            for (int i = 0; i < listBox1.Items.Count; i++)
                            {
                                if (listBox1.Items[i].ToString().Contains(folderDialog.SelectedPath))
                                {
                                    listBox1.Items.Remove(listBox1.Items[i].ToString());
                                    i = 0;
                                }
                            }
                        }
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
                listBox1.Items.Remove(listBox1.SelectedItem);
            if (radioBtnEverything.Checked)
                radioBtnSelect.Checked = true;
        }

        private void radioBtnEverything_CheckedChanged(object sender, EventArgs e)
        {
            CheckForFormUpdates();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioBtnEndsWith.Checked = true;
            ShowWindow(GetConsoleWindow(), 0);
            CheckForFormUpdates();
            radioBtnEverything.Checked = true;
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

        private void madeByFl1kToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/fl1k");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            radioBtnSelect.Checked = true;
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Console.Clear();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cShowWindow = !cShowWindow;
            ShowWindow(GetConsoleWindow(), Convert.ToInt32(cShowWindow));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0 || textBox2.TextLength == 0 || textBox1.TextLength == 0 || !(Directory.Exists(textBox1.Text)))
                MessageBox.Show("Please check input boxes for mistakes. \n(Copy path isn't valid / No term / No search path)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                DialogResult dialogResult = MessageBox.Show($"Log file with paths will be created, do you want the files to be copied to {textBox1.Text}\\Results?", "File Searcher", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                    Copy = true;
                else if (dialogResult == DialogResult.No)
                    Copy = false;

                string Date = String.Format("{0:yyyy-MM-dd-h-m-s}", DateTime.Now);
                ShowWindow(GetConsoleWindow(), 1);
                foreach (string path in listBox1.Items)
                {
                    if (radioBtnContains.Checked)
                    {
                        List<string> toCopy = SearchDirectories.FileNameContains(path, textBox2.Text, textBox1.Text);
                        SearchDirectories.Copy(toCopy, textBox1.Text, Date, Copy);
                    }
                    else if (radioBtnEndsWith.Checked)
                    {
                        List<string> toCopy = SearchDirectories.EndsWith(path, textBox2.Text, textBox1.Text);
                        SearchDirectories.Copy(toCopy, textBox1.Text, Date, Copy);
                    }
                    else if (radioBtnStartsWith.Checked)
                    {
                        List<string> toCopy = SearchDirectories.StartsWith(path, textBox2.Text, textBox1.Text);
                        SearchDirectories.Copy(toCopy, textBox1.Text, Date, Copy);
                    }
                }
                Console.WriteLine("\nDone..");
                Console.WriteLine("Closing the console will also close the form, if you want to close it go to File > Console > Hide");
            }
        }
    }
}
