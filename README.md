# bgp-sel
A tool written in C# that makes use of chromedriver in order to scrape search contents from bgp.he.net. Returns a list of IPv4 addresses that can be piped into nmap in order to make service discovery a little bit easier.

# Requirements
Download the most recent version of ChromeDriver from <a href="http://chromedriver.chromium.org/downloads">here</a> and place it in the same folder as the bgp-sel.exe.

Project built for .NET Framework 4.6.1 which can be found <a href="https://www.microsoft.com/en-us/download/details.aspx?id=49981">here</a>

# Usage
bgp-sel.exe "searchterm"

Note: If you want to make use of negative search terms such as -"not this" you can either just run bgp-sel.exe or esecape the quotation marks (e.g. bgp-sel.exe "this -\"not this\"")

#Future Plans
Add command line arguments, and reporting options.
