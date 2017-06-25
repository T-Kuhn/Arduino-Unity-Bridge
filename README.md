# Arduino-Unity-Bridge
Easy to use Serial Port communication Unity <-> Arduino.

## Necessary settings on Unity
We need to open a serial port from within the mono enviroment. The mono Enviroment Unity uses normally is a stripped down one. But this can be changed via Edit -> Project Settings -> Player. Look for a Setting called "Api Compatibility Level" and set it to ".NET 2.0".

## Simple Example
### On the Arduino
We put data on the Serial bus like this. 
```C++
// Some variables we want to send to Unity
int someInt = 128;
float someFloat = 512.256f;
char* someString = "astring";

void setup() {
    // Open Serial Port
    Serial.begin(9600);
}

void loop() {
    delay(20);
    serialPrintf("someInt:%d,someFloat:%f,someString:%s", someInt, someFloat, someString);
}
```
Note that `serialPrintf()` is just a helper function converting different types to string. See "Arduino-Unity-Bridge.ino" for the full code (in the "Arduino Code" directory).

### On Unity

Open the Arduino-Unity-Bridge project and hit play. You should see the transmitted values in the debug log.
There is a GameObject with a script called "Arduino" on it. It contains the following code.

```C#
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
```

Values from the Arduino are read by creating an instance of the ArduinoSerial class, setting it up with the same baud rate as specified on the arduino, and then calling `arduinoSerial.ReadInput("someInt")` on it. The argument "someInt" has to match with the text set on the Arduino like here : `serialPrintf("someInt:%d,someFloat:%f,someString:%s", someInt, someFloat, someString);`. Note how the key value pairs are separated by "," and the keys and values themselves are separated by ":" with no spaces in between. 
