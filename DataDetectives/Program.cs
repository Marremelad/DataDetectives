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
        string url = "https://www.myh.se/om-oss/sok-handlingar-i-vart-diarium?katalog=Tillsynsbeslut%20yrkesh%C3%B6gskoleutbildning";

        try
        {
            // Starta en ny Chrome-webbläsare i "headless" läge
            var options = new ChromeOptions();
            options.AddArgument("--headless"); // Kör i headless-läge för att inte visa webbläsarens UI
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            using (var driver = new ChromeDriver(options))
            {
                // Navigera till URL
                driver.Navigate().GoToUrl(url);

                // Vänta tills sidan är helt laddad
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until(d => d.FindElement(By.CssSelector("a.v-list-item"))); // Vänta tills ett specifikt element finns på sidan

                // Hämta HTML-innehållet från den renderade sidan
                string htmlContent = driver.PageSource;

                // Använd HtmlAgilityPack för att analysera HTML-innehållet
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                // Extrahera länkar från HTML-innehållet
                var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
                
                if (linkNodes != null)
                {
                    foreach (var link in linkNodes)
                    {
                        // Skriv ut länkens URL och text
                        string hrefValue = link.GetAttributeValue("href", "");
                        string linkText = link.InnerText.Trim();

                        Console.WriteLine($"Länk: {hrefValue}");
                        Console.WriteLine($"Text: {linkText}");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Inga länkar hittades.");
                }
            }
        }
        catch (WebDriverException e)
        {
            Console.WriteLine($"Ett fel uppstod: {e.Message}");
        }
    }
}