using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ParisStateServicesBot
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var webDriver = new ChromeDriver())
            {
                webDriver.Navigate().GoToUrl("http://www.hauts-de-seine.gouv.fr/booking/create/9489/0");
                webDriver.FindElement(By.XPath("//div[@id='cookies-banner']/div/a[.='Accepter']"))?.Click();
                Thread.Sleep(1000);
                webDriver.FindElement(By.Id("condition")).Click();
                webDriver.FindElement(By.Name("nextButton")).Click();

                webDriver.FindElement(By.Id("planning9491")).Click();
                webDriver.FindElement(By.Name("nextButton")).Click();

                var bookingText = webDriver.FindElement(By.Id("inner_Booking")).Text;
                Console.Out.WriteLine(bookingText);
            }
            Console.ReadKey();
        }
    }
}