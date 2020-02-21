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
using System.Net;
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
                //setup for scraper
                IConfiguration config = AngleSharp.Configuration.Default.WithDefaultLoader();
                IBrowsingContext context = BrowsingContext.New(config);
                IDocument document = await context.OpenAsync(GetAppSettings("url"));

                //Getting the largest page number as memes are paginated
                int maxPage = document.QuerySelectorAll("div.pagination a").Where(x => !x.ClassList.Contains("next_page")).Select(x => int.Parse(x.InnerHtml)).Max();

                for (int i = 1; i <= 2; i++)
                {
                    //Looping through each page of memes, getting the URL for each meme and then accessing that url to get data on an individual meme and creating an object for it
                    string address = GetAppSettings("url") + "/page/" + i;
                    IDocument pageDocument = await context.OpenAsync(address);
                    foreach(IElement element in pageDocument.QuerySelectorAll("td a.photo"))
                     {
                        try
                        {
                            byte[] imageBytes;
                            List<string> tagsList = new List<string>();
                            //Getting the URL for the individual meme
                            string memeUrl = GetAppSettings("url").Replace(@"/memes", "") + element.GetAttribute("href");
                            IDocument memeDocument = await context.OpenAsync(memeUrl);

                            //Getting the bytearray of the meme image to be stored in the object
                            using (WebClient webClient = new WebClient())
                            {
                                imageBytes = webClient.DownloadData(memeDocument.QuerySelector("header .photo.left.wide").GetAttribute("href"));
                            }
                            //Creating a meme
                            //This will be replaced by adding the meme to a database instead of just to a list.
                            memes.Add(new Meme
                            {
                                //Name = memeDocument.QuerySelectorAll("a").FirstOrDefault(x => x.ParentElement.ParentElement.ClassName == "info wide").InnerHtml,
                                Name = memeDocument.QuerySelector("section.info.wide h1 a").InnerHtml,
                                URL = memeUrl,
                                Views = int.Parse(memeDocument.QuerySelector(".views a").InnerHtml.Replace(",", "")),
                                Tags = memeDocument.QuerySelectorAll("#entry_tags dd a").Select(x => x.InnerHtml).ToArray(),
                                ImageByteArray = imageBytes
                            });
                            Console.WriteLine(memes.LastOrDefault().ToString());
                            Thread.Sleep(5000);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Ran into an error " + ex);
                        }
                        
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid path and try again.", "Invalid Path",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
