// install this library to your Arduino libraries folder:
// https://github.com/jarzebski/Arduino-MPU6050
#include <MPU6050.h>

MPU6050 mpu;

void setup() {
  Serial.begin(115200);
  
  // address 0x69 if ADD is pulled up to VCC
  //         0x68 if ADD is pulled down to GND
  mpu.begin(MPU6050_SCALE_2000DPS, MPU6050_RANGE_2G, 0x68);
  mpu.setDLPFMode(MPU6050_DLPF_6);
}

void loop() {

  sendRawValsToUnity();
  //sendRotationValuesToUnity();
  
  //send all 20ms. Thus, 50 times per second.
  delay(20);
}

float sendRawValsToUnity()
{
    Vector AccelVector = mpu.readRawAccel();
    serialPrintf("rawx:%d,rawy:%d,rawz:%d", (int)AccelVector.XAxis, (int)AccelVector.YAxis, (int)AccelVector.ZAxis);
}

float sendRotationValuesToUnity()
{
    Vector AccelVector = mpu.readRawAccel();
    float rotationAroundZAxis = (float)atan2(-AccelVector.YAxis, AccelVector.XAxis);
    float rotationAroundXAxis = (float)atan2(-AccelVector.YAxis, AccelVector.ZAxis);
    serialPrintf("raz:%f,rax:%f", rotationAroundZAxis, rotationAroundXAxis);
}

// Printing Values to the Serial port.
// serialPrintf("someInt:%d,someFloat:%f,someString:%s", someInt, someFloat, someString);
int serialPrintf(char *str, ...) {
    int i, j, count = 0;

    va_list argv;
    va_start(argv, str);
    for(i = 0, j = 0; str[i] != '\0'; i++) {
        if (str[i] == '%') {
            count++;

            Serial.write(reinterpret_cast<const uint8_t*>(str+j), i-j);

            switch (str[++i]) {
                case 'd': Serial.print(va_arg(argv, int));
                    break;
                case 'l': Serial.print(va_arg(argv, long));
                    break;
                case 'f': Serial.print(va_arg(argv, double));
                    break;
                case 'c': Serial.print((char) va_arg(argv, int));
                    break;
                case 's': Serial.print(va_arg(argv, char *));
                    break;
                case '%': Serial.print("%");
                    break;
                default:;
            };

            j = i+1;
        }
    };
    va_end(argv);

    if(i > j) {
        Serial.write(reinterpret_cast<const uint8_t*>(str+j), i-j);
    }

    Serial.println("");
    return count;
}
