using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;

public class ArduinoSerial
{
    String currentInput = "";
    SerialPort port;
    List<String> availablePorts = new List<String>();

    /// <summary>
    /// Starts serial communication with the arduino.
    /// </summary>
    /// <param name="baudRate">
    /// the baud rate must be the same as the one setup on the arduino.
    /// will use 9600 if nothing specified.
    /// </param>
    public void ConnectToArduino(int baudRate = 9600)
    {
        SafeAction(()=>GetPortNames());

        // just try to use the first port in the list. Needs to be fixed in the future.
        SafeAction(() => InitializeArduino(availablePorts[0], baudRate));

        Thread serialPortListenerThread = new Thread(RecieveDataInHelperThread);
        serialPortListenerThread.Start();
    }

    /// <summary>
    /// Closes the serial port and also shuts down all related helper threads.
    /// </summary>
    public void ClosePort()
    {
        SafeAction(() => port.Close(), false);
    }

    /// <summary>
    /// Returns the most currently recieved line. 
    /// </summary>
    /// <returns>the most currently recieved line.</returns>
    public String GetCurrentInputString()
    {
        lock (currentInput)
        {
            String str = currentInput;
        }
        return currentInput; 
    }

    void InitializeArduino(String listeningPort, int baudRate)
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
            Debug.Log("port opened");
        });
    }

    // This will run in the helper thread.
    void RecieveDataInHelperThread()
    {
        while (port.IsOpen)
        {
            String str = port.ReadLine();
            lock (currentInput)
            {
                currentInput = str;
            }
            // Sleeping 20ms means 50 updates per second.
            // Or less. Depending on how many times per second the arduino sends a line.
            // port.ReadLine() is blocking.
            Thread.Sleep(20);
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
        string[] ports = SerialPort.GetPortNames();

        foreach (string port in ports)
        {
            Debug.Log("found port: " + port);
            availablePorts.Add(port);
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