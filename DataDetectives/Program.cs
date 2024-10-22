using System.Diagnostics;

namespace DataDetectives;
using System;
using HtmlAgilityPack;


class Program
{
    static async Task Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();

        try
        {

            List<Task> tasks =
            [
                new Task(PageParserAsync.ParsePageAsync),
                new Task(PageParserAsync.ParsePageAsync),
                new Task(PageParserAsync.ParsePageAsync),
                new Task(PageParserAsync.ParsePageAsync),
                new Task(PageParserAsync.ParsePageAsync),
                new Task(PageParserAsync.ParsePageAsync),
                new Task(PageParserAsync.ParsePageAsync),
                new Task(PageParserAsync.ParsePageAsync),
                new Task(PageParserAsync.ParsePageAsync),
                new Task(PageParserAsync.ParsePageAsync)
            ];
            

            // Wait for both tasks to finish.
            await Task.WhenAll(tasks);

            var pages = PageParserAsync.Pages;
            var htmlDoc = new HtmlDocument();
            
            foreach (var page in pages)
            {
                htmlDoc.LoadHtml(page);

                // Extract button info.
                var button = htmlDoc.DocumentNode.SelectSingleNode("//button[contains(@aria-label, 'Current Page')]");
                if (button != null)
                {
                    Console.WriteLine($"Page {button.InnerText.Trim()}");
                }
                else
                {
                    Console.WriteLine("Didn't find button.");
                }

                // Extract list item info.
                var listItemNodes = htmlDoc.DocumentNode.SelectNodes("//a[contains(@class, 'v-list-item') and contains(@class, 'v-list-item--link')]");
                if (listItemNodes != null)
                {
                    foreach (var listItem in listItemNodes)
                    {
                        // Extract diary number.
                        var diaryNumberNode = listItem.SelectSingleNode(".//div[contains(@class, 'v-list-item__subtitle') and contains(@class, 'letter-space-2')]");
                        if (diaryNumberNode != null)
                        {
                            Console.WriteLine("Diarienummer: " + diaryNumberNode.InnerText.Trim());
                        }
                        else
                        {
                            Console.WriteLine("Inget diarienummer hittades.");
                        }

                        // Extract review information.
                        var reviewNode = listItem.SelectSingleNode(".//div[contains(@class, 'v-list-item__title') and contains(@class, 'myh-h3')]");
                        if (reviewNode != null)
                        {
                            Console.WriteLine("Granskning: " + reviewNode.InnerText.Trim());
                        }
                        else
                        {
                            Console.WriteLine("Ingen granskningsinformation hittades.");
                        }

                        // Extract actor name.
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

        sw.Stop();
        Console.WriteLine($"tid för att skrapa 10 sidor: {sw.ElapsedMilliseconds} ms.");
    }
}
