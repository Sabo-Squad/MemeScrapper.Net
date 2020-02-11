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
using static MemeScrapper.Helper; 

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
                //Getting the largest page number as memes are paginated
                int maxPages = await GetMaxPagesFromParentHTML();

                Console.WriteLine(maxPages);

            } else
            {
                MessageBox.Show("Please enter a valid path and try again.", "Invalid Path",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        public async Task<int> GetMaxPagesFromParentHTML()
        {
            List<int> pageNums = new List<int>();

            IConfiguration config = AngleSharp.Configuration.Default.WithDefaultLoader();
            string address = GetAppSettings("url");
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(address);

            //Getting each a tag under div of class pagination
            foreach (IElement element in document.All.Where(x => x.LocalName == "a" && x.ParentElement.ClassList.Contains("pagination")))
            {
                int number;
                //if the innerhtml(pagenumber) is parsable, add it to a list.
                if (int.TryParse(element.InnerHtml, out number))
                    pageNums.Add(int.Parse(element.InnerHtml));
            }

            //returning the largest number (last page)
            return pageNums.Max();
        }
    }
}
