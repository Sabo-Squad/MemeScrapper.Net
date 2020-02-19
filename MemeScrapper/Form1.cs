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
using System.Threading;
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
                List<Meme> memes = new List<Meme>();
                string address = GetAppSettings("url");
                //setup for scraper
                IConfiguration config = AngleSharp.Configuration.Default.WithDefaultLoader();
                IBrowsingContext context = BrowsingContext.New(config);

                //Getting the largest page number as memes are paginated
                int maxPage = await GetMaxPagesFromParentHTML(address, context);

                for (int i = 1; i <= 2; i++)
                {
                    //Looping through each page of memes, getting the URL for each meme and then accessing that url to get data on an individual meme and creating an object for it
                    address = GetAppSettings("url") + "/page/" + i;
                    IDocument document = await context.OpenAsync(address);
                    foreach(IElement element in document.All.Where(x => x.LocalName == "a" && x.GetAttribute("class") == "photo"))
                     {
                        //Getting the URL for the individual meme
                        string memeUrl = GetAppSettings("url").Replace(@"/memes", "") + element.GetAttribute("href");
                        IDocument memeDocument = await context.OpenAsync(memeUrl);

                        List<string> tagsList = new List<string>();

                        foreach(IElement ele in memeDocument.All.Where(x => x.LocalName == "a" && x.ParentElement.LocalName == "dd" && x.ParentElement.ParentElement.Id == "entry_tags"))
                        {
                            tagsList.Add(ele.InnerHtml);
                        }
                        //Creating a meme
                        memes.Add(new Meme
                        {
                            Name = memeDocument.QuerySelectorAll("a").FirstOrDefault(x => x.ParentElement.ParentElement.ClassName == "info wide").InnerHtml,
                            URL = memeUrl,
                            Views = int.Parse(memeDocument.QuerySelectorAll("a").FirstOrDefault(x => x.ParentElement.ClassName == "views").InnerHtml.Replace(",", "")),
                            Tags = tagsList.ToArray()
                        });
                        Thread.Sleep(2000);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid path and try again.", "Invalid Path",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task<int> GetMaxPagesFromParentHTML(string address, IBrowsingContext context)
        {
            int maxPage = 0;

            IDocument document = await context.OpenAsync(address);

            //Getting each a tag under div of class pagination
            foreach (IElement element in document.All.Where(x => x.LocalName == "a" && x.ParentElement.ClassList.Contains("pagination")))
            {
                int number;
                //if the innerhtml(pagenumber) is parsable, compare it to maxPage and assign if greater
                if (int.TryParse(element.InnerHtml, out number))
                    maxPage = maxPage > number ? maxPage : number;
            }

            //returning the largest number (last page)
            return maxPage;
        }
    }
}
