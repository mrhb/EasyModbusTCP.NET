// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

using System.Net;

using System.Threading;
using System.Linq;

using System.ComponentModel;
using EasyModbus;
using System.Net.Sockets;
using System.Text;
using OnlineMonitoringLog.Drivers.Types;

namespace OnlineMonitoringLog.Drivers.ModbusTCP
{
    public class ModbusTCPUnitMOck 
    {

        static Thread ConnectionThread;
        private Thread ReadDataThread;
        static List<ModbusTCPUnitMOck> obj = new List<ModbusTCPUnitMOck>();
        private ModbusClient _modbusClient;
        public ModbusClient modbusClient {
            get { return _modbusClient; }
            set
                {
                _modbusClient = value;
                }
            }
        static ModbusTCPUnitMOck()
        {
            ConnectionThread = new Thread(() => StartListening());
            ConnectionThread.Name = "ModbusTCP";
            ConnectionThread.IsBackground = true;

        }

        public ModbusTCPUnitMOck(int unitId, IPAddress ip) 
        {
            if (ConnectionThread.ThreadState != ThreadState.Running)
                try { ConnectionThread.Start(); } catch { }
            ID = unitId;
            obj.Add(this);

            ReadDataThread = new Thread(() => ProcessUa());
            ReadDataThread.Name = "ModbusTCPReadData_" + unitId.ToString();
            ReadDataThread.IsBackground = true;
            ReadDataThread.Start();

         
        }

        private void ProcessUa()
        {
            while (true)
            {
                //if (modbusClient == null) { Thread.Sleep(2000); break; }
                try
                {
                    //ModbusClient modbusClient = new ModbusClient("192.168.1.110", 502);    //Ip-Address and Port of Modbus-TCP-Server
                    //modbusClient.Connect();                                                    //Connect to Server
                    //modbusClient.WriteMultipleCoils(4, new bool[] { true, true, true, true, true, true, true, true, true, true });    //Write Coils starting with Address 5
                    //                                                                                                                  //bool[] readCoils = modbusClient.ReadCoils(9, 10);                        //Read 10 Coils from Server, starting with address 10
                    while (true)
                    {

                        int[] readHoldingRegisters = modbusClient.ReadHoldingRegisters(0, 12);    //Read 10 Holding Registers from Server, starting with Address 1
                        var datas = new Dictionary<string, object>();
                        // Console Output

                        for (int i = 1; i < readHoldingRegisters.Length; i++)
                        {
                            Console.WriteLine($"Id:{modbusClient.UnitIdentifier}   Value of HoldingRegister " + (i + 1) + " " + readHoldingRegisters[i].ToString());
                          
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception c)
                {
                    Console.WriteLine($"Error Occured in \"ProcessUa\" at {ReadDataThread.Name}");
                }

                Thread.Sleep(2000);
            }


        }



        public static ManualResetEvent allDone = new ManualResetEvent(false);



        public static void StartListening()
        {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse("192.168.1.19");// ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 503);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(1);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    Thread.Sleep(5000);
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }
      

        public int ID { get; private set; }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            Console.WriteLine($"socket conncetion to {handler.RemoteEndPoint} has established");
            // Create the state object.  
       

            var modbusClient = new ModbusClient(handler);

            modbusClient.UnitIdentifier = Convert.ToByte(0);
            int unitId = modbusClient.ReportSlaveID();
            Console.WriteLine($"{handler.RemoteEndPoint} has UnitId:  " + unitId.ToString());

            try
            {
                obj.Where(a => a.ID == unitId).First().modbusClient = modbusClient;
                //for (int j = 1; j == 1; j++)
                //{


                //        obj[unitnum].modbusClient = modbusClient; unitnum++;
                //        // modbus.UnitIdentifier = Convert.ToByte(j);
                //    //    int[] serverResponse = modbusClient.ReadHoldingRegisters(1, 10);


                //    //for (int i = 0; i < serverResponse.Length; i++)
                //    //{
                //    //    Console.WriteLine($"data from {handler.RemoteEndPoint} recieved: {serverResponse[i]} ");
                //    //}

                //    Thread.Sleep(500);

                //}

                //    modbus.ReadHoldingRegisters(1, 10);

            }
            catch
            {
                handler.Close();
                //unitnum--;
                Console.WriteLine("AcceptCallback Error");
            }

        }


    }
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }


}

