namespace DataDetectives;
using System;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

public static class PageParser
{
    public static HashSet<int> SearchedNumbers = new HashSet<int>();
    public static List<string> Pages = new List<string>();
    private static readonly object LockObject = new object();


    public static void ParsePage()
    {
        // string url = $"https://www.myh.se/om-oss/sok-handlingar-i-vart-diarium?katalog=Tillsynsbeslut%20yrkesh%C3%B6gskoleutbildning&p={i}";
        // Starta en ny Chrome-webbläsare i "headless" läge
        var options = new ChromeOptions();
        options.AddArgument("--headless"); // Kör utan att visa webbläsarens UI
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        
        using (var driver = new ChromeDriver(options))
        {
            for (int i = 1; i < 11; i++)
            {
                lock (LockObject)
                {
                    if (!SearchedNumbers.Add(i)) continue;
                }
                
                string url = $"https://www.myh.se/om-oss/sok-handlingar-i-vart-diarium?katalog=Tillsynsbeslut%20yrkesh%C3%B6gskoleutbildning&p={i}";
                // Navigera till webbsidan
                driver.Navigate().GoToUrl(url);

                // Vänta tills elementet vi behöver är tillgängligt
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until(d => d.FindElement(By.CssSelector("a.v-list-item")));

                // Hämta HTML-innehållet efter att JavaScript genererat sidan
                var htmlContent = driver.PageSource;

                lock (LockObject)
                {
                    Pages.Add(htmlContent);    
                }
            }
        }
    }
}





// &p=1