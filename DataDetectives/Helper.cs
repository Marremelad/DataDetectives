namespace DataDetectives;
using System;
using HtmlAgilityPack;

public static class Output
{


    public static void DisplayPages(List<string> pages)
    {
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
}