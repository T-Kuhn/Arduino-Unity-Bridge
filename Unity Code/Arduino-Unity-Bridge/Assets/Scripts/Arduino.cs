using UnityEngine;

public class Arduino : MonoBehaviour
{
    ArduinoSerial arduinoSerial;

    void Start()
    {
        arduinoSerial = new ArduinoSerial();
        arduinoSerial.ConnectToArduino(baudRate: 9600);
    }

    void Update()
    {
        Debug.Log("someInt: " + arduinoSerial.ReadInput("someInt"));
        Debug.Log("someFloat: " + arduinoSerial.ReadInput("someFloat"));
        Debug.Log("someString: " + arduinoSerial.ReadInput("someString"));
    }

    void OnApplicationQuit()
    {
        arduinoSerial.ClosePort();            
    }
}