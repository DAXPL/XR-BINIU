#include <WiFi.h>
#include <ESPAsyncWebServer.h>
#include <ArduinoJson.h>
#include <Wire.h>
#include <Adafruit_ADXL345_U.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BMP280.h>
//CZUJNIK TEMPERATURY
#include <DHT.h>
#define DHTPIN 21
#define DHTTYPE DHT11
//CZUJNIK ULTRADZWIEKOWY
#define TRIG1 25
#define ECHO1 26
#define TRIG2 27
#define ECHO2 14
#define TRIG3 33
#define ECHO3 13

Adafruit_BMP280 bmp;
Adafruit_ADXL345_Unified accel = Adafruit_ADXL345_Unified(123);

// Inicjalizacja I2C na pinach innych niż domyślne
TwoWire myWire(0); // I2C port 0

DHT dht(DHTPIN, DHTTYPE);
unsigned long lastSensorSend = 0;
const unsigned long sensorInterval = 2000;

bool lightDetected = false;

const char* ssid = "INEA-4383";
const char* password = "7PHTbE2G";

const int ENA = 4;
const int IN1 = 16;
const int IN2 = 17;

const int ENB = 5;
const int IN3 = 18;
const int IN4 = 19;

const int pwmChannelA = 0;
const int pwmChannelB = 1;

AsyncWebServer server(80);
AsyncWebSocket ws("/");

void setupPWM() {
  ledcSetup(pwmChannelA, 1000, 8);
  ledcAttachPin(ENA, pwmChannelA);
  
  ledcSetup(pwmChannelB, 1000, 8);
  ledcAttachPin(ENB, pwmChannelB);
}
//DO KONTROLOWANIA STEROWNOSCI MOZLIWOSC DO POPRAWY
void handleControlCommand(float powerLeft, float powerRight) {
  if (powerLeft > 0 && powerRight > 0) {
    //PRZOD
    digitalWrite(IN1, HIGH);
    digitalWrite(IN2, LOW);
    digitalWrite(IN3, HIGH);
    digitalWrite(IN4, LOW);
    ledcWrite(pwmChannelA, int(powerLeft * 255));
    ledcWrite(pwmChannelB, int(powerRight * 255));
  } else if (powerLeft < 0 && powerRight < 0) {
    //TYL
    digitalWrite(IN1, LOW);
    digitalWrite(IN2, HIGH);
    digitalWrite(IN3, LOW);
    digitalWrite(IN4, HIGH);
    ledcWrite(pwmChannelB, int(-powerRight * 255));
    ledcWrite(pwmChannelA, int(-powerLeft * 255));
  } else if (powerLeft < 0 && powerRight > 0) {
    //LEWO
    digitalWrite(IN1, HIGH);
    digitalWrite(IN2, LOW);
    digitalWrite(IN3, LOW);
    digitalWrite(IN4, LOW);
    ledcWrite(pwmChannelB, int(powerRight * 255));
    ledcWrite(pwmChannelA, int(-powerLeft * 255));
  }else if (powerLeft > 0 && powerRight < 0) {
    //PRAWO
    digitalWrite(IN1, LOW);
    digitalWrite(IN2, LOW);
    digitalWrite(IN3, HIGH);
    digitalWrite(IN4, LOW);
    ledcWrite(pwmChannelB, int(-powerRight * 255));
    ledcWrite(pwmChannelA, int(powerLeft * 255));
  }else {
    digitalWrite(IN1, LOW);
    digitalWrite(IN2, LOW);
    ledcWrite(pwmChannelA, 0);
    digitalWrite(IN3, LOW);
    digitalWrite(IN4, LOW);
    ledcWrite(pwmChannelB, 0);
  }
}

void stopMotors() {
  digitalWrite(IN1, LOW);
  digitalWrite(IN2, LOW);
  digitalWrite(IN3, LOW);
  digitalWrite(IN4, LOW);
  ledcWrite(pwmChannelA, 0);
  ledcWrite(pwmChannelB, 0);
}

