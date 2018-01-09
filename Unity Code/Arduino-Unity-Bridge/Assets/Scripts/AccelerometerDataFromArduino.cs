using System.Globalization;
using UnityEngine;

public class AccelerometerDataFromArduino : MonoBehaviour
{
    public GameObject go;
    private ArduinoSerial arduinoSerial;

    void Start()
    {
        arduinoSerial = new ArduinoSerial();
        arduinoSerial.ConnectToArduino(baudRate: 115200);
    }

    void Update()
    {
        if (arduinoSerial.ReadInput("rawx") == null)
        {
            return;
        }

        float rawx = float.Parse(arduinoSerial.ReadInput("rawx"), CultureInfo.InvariantCulture.NumberFormat);
        float rawy = float.Parse(arduinoSerial.ReadInput("rawy"), CultureInfo.InvariantCulture.NumberFormat);
        float rawz = float.Parse(arduinoSerial.ReadInput("rawz"), CultureInfo.InvariantCulture.NumberFormat);
        Debug.Log("rawx: " + rawx);
        Debug.Log("rawy: " + rawy);
        Debug.Log("rawz: " + rawz);

        float raz = Mathf.Atan2(-rawy, rawx);
        float rax = Mathf.Atan2(-rawy, rawz);

        go.transform.rotation = Quaternion.RotateTowards(go.transform.rotation, 
            Quaternion.Euler( (rax * 180 / Mathf.PI) - 90, 0f, -((raz * 180 / Mathf.PI) - 90) ), Time.deltaTime * 500);
    }

    void OnApplicationQuit()
    {
        arduinoSerial.ClosePort();            
    }
}