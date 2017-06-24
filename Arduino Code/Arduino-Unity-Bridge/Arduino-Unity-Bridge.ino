// Some variables we want to send to Unity
int someInt = 128;
float someFloat = 512.256f;
char* someString = "astring";

void setup() {

    // Open Serial Port
    Serial.begin(9600);

}

void loop() {

    // wating for 10ms.  
    delay(20);
    serialPrintf("someInt:%d,someFloat:%f,someString:%s", someInt, someFloat, someString);

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

