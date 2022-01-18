using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;
using NmeaParser;
using NmeaParser.Messages;

namespace GpsReader
{
    internal static class Program
    {
        private static void Main()
        {


            ConcurrentStack<string> stack = new ConcurrentStack<string>();
            GetPorts().ForEach(Console.WriteLine);


            var portName ="COM4";
            var baudRate = 9600;
            #region Read Gps

            using var serialPort = new SerialPort(portName, baudRate);
            using var device = new SerialPortDevice(serialPort);
            device.MessageReceived += (sender, e) =>
            {
                switch (e.Message.MessageType)
                {
                    case "GPGLL":
                        if (e.Message is Gll gll)
                        {
                            //Console.WriteLine(gll.ToString());
                            stack.Push(gll.ToString());
                        }
                        else
                        {
                            Debugger.Break();
                        }
                        return;
                    case "GPRMC":
                        if (e.Message is Rmc rmc)
                        {
                            //Console.WriteLine(rmc.ToString());
                            stack.Push(rmc.ToString());
                        }
                        else
                        {
                            Debugger.Break();
                        }
                        return;
                    case "GPVTG":
                        if (e.Message is Vtg vtg)
                        {
                            //Console.WriteLine(vtg.ToString());
                            stack.Push(vtg.ToString());
                        }
                        else
                        {
                            Debugger.Break();
                        }
                        return;
                    case "GPGGA":
                        if (e.Message is Gga gga)
                        {
                            //Console.WriteLine(gga.ToString());
                            stack.Push(gga.ToString());
                        }
                        else
                        {
                            Debugger.Break();
                        }
                        return;
                    case "GPGSA":
                        if (e.Message is Gsa gsa)
                        {
                            //Console.WriteLine(gsa.ToString());
                            stack.Push(gsa.ToString());
                        }
                        else
                        {
                            Debugger.Break();
                        }
                        return;
                    case "GPGSV":
                        if (e.Message is Gsv gsv)
                        {
                            //Console.WriteLine(gsv.ToString());
                            stack.Push(gsv.ToString());
                        }
                        else
                        {
                            Debugger.Break();
                        }
                        return;
                    default:
                        var type = e.Message.MessageType;
                        Console.WriteLine($"Type of : {type}");
                        return;
                }

            };
            var task = device.OpenAsync();
            Console.WriteLine("Running");


            #endregion

            #region Console Write Task
            
            var cts = new CancellationTokenSource();
            var consoleTask=Task.Factory.StartNew(() => ConsoleWriter(cts.Token, stack));
            #endregion
            
            // Shutdown
            Console.WriteLine("press to exit");
            Console.ReadKey();
            device.CloseAsync();
            cts.Cancel();
            consoleTask.Wait();
            Console.WriteLine("alles ende");
            
        }

        private static void ConsoleWriter(CancellationToken token, ConcurrentStack<string> stack)
        {
            while (!token.IsCancellationRequested)
            {
                if (!stack.IsEmpty)
                {
                    var ok = stack.TryPop(out string bla);
                    if (ok)
                        Console.WriteLine(bla);
                }

                Thread.Sleep(500);
                Console.WriteLine($"Count ......................{stack.Count}");
            }

            Console.WriteLine("ende reader");
        }

        public static List<string> GetPorts()
        {
            var portNames = SerialPort.GetPortNames();
            return portNames.ToList();
        }
    }
}