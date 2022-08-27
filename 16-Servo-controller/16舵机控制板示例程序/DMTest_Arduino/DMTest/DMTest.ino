#include <Wire.h>
#include <DMREG.h>

void setup() 
{
  Serial.begin(9600);
}

void loop() 
{
  Serial.write(DM0_Speed1_Position_90, 10);
  delay(1000);
  Serial.write(DM0_Speed2_Position_0, 10);
  delay(1000);
  Serial.write(DM_Action2, 5);
  delay(2000);
}
