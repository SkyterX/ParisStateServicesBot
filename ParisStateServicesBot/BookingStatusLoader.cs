using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ParisStateServicesBot
{
    public class BookingStatusLoader : IDisposable
    {
        private readonly IWebDriver webDriver;

        public BookingStatusLoader(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
        }

        public string GetBookingStatus()
        {
            webDriver.Navigate().GoToUrl("http://www.hauts-de-seine.gouv.fr/booking/create/9489/0");

            var cookiesBannerXPath = "//div[@id='cookies-banner']/div/a[.='Accepter']";
            var cookiesBanner = webDriver.FindElement(By.XPath(cookiesBannerXPath));
            cookiesBanner?.Click();
            new WebDriverWait(webDriver, TimeSpan.FromSeconds(5))
                .Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath(cookiesBannerXPath)));

            webDriver.FindElement(By.Id("condition")).Click();
            webDriver.FindElement(By.Name("nextButton")).Click();

            webDriver.FindElement(By.Id("planning9491")).Click();
            webDriver.FindElement(By.Name("nextButton")).Click();

            return webDriver.FindElement(By.Id("inner_Booking")).Text;
        }

        public void Dispose()
        {
            webDriver?.Dispose();
        }
    }
}