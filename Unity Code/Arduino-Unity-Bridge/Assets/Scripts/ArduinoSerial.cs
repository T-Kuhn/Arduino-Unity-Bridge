using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;
using UniRx;

public class ArduinoSerial
{
    SerialPort port;
    Thread serialPortListenerThread;
    bool dataRecieved;
    int baud;
    Stack<string> availablePorts = new Stack<string>();
    // Note: A ConcurrentDict<Tkey, TValue> would be ideal.
    // But it can not be used on unity's current mono enviroment.
    Dictionary<string, string> recievedValues = new Dictionary<string, string>();
    readonly object lockObject = new object();

    /// <summary>
    /// Starts serial communication with the arduino.
    /// </summary>
    /// <param name="baudRate">
    /// the baud rate must be the same as the one setup on the arduino.
    /// will use 9600 if nothing specified.
    /// </param>
    public void ConnectToArduino(int baudRate = 9600)
    {
        baud = baudRate;

        Observable.Timer(System.TimeSpan.FromSeconds(5), System.TimeSpan.FromSeconds(5))
            .Subscribe(_ => CheckConnection());
        SafeAction(()=>GetPortNames());
        Connect();
    }

    void Connect()
    {
        if (serialPortListenerThread != null)
        {
            serialPortListenerThread.Abort();
            ClosePort();
        }

        if (availablePorts != null && availablePorts.Count > 0)
        {
            var portName = availablePorts.Pop();
            // just try to use the first port in the list. Needs to be fixed in the future.
            SafeAction(() => InitializeArduino(portName, baud));

            serialPortListenerThread = new Thread(RecieveDataInHelperThread);
            serialPortListenerThread.Start();
        }
        else
        {
            SafeAction(()=>GetPortNames());
        }
    }

    void CheckConnection()
    {
        if (dataRecieved)
        {
            Debug.Log("connection is OK.");
        }
        else
        {
            Debug.Log("reconnect");
            Connect();
        }

        dataRecieved = false;
    }
    // a quick memo:
    // here's how the thing should behave:
    // 1. as soon as the connect thing get's called, get the names of all the serial ports.
    // 2. connect using the first in the list.
    // 3. do the thing for 1 second.
    // 4. look if there was good incoming data in that period.
    //    if "yes":
    //    schedule a new 1-second check.
    //    if "no":
    //      check if there are any ports available.
    //      if "yes":
    //        close old port and connect to next available port.
    //      if "no":
    //        get new ports Stack and connect to the first port in there.

    /// <summary>
    /// Closes the serial port and also shuts down all related helper threads.
    /// </summary>
    public void ClosePort()
    {
        Debug.Log("close port");
        SafeAction(() => port.Close(), false);
    }

    /// <summary>
    /// Returns the most currently recieved line. 
    /// </summary>
    /// <returns>the most currently recieved line.</returns>
    public string ReadInput(string key)
    {
        string str;
        lock (lockObject)
        {
            if (recievedValues.ContainsKey(key))
            {
                dataRecieved = true;
                str = recievedValues[key];
            }
            else
            {
                str = null;
            }
        }
        return str; 
    }

    void InitializeArduino(string listeningPort, int baudRate)
    {
        Debug.LogFormat("try to connect to Arduino on port {0} with a baudrate of: {1}.", listeningPort, baudRate);
        SafeAction(() =>
            {
            port = new SerialPort(listeningPort, baudRate);
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.DataBits = 8;
            port.Handshake = Handshake.None;

            // NOTE: The "DataRecieved" Event isn't implemented. Thus we can not use it.
            // port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            port.Open();
        });
    }

    // This will run in the helper thread.
    void RecieveDataInHelperThread()
    {
        while (port.IsOpen)
        {
            // We expect a string like this: "time:123,anotherValue:4829.333,aString:string".
            String str = port.ReadLine();
            var subStrings = str.Split(',');
            lock (lockObject)
            {
                foreach(var ss in subStrings)
                {
                    var pair = ss.Split(':');
                    recievedValues[pair[0]] = pair[1];
                }
            }
            // Sleeping 20ms means 50 updates per second.
            // Or less. Depending on how many times per second the arduino sends a line.
            // port.ReadLine() is blocking.
            Thread.Sleep(19);
        }
    }

    // Run an action with try & catch.
    void SafeAction(Action action, bool message = true)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            if (message)
            {
                Debug.Log("exeption: " + ex.Message);
            }
        }
    }

    // Gets a list of all serial port names.
    void GetPortNames()
    {
        availablePorts = new Stack<string>();
        string[] ports = SerialPort.GetPortNames();

        foreach (string port in ports)
        {
            Debug.Log("found port: " + port);
            availablePorts.Push(port);
        }
    }

    // Send text to the serial port.
    void SendText(String str)
    {
        SafeAction(() =>
        {
            port.Write(str);
        });
    }
}