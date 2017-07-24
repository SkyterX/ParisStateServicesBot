using System;
using System.Threading;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.PageObjects;

namespace ParisStateServicesBot
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var loader = new BookingStatusLoader(new ChromeDriver()))
                Console.Out.WriteLine(loader.GetBookingStatus());
            Console.ReadKey();
        }
    }
}