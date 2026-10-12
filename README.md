<div align="center">

# 🤖 OBR 2026 — Robô de Simulação

**Robô autônomo de resgate desenvolvido para a etapa de Simulação da Olimpíada Brasileira de Robótica 2026**

[![Plataforma](https://img.shields.io/badge/Plataforma-sBotics-00bcd4?style=for-the-badge&logo=unity&logoColor=white)](https://sbotics.net/)
[![Linguagem](https://img.shields.io/badge/Linguagem-C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/pt-br/dotnet/csharp/)
[![Licença](https://img.shields.io/badge/Licença-MIT-green?style=for-the-badge)](LICENSE)
[![OBR 2026](https://img.shields.io/badge/OBR-2026-blue?style=for-the-badge)](https://www.obr.org.br/)

</div>

---

## 📖 Sobre o projeto

Este repositório contém o código-fonte, as configurações de robô e as arenas utilizadas pela equipe na **etapa de Simulação da OBR 2026**, disputada na plataforma [sBotics](https://sbotics.net/).

O robô foi projetado para navegar de forma **totalmente autônoma** por arenas de resgate. Ele segue a linha preta usando um **controlador proporcional (P)**, detecta marcações verdes para realizar curvas programadas, desvia de obstáculos com **sensores ultrassônicos** e utiliza a **bússola integrada** para rotacionar com precisão. Na inicialização, os braços mecânicos são erguidos e travados, posicionando o robô para o início do percurso.

### ✨ Destaques técnicos

| Recurso | Detalhe |
|---|---|
| 🧮 Controle | Controlador proporcional (ganho `kp = 3.5`) |
| 🔵 Sensores de cor | 4 sensores (LL, L, R, RR) com ponderação lateral e detecção de verde |
| 📡 Ultrassônicos | 4 sensores (frente esquerda/direita, esquerda, direita) para desvio de obstáculos |
| 🧭 Bússola | Rotação precisa via `Bot.Compass` com margem de ±6° |
| ⚙️ Motores | 4 servomotores de tração + 2 braços (erguidos e travados na inicialização) |
| 🗺️ Arenas | 4 arenas customizadas de diferentes dificuldades |
| 🤖 Robôs | 2 configurações de robô (`.sBot`) |

---

## 📸 Demonstração

<div align="center">

![Robô navegando autonomamente na arena do sBotics](assets/demo.png)

*O robô percorre a arena de forma autônoma — **0:19 de tempo**, **5 pontos** marcados, rodando a 59 FPS*

</div>

---

## ⚙️ Pré-requisitos e Instalação

### Requisitos mínimos de sistema

- **Sistema operacional:** Windows ou Linux
- **RAM:** mínimo 4 GB
- **Processador:** moderno (geração recente, 64-bit)
- **Conta:** cadastro gratuito em [sbotics.net](https://sbotics.net/)

### Passo a passo de instalação

1. Acesse **[sbotics.net](https://sbotics.net/)** e crie sua conta gratuita.
2. Faça o download do instalador para o seu sistema operacional.
3. Execute o instalador e siga as instruções na tela.
4. Abra o sBotics e faça login com sua conta.

---

## 🚀 Como usar

### 1. Importar o robô

1. No sBotics, acesse a seção **Robôs**.
2. Clique em **Importar** e selecione um dos arquivos da pasta `robots/`:

| Arquivo | Descrição |
|---|---|
| `newestSbotics.sBot` | ✅ Versão mais recente — **recomendada** |
| `Werther.sBot` | Versão alternativa / experimental |

### 2. Importar uma arena

1. Acesse a seção **Arenas** e clique em **Importar**.
2. Escolha uma arena da pasta `arenas/` de acordo com a dificuldade desejada:

| Arena | Dificuldade | Descrição |
|---|---|---|
| `greenPlay.sArena` | ⭐ Iniciante | Arena principal de treino e validação |
| `90obstacle.sArena` | ⭐⭐ Médio | Curvas em 90° e obstáculos |
| `Myckey.sArena` | ⭐⭐⭐ Avançado | Percurso complexo e desafiador |
| `pesadelo_para_cinco.sArena` | ⭐⭐⭐⭐ Extremo | Alta dificuldade — teste de estresse |

### 3. Carregar o código

1. Com o robô posicionado na arena, abra o **Editor de Código** no sBotics.
2. Importe ou cole o conteúdo do arquivo `codes/simulation.cs`.
3. Clique em **Executar**.

### 4. Resultado esperado

O robô executa a seguinte sequência ao iniciar:

```
1. Motores de tração são desbloqueados
2. Braços mecânicos são desbloqueados, erguidos e travados na posição inicial
3. Loop principal — a cada ciclo o robô decide entre três comportamentos:
   a) Verde detectado → avança, identifica lado (esquerda/direita) e gira 90°
   b) Obstáculo à frente (distância ≤ 2) → desvia, reencosta na linha e reposiciona
   c) Linha normal → controle proporcional corrige trajetória continuamente
```

---

## 🧠 Como o código funciona

O arquivo [`codes/simulation.cs`](codes/simulation.cs) organiza o comportamento do robô em três camadas:

### 1. Seguimento de linha (controle P)

Quatro sensores de cor leem o brilho da pista em tempo real. O erro é calculado pela diferença ponderada entre os lados direito e esquerdo, e corrigido proporcionalmente:

```csharp
// Cálculo do erro — sensores externos têm peso 1.5x
error = (colorRR.Analog.Brightness * 1.5 + colorR.Analog.Brightness) / 2
      - (colorL.Analog.Brightness  + colorLL.Analog.Brightness * 1.5) / 2;

p = error * kp;   // kp = 3.5
run(force, speed + p, speed - p);
```

### 2. Detecção de verde e curvas

Quando qualquer sensor detecta a cor verde, o robô avança, identifica em qual lado está o verde e rotaciona 90° nessa direção usando a bússola:

```csharp
if (verde_A()) {
    // identifica lado e chama gyrate(90) ou gyrate(-90)
}
```

### 3. Desvio de obstáculos

Se os sensores frontais detectam um objeto a **≤ 2 unidades**, o robô desvia girando 90°, avança até reencostrar na linha (`lineM()`) e reposiciona:

```csharp
else if (distanceF1 <= 2 || distanceF2 <= 2) {
    // gira, avança, procura linha, reposiciona
}
```

### Parâmetros de controle

| Parâmetro | Valor | Função |
|---|---|---|
| `kp` | `3.5` | Ganho proporcional |
| `speed` | `175` | Velocidade base dos motores |
| `force` | `500` | Força aplicada aos motores |
| Margem bússola | `±6°` | Tolerância para parar a rotação |

### Componentes do robô

| Componente | Tipo | Uso |
|---|---|---|
| `motorLF`, `motorLB`, `motorRF`, `motorRB` | `Servomotor` | Tração — usados ativamente no loop |
| `armLeft`, `armRight` | `Servomotor` | Braços — erguidos e travados na inicialização |
| `motorBag`, `handLeft`, `handRight` | `Servomotor` | Declarados no robô, reservados para uso futuro |
| `colorLL`, `colorL`, `colorR`, `colorRR` | `ColorSensor` | Seguimento de linha e detecção de verde |
| `ultraLeft`, `ultraRight` | `UltrasonicSensor` | Leitura lateral de distância |
| `ultraFront1`, `ultraFront2` | `UltrasonicSensor` | Detecção de obstáculos à frente |
| `Bot.Compass` | `double` | Ângulo atual para rotação precisa |

### Arquitetura de funções

```
Main()
 ├── lockMotors(false) / lockArms(false) / upArms() / lockArms(true)
 └── loop while(true)
      ├── GetColors()         → atualiza leituras dos sensores de cor
      ├── GetDistance()       → atualiza leituras dos ultrassônicos
      ├── verde_A()?          → gyrate(±90) + avanço
      ├── distanceF ≤ 2?      → desvio + lineM() + reposicionamento
      └── else                → run() com correção proporcional

Funções auxiliares:
 ├── run()       → move os 4 motores com velocidades distintas para cada lado
 ├── right()     → rotação horária (motores L frente, R ré)
 ├── left()      → rotação anti-horária (motores L ré, R frente)
 ├── gyrate()    → gira X graus usando bússola com margem de ±6°
 ├── isgreen()   → verifica se um sensor lê verde (G > R*0.9 e G > B*0.9)
 ├── verde_R/L/A() → localiza o verde (direita, esquerda ou qualquer sensor)
 ├── GetColors() → captura string de cor de cada sensor
 ├── GetDistance() → captura distância de cada ultrassônico (999 se sem sinal)
 └── lineM()     → retorna true se sensor L está sobre a linha preta
```

---

## 📁 Estrutura do projeto

```
Sbotics_read/
│
├── README.md                       # Documentação do projeto
├── LICENSE                         # Licença MIT
│
├── assets/
│   └── demo.png                    # Captura do robô em execução
│
├── codes/
│   └── simulation.cs               # Código principal do robô (C#)
│
├── robots/
│   ├── newestSbotics.sBot          # Configuração mais recente do robô
│   └── Werther.sBot                # Configuração alternativa
│
└── arenas/
    ├── greenPlay.sArena            # Arena de treino (iniciante)
    ├── 90obstacle.sArena           # Arena com curvas em 90°
    ├── Myckey.sArena               # Arena avançada
    └── pesadelo_para_cinco.sArena  # Arena extrema
```

---

## 👥 Equipe

Desenvolvido com 💙 para a **OBR 2026 — Modalidade Simulação**.

| Nome | Contribuição |
|---|---|
| **Maurício Calvet** | Programação e estratégia de controle |
| **Lucas Benjamin** | Programação e estratégia de controle |
| **Huan Lin Fui** | Programação e estratégia de controle |
| **Arthur Martins** | Programação e estratégia de controle |

---

## 📄 Licença

Distribuído sob a licença **MIT**. Consulte o arquivo [LICENSE](LICENSE) para mais informações.

---

<div align="center">

*Feito para a [Olimpíada Brasileira de Robótica 2026](https://www.obr.org.br/) — Modalidade Simulação*

</div>
