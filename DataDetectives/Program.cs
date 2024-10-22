namespace DataDetectives;
using System;
using System.Diagnostics;
using HtmlAgilityPack;

class Program
{
    static void Main()
    {
        Stopwatch sw = Stopwatch.StartNew();
        
        int numberOfPages = Helper.GetNumberOfPages();

        try
        {
            sw.Start();
            
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
            
            sw.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something went wrong: {ex}");
        }
        
        var sortedPages = PageParser.Pages.OrderBy(pageHtml =>
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageHtml);
                
            var pageButton = htmlDoc.DocumentNode.SelectSingleNode("//button[@aria-current='true']");
            return pageButton != null ? int.Parse(pageButton.InnerText.Trim()) : 0;
        }).ToList();
        
        Helper.DisplayPages(sortedPages);
        
        Console.WriteLine($"Time elapsed attempting to scrape {numberOfPages} pages: {sw.ElapsedMilliseconds} ms.");
        Console.WriteLine($"Pages scraped: {PageParser.Pages.Count}.\n");
    }
}