void onWebSocketEvent(AsyncWebSocket *server,
                      AsyncWebSocketClient *client,
                      AwsEventType type,
                      void *arg,
                      uint8_t *data,
                      size_t len) {
  if (type == WS_EVT_DATA) {
    AwsFrameInfo *info = (AwsFrameInfo*)arg;
    if (info->final && info->index == 0 && info->len == len && info->opcode == WS_TEXT) {
      String jsonString = String((char*)data);
      Serial.println("JSON odebrany:");
      Serial.println(jsonString);

      DynamicJsonDocument doc(256);
      DeserializationError error = deserializeJson(doc, jsonString);

      if (!error) {
        float powerLeft = doc["power"]["left"];
        float powerRight = doc["power"]["right"];
        Serial.printf("Moc: left = %.2f, right = %.2f\n", powerLeft, powerRight);
        handleControlCommand(powerLeft, powerRight);
      } else {
        Serial.println("Błąd parsowania JSON!");
      }
    }
  }
}

long readUltrasonic(int trigPin, int echoPin) {
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);

  long duration = pulseIn(echoPin, HIGH, 30000);
  long distance = duration * 0.034 / 2;

  return (duration == 0) ? -1 : distance;
}

void setupUltrasonicPins() {
  pinMode(TRIG1, OUTPUT);
  pinMode(ECHO1, INPUT);

  pinMode(TRIG2, OUTPUT);
  pinMode(ECHO2, INPUT);

  pinMode(TRIG3, OUTPUT);
  pinMode(ECHO3, INPUT);
}

void setup() {
Serial.begin(115200);

  setupPWM();        
  stopMotors();      

  dht.begin();

  pinMode(22, INPUT); // Fotorezystor
  setupUltrasonicPins();
  myWire.begin(34, 35); // SDA = 34, SCL = 35

  // Inicjalizacja BMP280
  if (!bmp.begin(0x76)) {
    Serial.println("Nie wykryto BMP280");
  }

  // Inicjalizacja ADXL345
  if (!accel.begin()) {
    Serial.println("Nie wykryto ADXL345");
  }

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Łączenie z WiFi...");
  }

  Serial.println("Połączono z WiFi");
  Serial.println(WiFi.localIP());

  ws.onEvent(onWebSocketEvent);
  server.addHandler(&ws);
  server.begin();

  Serial.println("WebSocket uruchomiony");

  pinMode(IN1, OUTPUT);
  pinMode(IN2, OUTPUT);
  pinMode(IN3, OUTPUT);
  pinMode(IN4, OUTPUT);

}

void loop() {
if (millis() - lastSensorSend > sensorInterval) {
    lastSensorSend = millis();

    float temp = dht.readTemperature();
    float hum = dht.readHumidity();
    int lightValue = digitalRead(22);
    lightDetected = (lightValue == HIGH);
    long dist1 = readUltrasonic(TRIG1, ECHO1);
    long dist2 = readUltrasonic(TRIG2, ECHO2);
    long dist3 = readUltrasonic(TRIG3, ECHO3);
    sensors_event_t event;
    accel.getEvent(&event);
    float temperature88 = bmp.readTemperature();
    float pressure = bmp.readPressure();
    DynamicJsonDocument doc(512);
    JsonObject sensor = doc.createNestedObject("sensor");

      doc["sensor"]["temperature"] = temp;
      doc["sensor"]["humidity"] = hum;
      doc["sensor"]["lightT"] = lightDetected;
      doc["sensor"]["distance"]["front"] = dist1;
      doc["sensor"]["distance"]["left"] = dist2;
      doc["sensor"]["distance"]["right"] = dist3;
      doc["sensor"]["pressure"] = pressure;
      doc["sensor"]["alX"] = event.acceleration.x;
      doc["sensor"]["alY"] = event.acceleration.y;
      doc["sensor"]["alZ"] = event.acceleration.z;
      //JsonObject barometer = sensor.createNestedObject("barometer");
      //barometer["temperature"] = temperature88;
      //barometer["pressure"] = pressure;
      //Serial.println(temperature88);
      //Serial.println(pressure);
      //JsonObject acceleration = sensor.createNestedObject("acceleration");
      //acceleration["x"] = event.acceleration.x;
      //acceleration["y"] = event.acceleration.y;
      //acceleration["z"] = event.acceleration.z;
      String json;
      serializeJson(doc, json);
      ws.textAll(json);

      Serial.println("Wysłano dane do Unity:");
      Serial.println(json);  
      delay(500);

  }}
