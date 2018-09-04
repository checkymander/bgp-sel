using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using Mono.Options;
using OpenQA.Selenium.Chrome;

namespace bgp_sel
{
    class Program
    {
        static void Main(string[] args)
        {
            string format = "";
            bool help = false;
            string search="";
            string searchurl;
            OptionSet options = new OptionSet()
                .Add("?:|help:|h:", "Prints out the options", option => help = true)
                .Add("f:|format:", "Output Format: txt, csv, terminal", f => format = f)
                .Add("s|search=", "String to search", s => search = s);
            options.Parse(args);
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

            if (help)
            {
                displayHelp("Usage: bgp-sel.exe --search \"searchterm\"", options);
                Environment.Exit(0);
            }
            if (string.IsNullOrEmpty(search))
            {

                Console.Write("Enter Search Term (Don't forget double quotes!): ");
                search = Uri.EscapeDataString(Console.ReadLine());
                searchurl = "https://bgp.he.net/search?search%5Bsearch%5D=" + search + "&commit=Search";

            }
            else
            {
                searchurl = "https://bgp.he.net/search?search%5Bsearch%5D=" + Uri.EscapeDataString(search) + "&commit=Search";
            }
            if (string.IsNullOrEmpty(format))
            {
                format = "terminal";
            }
            Console.WriteLine("Searching {0}", searchurl);

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
                }
                else if (match.Success)
                {
                    IPs.Add(link.Text);
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
                            IPs.Add(url.Text);
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
        public static void displayHelp(string message, OptionSet o)
        {
            Console.Error.WriteLine(message);
            o.WriteOptionDescriptions(Console.Error);
            Environment.Exit(-1);
        }
    }
}
