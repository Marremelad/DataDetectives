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
        int numberOfPages = int.Parse(buttons[buttons.Count - 1].InnerText.Trim());
        
        using (var driver = new ChromeDriver(options))
        {
            for (int i = 1; i < numberOfPages + 1; i++)
            {
                lock (LockObject)
                {
                    if (!SearchedNumbers.Add(i)) continue;
                }
                
                string url = $"https://www.myh.se/om-oss/sok-handlingar-i-vart-diarium?katalog=Tillsynsbeslut%20yrkesh%C3%B6gskoleutbildning&p={i}";
                driver.Navigate().GoToUrl(url);
                
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until(d => d.FindElement(By.CssSelector("a.v-list-item")));
                
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