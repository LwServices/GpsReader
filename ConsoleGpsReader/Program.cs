using NmeaParser;
using NmeaParser.Nmea;
using NmeaParser.Nmea.Gps;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGpsReader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sw = new SerialWrapper();
            sw.InitPort(9600);
            sw.InitGps();

            Console.ReadKey();
        }
    }

    public class SerialWrapper
    {
        private const int BaudRate = 9600;
        private SerialPort _serialPort;

        public SerialWrapper()
        {
        }

        public bool InitPort(int baudRate)
        {
            var portNames = SerialPort.GetPortNames();
            if (portNames.Any())
            {
                var firstPort = portNames[0];
                Console.WriteLine($"First port is {firstPort}");
                return InitPort(firstPort, BaudRate);
            }
            Console.WriteLine("No Serial Ports Found");

            return false;
        }

        public bool InitPort(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, BaudRate);
            return _serialPort.IsOpen;
        }

        public bool InitGps()
        {
            var device = new SerialPortDevice(_serialPort);
            device.MessageReceived += DeviceOnMessageReceived;
            device.OpenAsync();

            return true;
        }

        private void DeviceOnMessageReceived(object sender, NmeaMessageReceivedEventArgs e)

        {
            if (e.Message is Gpgsa gpgsa)
            {
                Console.WriteLine(gpgsa.GpsMode);
            }

            if (e.Message is Gpgll gpgll)
            {
                Console.WriteLine(gpgll.FixTime);
            }

            if (e.Message is Gprmc gprmc)
            {
                Console.WriteLine(gprmc.Course);
            }

            if (e.Message is Gpgga gpgga)
            {
                Console.WriteLine(gpgga.Hdop);
            }
        }
    }
}