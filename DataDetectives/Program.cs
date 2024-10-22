using System.Diagnostics;

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
        Stopwatch sw = Stopwatch.StartNew();
        Stopwatch.StartNew();
        
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        
        var htmlContent1 = "";
        using (var driver = new ChromeDriver(options))
        {
            string url = $"https://www.myh.se/om-oss/sok-handlingar-i-vart-diarium?katalog=Tillsynsbeslut%20yrkesh%C3%B6gskoleutbildning";
            driver.Navigate().GoToUrl(url);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(d => d.FindElement(By.CssSelector("a.v-list-item")));

            htmlContent1 = driver.PageSource;
        }

        var htmlDoc1 = new HtmlDocument();
        htmlDoc1.LoadHtml(htmlContent1);

        var buttons = htmlDoc1.DocumentNode.SelectNodes("//button[contains(@aria-label, 'Goto Page')]");
        int numberOfPages = int.Parse(buttons[^1].InnerText.Trim());

        try
        {
            List<Thread> pageParsers = new List<Thread>();

            for (int i = 0; i < 8; i++)
            {
                Thread thread = new Thread(() => PageParser.ParsePage(numberOfPages));
                pageParsers.Add(thread);
                thread.Start();
            }
            
            foreach (Thread thread in pageParsers)
            {
                thread.Join();
            }
            
            var pages = PageParser.Pages;
            var htmlDoc = new HtmlDocument();
            
            foreach (var page in pages)
            {
                htmlDoc.LoadHtml(page);

                // Extrahera information från varje listobjekt (<a>-element)
                var button = htmlDoc.DocumentNode.SelectSingleNode("//button[contains(@aria-label, 'Current Page')]");
                if (button != null)
                {
                    Console.WriteLine($"Page {button.InnerText.Trim()}.");
                }
                else
                {
                    Console.WriteLine("Page number not found.");
                }


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
                            Console.WriteLine("Aktör: " + actorNode.InnerText.Trim() + "\n");
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

                Console.WriteLine("-----------------------------------\n\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ett fel inträffade: " + ex.Message);
        }

        sw.Stop();
        Console.WriteLine($"Time elapsed attempting to scrape {numberOfPages} pages: {sw.ElapsedMilliseconds} ms.");
        Console.WriteLine($"Pages scraped: {PageParser.Pages.Count}.\n");
    }
}
