using System.Globalization;
using UnityEngine;

public class RotaryDataFromArduino : MonoBehaviour
{
    public GameObject go;
    ArduinoSerial arduinoSerial;
    Quaternion startRot;

    void Start()
    {
        arduinoSerial = new ArduinoSerial();
        arduinoSerial.ConnectToArduino(baudRate: 115200);
        startRot = go.transform.rotation;
    }

    void Update()
    {
        if (arduinoSerial.ReadInput("val") == null)
        {
            return;
        }

        float value = float.Parse(arduinoSerial.ReadInput("val"), CultureInfo.InvariantCulture.NumberFormat);

        //go.transform.rotation = Quaternion.RotateTowards(go.transform.rotation, 
        //   Quaternion.Euler( startRot.eulerAngles.x, startRot.eulerAngles.y, value / 600f * 360f ), Time.deltaTime * 500);
        go.transform.rotation = Quaternion.Euler(startRot.eulerAngles.x, startRot.eulerAngles.y, value / 600f * 360f);
    }

    void OnApplicationQuit()
    {
        arduinoSerial.ClosePort();            
    }
}