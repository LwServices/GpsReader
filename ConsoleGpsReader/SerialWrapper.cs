using NmeaParser;
using NmeaParser.Nmea.Gps;
using System;
using System.IO.Ports;

namespace ConsoleGpsReader
{
    public class SerialWrapper
    {
        private const int BaudRate = 9600;
        private SerialPort _serialPort;

        public bool InitPort()
        {
            return InitPort(BaudRate);
        }

        public bool InitPort(int baudRate)
        {
            var portNames = SerialPort.GetPortNames();
            if (portNames.Length > 0)
            {
                var firstPort = portNames[0];
                Console.WriteLine($"First port is {firstPort}");
                return InitPort(firstPort, baudRate);
            }
            return false;
        }

        public bool InitPort(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate);
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