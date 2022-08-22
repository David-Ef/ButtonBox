/*
  Adapted from:
    AnalogReadSerial

    Reads an analog input on pin 0, prints the result to the Serial Monitor.
    Graphical representation is available using Serial Plotter (Tools > Serial Plotter menu).
    Attach the center pin of a potentiometer to pin A0, and the outside pins to +5V and ground.

    This example code is in the public domain.

    http://www.arduino.cc/en/Tutorial/AnalogReadSerial
*/

void setup() {
  Serial.begin(9600);

  // Left button
  pinMode(B10, INPUT_PULLUP);
  // Right button
  pinMode(4, INPUT_PULLUP);
  // Slider
  pinMode(A7, INPUT);//_PULLUP);
}

void loop() {
  // read the input on analog pin 0:
  int Button1Value = digitalRead(B10);
  int Button2Value = digitalRead(4);
  int SliderValue = analogRead(A7);
  // print out the value you read:
  Serial.print(Button1Value);
  Serial.print("-");
  Serial.print(Button2Value);
  Serial.print("-");
  Serial.println((float)SliderValue/1023.);
  delay(10);
}
