using System;

namespace ConsoleGpsReader
{
    internal static class Program
    {
        private static void Main()
        {
            var serialWrapper = new SerialWrapper();
            if (serialWrapper.InitPort())
            {
                serialWrapper.InitGps();
            }
            else
            {
                Console.WriteLine("No SerialPort's found");
            }

            Console.ReadKey();
        }
    }
}