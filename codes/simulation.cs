/*
Código para OBR simulação 2026
Autores: Maurício Calvet, Lucas Benjamin, Huan Lin Fui e Arthur Martins
*/

//Definição de motores de movimento
Servomotor motorLF = Bot.GetComponent<Servomotor>("motorLF");
Servomotor motorRF = Bot.GetComponent<Servomotor>("motorRF");
Servomotor motorLB = Bot.GetComponent<Servomotor>("motorLB");
Servomotor motorRB = Bot.GetComponent<Servomotor>("motorRB");
Servomotor armLeft = Bot.GetComponent<Servomotor>("armLeft");
Servomotor armRight = Bot.GetComponent<Servomotor>("armRight");
Servomotor motorBag = Bot.GetComponent<Servomotor>("motorBag");
Servomotor handLeft = Bot.GetComponent<Servomotor>("handLeft");
Servomotor handRight = Bot.GetComponent<Servomotor>("handRight");

//Definição dos sensores de cor
ColorSensor colorLL = Bot.GetComponent<ColorSensor>("colorLL");
ColorSensor colorL = Bot.GetComponent<ColorSensor>("colorL");
ColorSensor colorR = Bot.GetComponent<ColorSensor>("colorR");
ColorSensor colorRR = Bot.GetComponent<ColorSensor>("colorRR");


//Variáveis para o PID
double error; 
double p; 
double kp = 3.5; 
double speed = 175;
double force = 500;

//Ponto onde o código é executado
async Task Main() {
    lockMotors(false);
    lockArms(false);
    upArms(500, 150);
    await Time.Delay(2000);
    lockArms(true);
    run(500, 200, 200);
    await Time.Delay(500);
    while(true) {
        await Time.Delay(1);
        IO.Print(error.ToString());
        error =  (colorRR.Analog.Brightness*1.5 + colorR.Analog.Brightness)/2 - (colorL.Analog.Brightness + colorLL.Analog.Brightness*1.5)/2;
        p = error*kp;
        run(force, speed + p, speed - p);
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

//Função  para mover os motores do resgate em determinada direção
void upArms(double force, double speed){
    armLeft.Apply(force, speed);
    armRight.Apply(force, speed);
}