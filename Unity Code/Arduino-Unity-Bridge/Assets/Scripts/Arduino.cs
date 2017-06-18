using UnityEngine;

public class Arduino : MonoBehaviour
{
    ArduinoSerial arduinoSerial;

    void Start()
    {
        Debug.Log("trying to open that Serial Com");

        arduinoSerial = new ArduinoSerial();
        arduinoSerial.ConnectToArduino(baudRate: 9600);
    }

    void Update()
    {
        Debug.Log("current line is: " + arduinoSerial.GetCurrentInputString());
    }

    void OnApplicationQuit()
    {
        arduinoSerial.ClosePort();            
    }
}