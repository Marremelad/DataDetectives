namespace DataDetectives;
using System;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

public static class PageParserAsync
{
    private static readonly HashSet<int> ScrapedPages = new HashSet<int>(); 
    public static readonly List<string> Pages = new List<string>();
    private static readonly object LockObject = new object();

    public static async void ParsePageAsync(int numberOfPages)
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        

        await Task.Run(() =>
        {
            
            using (var driver = new ChromeDriver(options))
            {
                for (int i = 1; i < numberOfPages - 1; i++)
                {
                    lock (LockObject)
                    {
                        if (!ScrapedPages.Add(i)) continue;
                    }
                    
                    string url = $"https://www.myh.se/om-oss/sok-handlingar-i-vart-diarium?katalog=Tillsynsbeslut%20yrkesh%C3%B6gskoleutbildning&p={i}";
                    driver.Navigate().GoToUrl(url);

                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    wait.Until(d => d.FindElement(By.CssSelector("a.v-list-item")));

                    var htmlFromPage = driver.PageSource;
                    
                    lock (LockObject)
                    {
                        Pages.Add(htmlFromPage);
                    }
                }
            }
            
        });
    }
}
