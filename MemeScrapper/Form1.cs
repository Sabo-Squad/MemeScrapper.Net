using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemeScrapper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button1_click(object sender, EventArgs e)
        {
            string outputPath = textBox1.Text;

            if (Directory.Exists(outputPath))
            {

                IConfiguration config = AngleSharp.Configuration.Default.WithDefaultLoader();
                string address = GetAppSettings("url");
                IBrowsingContext context = BrowsingContext.New(config);
                IDocument document = await context.OpenAsync(address);

                using (StreamWriter sw = new StreamWriter(outputPath + @"\MemeHtml.txt", true))
                {
                    sw.Write(document.DocumentElement.OuterHtml);
                }
            } else
            {
                MessageBox.Show("Please enter a valid path and try again.", "Invalid Path",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public string GetAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
