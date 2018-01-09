#include "Encoder.h"

// 9 and 10 are inputs
Encoder Encoder1(9, 10);

void setup() {
  Serial.begin(115200);

  // - - - - - - - - - - - - - - - - - - -
  // - - - - -  SET UP TIMER 1 - - - - - -
  // - - - - - - - - - - - - - - - - - - - 
  noInterrupts();               // disable all interrupts
  TCCR1A = 0;
  TCCR1B = 0;
  TCNT1  = 0;

  OCR1A = 3;                   // compare match register 16MHz/256/20kHz
  TCCR1B |= (1 << WGM12);      // CTC mode
  TCCR1B |= (1 << CS12);       // 256 prescaler 
  TIMSK1 |= (1 << OCIE1A);     // enable timer compare interrupt

  interrupts();                // enable all interrupts
}

// - - - - - - - - - - - - - - - - - - -
// - - TIMER 1 INTERRUPT FUNCTION  - - -
// - - - - - every 60 us - - - - - - - -
// - - - - - - - - - - - - - - - - - - - 
ISR(TIMER1_COMPA_vect)
{
  // ENCODER READ FUNCTION COMES HERE!
  Encoder1.update();
  digitalWrite(7, digitalRead(7) ^ 1);   // toggle pin 7
}

void loop() {
  serialPrintf("val:%d", Encoder1.count);
      
  //send all 20ms. Thus, 50 times per second.
  delay(20);
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
