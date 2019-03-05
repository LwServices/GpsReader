using NmeaParser;
using NmeaParser.Nmea.Gps;
using System;
using System.IO.Ports;
using System.Runtime.InteropServices;

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
            //_serialPort.Open();
            return _serialPort != null;
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
            switch (e.Message)
            {
                case Gpgsa gpgsa:
                    Console.WriteLine(gpgsa.GpsMode);
                    return;

                case Gpgll gpgll:
                    Console.WriteLine(gpgll.FixTime);
                    return;

                case Gprmc gprmc:
                    Console.WriteLine(gprmc.Course);
                    return;

                case Gpgga gpgga:
                    Console.WriteLine(gpgga.Hdop);
                    return;

                case Gpgsv gpgsv:
                    Console.WriteLine(gpgsv.SVsInView);
                    return;
            }

            var msg = e.Message;
            Console.WriteLine($"Type of : {msg.GetType()}");
            //Console.ReadKey();
        }
    }
}