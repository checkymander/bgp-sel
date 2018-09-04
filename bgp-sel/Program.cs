using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace bgp_sel
{
    class Program
    {
        static void Main(string[] args)
        {

            ChromeOptions co = new ChromeOptions();
            co.AddArgument("--headless");
            Console.WriteLine(Directory.GetCurrentDirectory());
            //IWebDriver driver = new ChromeDriver(@"C:\Users\scott\source\repos\bgp-sel\packages\ChromeDriver\",co);
            IWebDriver driver = new ChromeDriver(Directory.GetCurrentDirectory(), co);
            List<string> toParse = new List<string>();
           
            driver.Navigate().GoToUrl("https://bgp.he.net/search?search%5Bsearch%5D=sony&commit=Search");
            driver.Manage().Window.Maximize();
            string output = driver.FindElement(By.TagName("html")).Text;
            var links = driver.FindElements(By.TagName("a"));
            var pattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
            var r = new Regex(pattern);
            foreach (var link in links)
            {
                Match match = r.Match(link.Text);
                if (link.Text.Contains("AS"))
                {
                    toParse.Add(link.Text);
                    Console.WriteLine("Adding toParse {0}", link.Text);
                   
                }
                else if (match.Success)
                {
                    Console.WriteLine("IPv4 found: {0}", link.Text);
                }
            }

            if(toParse.Count > 0)
            {
                foreach (var link in toParse)
                {
                    var newURL = "https://bgp.he.net" + link;
                    driver.Navigate().GoToUrl(newURL);
                    var newlinks = driver.FindElements(By.TagName("a"));
                    foreach (var url in newlinks)
                    {
                        Match match = r.Match(url.Text);
                        if (match.Success)
                        {
                            Console.WriteLine("IPv4 found: {0}", url.Text);
                        }

                    }
                }
            }
        }
    }
}
