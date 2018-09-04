using System;
using System.Web;
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
            Console.WriteLine("Welcome to bgp-sel, a bgp.he.net scraper written in .NET! Searches for IPv4 addresses in order to assist Bounty Hunters");
            string searchurl;
            string search;
            //ToDo: Include report options, CSV,txt,IP's only, Descriptions etc etc.
            //Command Line Options to support
            //Currently only supports IPv4 - Need to add IPv6 regex.
            ChromeOptions co = new ChromeOptions();
            co.AddArgument("--headless");
            co.AddArgument("log-level=3");
            co.AddArgument("--disable-extensions");
            IWebDriver driver = new ChromeDriver(Directory.GetCurrentDirectory(), co);
            List<string> toParse = new List<string>();
            List<string> IPs = new List<string>();
            if (args.Length==0)
            {

                Console.Write("Enter Search Term (Don't forget double quotes!): ");
                search = Uri.EscapeDataString(Console.ReadLine());
                searchurl = "https://bgp.he.net/search?search%5Bsearch%5D=" + search + "&commit=Search";

            }
            else
            {
                searchurl = "https://bgp.he.net/search?search%5Bsearch%5D=" + Uri.EscapeDataString(args[1]) + "&commit=Search";
            }

            Console.WriteLine("Searching {0}", searchurl);
            Console.ReadKey();

            driver.Navigate().GoToUrl(searchurl);
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
                    //Console.WriteLine("Adding toParse {0}", link.Text);
                   
                }
                else if (match.Success)
                {
                    IPs.Add(link.Text);
                    //Console.WriteLine(link.Text);
                    //Console.WriteLine("IPv4 found: {0}", link.Text);
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
                            //Console.WriteLine(url.Text);
                            IPs.Add(url.Text);
                            // Console.WriteLine("IPv4 found: {0}", url.Text);
                        }

                    }
                }
            }

            if(IPs.Count == 0)
            {
                Console.WriteLine("No IP Ranges Found");
            }
            else
            {
                foreach(var IP in IPs)
                {
                    Console.WriteLine(IP);
                }
            }
        }
    }
}
