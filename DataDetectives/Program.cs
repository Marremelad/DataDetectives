namespace DataDetectives;
using System;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Thread pageParser1 = new Thread(PageParser.ParsePage);
            Thread pageParser2 = new Thread(PageParser.ParsePage);
            
            pageParser1.Start();
            pageParser2.Start();

            pageParser1.Join();
            pageParser2.Join();

            var pages = PageParser.Pages;
            var htmlDoc = new HtmlDocument();
            
            foreach (var page in pages)
            {
                htmlDoc.LoadHtml(page);

                // Extrahera information från varje listobjekt (<a>-element)
                var listItemNodes = htmlDoc.DocumentNode.SelectNodes("//a[contains(@class, 'v-list-item') and contains(@class, 'v-list-item--link')]");
                if (listItemNodes != null)
                {
                    foreach (var listItem in listItemNodes)
                    {
                        // Extrahera diarienummer
                        var diaryNumberNode = listItem.SelectSingleNode(".//div[contains(@class, 'v-list-item__subtitle') and contains(@class, 'letter-space-2')]");
                        if (diaryNumberNode != null)
                        {
                            Console.WriteLine("Diarienummer: " + diaryNumberNode.InnerText.Trim());
                        }
                        else
                        {
                            Console.WriteLine("Inget diarienummer hittades.");
                        }

                        // Extrahera granskningsinformation
                        var reviewNode = listItem.SelectSingleNode(".//div[contains(@class, 'v-list-item__title') and contains(@class, 'myh-h3')]");
                        if (reviewNode != null)
                        {
                            Console.WriteLine("Granskning: " + reviewNode.InnerText.Trim());
                        }
                        else
                        {
                            Console.WriteLine("Ingen granskningsinformation hittades.");
                        }

                        // Extrahera aktörsnamn
                        var actorNode = listItem.SelectSingleNode(".//span[contains(@class, 'v-card') and contains(@class, 'text--primary') and contains(@class, 'myh-body-2')]");
                        if (actorNode != null)
                        {
                            Console.WriteLine("Aktör: " + actorNode.InnerText.Trim());
                        }
                        else
                        {
                            Console.WriteLine("Ingen aktör hittades.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Inga listobjekt hittades.");
                }

                Console.WriteLine("\n\n\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ett fel inträffade: " + ex.Message);
        }
    }
}
