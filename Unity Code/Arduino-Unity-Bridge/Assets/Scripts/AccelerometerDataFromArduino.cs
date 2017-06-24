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

        float rax = float.Parse(arduinoSerial.ReadInput("rax"), CultureInfo.InvariantCulture.NumberFormat);
        float raz = float.Parse(arduinoSerial.ReadInput("raz"), CultureInfo.InvariantCulture.NumberFormat);
        Debug.Log("rax: " + rax);
        Debug.Log("raz: " + raz);

        go.transform.rotation = Quaternion.RotateTowards(go.transform.rotation, 
            Quaternion.Euler( (rax * 180 / Mathf.PI) - 90, 0f, -((raz * 180 / Mathf.PI) - 90) ), Time.deltaTime * 500);
    }

    void OnApplicationQuit()
    {
        arduinoSerial.ClosePort();            
    }
}