/*
Código para OBR simulação 2026
Autores: Maurício Calvet, Lucas Benjamin, Huan Lin Fui e Arthur Martins
*/

//Definição de motores de movimento
Servomotor motorLF = Bot.GetComponent<Servomotor>("motorLF");//Left Front
Servomotor motorRF = Bot.GetComponent<Servomotor>("motorRF");//Right Front
Servomotor motorLB = Bot.GetComponent<Servomotor>("motorLB");//Left Back
Servomotor motorRB = Bot.GetComponent<Servomotor>("motorRB");//Right Back
Servomotor armLeft = Bot.GetComponent<Servomotor>("armLeft");//Braço esquerdo da garra
Servomotor armRight = Bot.GetComponent<Servomotor>("armRight");//Braço direito da garra
Servomotor motorBag = Bot.GetComponent<Servomotor>("motorBag");//Portão da "caçamba"
Servomotor handLeft = Bot.GetComponent<Servomotor>("handLeft");//Mão esquerda da garra
Servomotor handRight = Bot.GetComponent<Servomotor>("handRight");//Mão direta da garra

//Definição dos sensores de cor
ColorSensor colorLL = Bot.GetComponent<ColorSensor>("colorLL");//Sensor da extrema esquerda
ColorSensor colorL = Bot.GetComponent<ColorSensor>("colorL");//Sensor da esquerda
ColorSensor colorR = Bot.GetComponent<ColorSensor>("colorR");//Sensor da direita
ColorSensor colorRR = Bot.GetComponent<ColorSensor>("colorRR");//Sensor da extrema direita
//Variáveis usadas nas leituras dos sensores de cor
string corL, corLL, corRR, corR;

//Definição dos sensores ultrassônicos
UltrasonicSensor ultraLeft = Bot.GetComponent<UltrasonicSensor>("ultraLeft");//Sensor da esquerda
UltrasonicSensor ultraRight = Bot.GetComponent<UltrasonicSensor>("ultraRight");//Sensor da direita
UltrasonicSensor ultraFront1 = Bot.GetComponent<UltrasonicSensor>("ultraFront1");//Sensor da frente esquerda
UltrasonicSensor ultraFront2 = Bot.GetComponent<UltrasonicSensor>("ultraFront2");//Sensor da frente direita
//Variáveis usadas nas leituras dos sensores ultrassônicos
double distanceF1, distanceF2, distanceR, distanceL;


//Variáveis para o PID
double error; 
double p; 
double kp = 3.5; 
double speed = 175;
double force = 500;

//Variável para necessária para utilização da bússola
double compass;

//Ponto onde o código é executado
async Task Main() {
    lockMotors(false);
    lockArms(false);
    upArms(500, 150);
    await Time.Delay(2000);
    lockArms(true);
    //run(500, 200, 200);
    //await Time.Delay(500);
    while(true) {
        await Time.Delay(1);
        GetColors();
        GetDistance();
        lockMotors(false);
        if (verde_A())
        {
            run(500, 200, 200);
            await Time.Delay(250);
            GetColors();
            if (verde_L())
            {   
                run(500, 200, 200);
                await Time.Delay(750);
                await gyrate(90);
                lockMotors(false);
                run(500, 200, 200);
                await Time.Delay(200);
            }
            if (verde_R())
            {
                run(500, 200, 200);
                await Time.Delay(750);
                await gyrate(-90);
                lockMotors(false);
                run(500, 200, 200);
                await Time.Delay(200);
            }
        }
        else if (distanceF1 <= 2 || distanceF2 <=2)
        {
            GetDistance();
            lockMotors(false);
            run(300, 150, 150);
            await Time.Delay(350);
            await gyrate(90);
            lockMotors(false);
            run(300, 150, 150);
            await Time.Delay(500);
            while (lineM() == false)
            {   
                await Time.Delay(50);
                GetColors();
                motorLF.Apply(500, 50);
                motorLB.Apply(500, 50);
                motorRF.Apply(500, 350);
                motorRB.Apply(500, 350);
            }
            run(500, 200, 200);
            await Time.Delay(200);
            lockMotors(false);
            await gyrate(80);
            run(300, -200, -200);
            await Time.Delay(500);
            lockMotors(false);
        }
        else{
            IO.Print(error.ToString());
            error =  (colorRR.Analog.Brightness*1.5 + colorR.Analog.Brightness)/2 - (colorL.Analog.Brightness + colorLL.Analog.Brightness*1.5)/2;
            p = error*kp;
            run(force, speed + p, speed - p);
        }
    }
}

