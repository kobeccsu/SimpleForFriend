using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        List<string> contractList = new List<string>();
        List<string> finishedList= new List<string>();


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
            {
                lblSelectedPath.Text = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.LastRoot = lblSelectedPath.Text;
                Properties.Settings.Default.Save();
                //string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);

                //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblSelectedPath.Text = Properties.Settings.Default.LastRoot;
            lblDestPath.Text = Properties.Settings.Default.LastDest;
            textBox1.Text = Properties.Settings.Default.LastContacts;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
            {
                lblDestPath.Text = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.LastDest = lblDestPath.Text;
                Properties.Settings.Default.Save();
                //string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);

                //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            contractList = textBox1.Text.Split(new char[] { ',', '，' }).ToList<string>();
            var dirs = Directory.GetDirectories(lblSelectedPath.Text).ToList<string>();
            IterateTree(dirs);
            ZipFile.CreateFromDirectory(lblDestPath.Text, new DirectoryInfo(lblDestPath.Text).Parent.FullName + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip");
            MessageBox.Show("导出完成!");
        }

        private void IterateTree(List<string> nodes)
        {
            nodes.ForEach((y) =>
            {
                var childs = Directory.GetDirectories(y).ToList<string>();
                if (childs.Count != 0)
                    IterateTree(childs);

                var curr = new DirectoryInfo(y);
                contractList.ForEach((x) =>
                {
                    if (x == curr.Name && !finishedList.Contains(x))
                    {
                        
                        Copy(y, lblDestPath.Text + "\\" + curr.Name);
                        finishedList.Add(x);
                    }
                });
            });
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            //foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            //{
            //    DirectoryInfo nextTargetSubDir =
            //        target.CreateSubdirectory(diSourceSubDir.Name);
            //    CopyAll(diSourceSubDir, nextTargetSubDir);
            //}
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LastContacts = textBox1.Text;
            Properties.Settings.Default.Save();
        }
    }
}
