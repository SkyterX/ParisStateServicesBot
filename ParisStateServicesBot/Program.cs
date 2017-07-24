using System;

namespace ParisStateServicesBot
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var factory = new TheFactory())
            {
                var bookingStatus = factory.BookingStatusLoader.GetBookingStatus();
                Console.Out.WriteLine(bookingStatus);
            }
            Console.ReadKey();
        }
    }
}