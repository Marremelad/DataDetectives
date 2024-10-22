namespace DataDetectives;
using System;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using System;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

class Program
{
    static void Main(string[] args)
    {
        string url = "https://www.myh.se/om-oss/sok-handlingar-i-vart-diarium?katalog=Tillsynsbeslut%20yrkesh%C3%B6gskoleutbildning";

        try
        {
            // Starta en ny Chrome-webbläsare i "headless" läge
            var options = new ChromeOptions();
            options.AddArgument("--headless"); // Kör utan att visa webbläsarens UI
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            using (var driver = new ChromeDriver(options))
            {
                // Navigera till webbsidan
                driver.Navigate().GoToUrl(url);

                // Vänta tills elementet vi behöver är tillgängligt
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until(d => d.FindElement(By.CssSelector("a.v-list-item")));

                // Hämta HTML-innehållet efter att JavaScript genererat sidan
                var htmlContent = driver.PageSource;

                // Ladda HTML-koden i HtmlDocument från HtmlAgilityPack
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

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
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ett fel inträffade: " + ex.Message);
        }
    }
}