//Função para definir o estado dos motores de movimento(travado ou destravado)
void lockMotors(bool key = true) {
    motorLF.Locked = key;
    motorLB.Locked = key;
    motorRF.Locked = key;
    motorRB.Locked = key;
}

//Função para definir o estado dos motores de resgate(travado ou destravado)
void lockArms(bool key = true) {
    armLeft.Locked = key;
    armRight.Locked = key;
}

//Função  para mover os motores de movimento em determinada direção
void run(double force, double speed, double speedR) {
    motorLF.Apply(force, speed);
    motorLB.Apply(force, speed);
    motorRF.Apply(force, speedR);
    motorRB.Apply(force, speedR);
}
//Função para mover os 4 motores de forma que faça ele se mover para a direita
void right(double force, double speed)
{
    motorLF.Apply(force, speed);
    motorLB.Apply(force, speed);
    motorRF.Apply(force, -speed);
    motorRB.Apply(force, -speed);
}
//Função para mover os 4 motores de forma que faça ele se mover para a esquerda
void left(double force, double speed)
{
    motorLF.Apply(force, -speed);
    motorLB.Apply(force, -speed);
    motorRF.Apply(force, speed);
    motorRB.Apply(force, speed);
}
//Função  para mover os motores do resgate em determinada direção
void upArms(double force, double speed){
    armLeft.Apply(force, speed);
    armRight.Apply(force, speed);
}
//Função para detectar o verde
bool isgreen(string sensor) {
  Color cor = Bot.GetComponent<ColorSensor>(sensor).Analog;
  double red = cor.Red;
  double green = cor.Green;
  double blue = cor.Blue;
  if (green * 0.9 > red && green * 0.9 > blue) {
    return true;
  } else {
    return false;
  }
}
//Localização do Verde
//Verde na direita
bool verde_R() {
  if (isgreen("colorRR") || isgreen("colorR")) {
    return true;
  } else {
    return false;
  }
}
//Verde na esquerda
bool verde_L() {
  if (isgreen("colorLL") || isgreen("colorL")) {
    return true;
  } else {
    return false;
  }
}
//Verde em qualquer um dos 4 sensores
bool verde_A() {
  if (isgreen("colorLL") || isgreen("colorL") || isgreen("colorR") || isgreen("colorRR"))  {
    return true;
  } else {
    return false;
  }
}
//Função usada para pegar o valor da bússola
double getCompass() {
  compass = Bot.Compass;
  return compass;
}
//Função usada para girar o robô X graus para direita ou X graus para esquerda
async Task gyrate(double angle, double speed = 200) {
  double value = getCompass();
  double focus = value + angle;

  if (focus > 360) {
    focus = focus - 360;
    IO.Print("Girando");
  } else if (focus < 0) {
    focus = 360 + focus;
    IO.Print("Hi");
  }
  double focusMarginP = focus + 6;  //5
  double focusMarginM = focus - 6;  //5
  while ((getCompass() <= focusMarginM || getCompass() >= focusMarginP)) {
    await Time.Delay(50);
    lockMotors(false);
    getCompass();
    if (angle > 0) {
      right(500, 175);
    } else {
      left(500, 175);
    }
    await Time.Delay(50);
  }
  IO.Print("saiu");
  lockMotors(true);
}
//Função usada para receber as cores lidas pelos sensores de cor
void GetColors() {
  corLL = Bot.GetComponent<ColorSensor>("colorLL").Analog.ToString();
  corL = Bot.GetComponent<ColorSensor>("colorL").Analog.ToString();
  corR = Bot.GetComponent<ColorSensor>("colorR").Analog.ToString();
  corRR = Bot.GetComponent<ColorSensor>("colorRR").Analog.ToString();
}
//Função usada para receber as distâncias lidas pelos sensores ultrassônicos 
void GetDistance() {
  distanceF1 = ultraFront1.Analog;
  distanceF2 = ultraFront2.Analog;
  distanceR = ultraRight.Analog;
  distanceL = ultraLeft.Analog;
  if (distanceF1 == -1) {
    distanceF1 = 999;
  }
  if (distanceF2 == -1) {
    distanceF2 = 999;
  }
  if (distanceR == -1) {
    distanceR = 999;
  }
  if (distanceL == -1) {
    distanceL = 999;
  }

}
//Função usada para saber quando o robô está mais ou menos no meio da linha
bool lineM() {
  if (corL == "Preto") {
    return true;
  } else {
    return false;
  }
}
