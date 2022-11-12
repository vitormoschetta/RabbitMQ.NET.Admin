# RabbitMQ

No link abaixo gravamos um exemplo prático que serve de base para esse projeto.

[RabbitMQ Projeto](https://www.canva.com/design/DAFRUv2rHdY/O6XmU5bD-NfQIzXWtCFaMA/watch?utm_content=DAFRUv2rHdY&utm_campaign=designshare&utm_medium=link&utm_source=publishsharelink)


Segue abaixo o link para o simulador de filas do RabbitMQ que foi demonstrado no vídeo acima:

[RabbitMQ Simulator](http://tryrabbitmq.com/)


## Get Started

#### Init RabbitMQ Management

```bash
docker-compose up -d rabbitmq
```
<http://localhost:15672>



#### Init Producer

```bash
cd other/Producer
dotnet run
```


#### Init Consumers

```bash
cd other/consumer.Pgto
dotnet run

cd other/Consumer.Pgto.Boleto
dotnet run

cd other/Consumer.Pgto.Cartao
dotnet run
```


<br>


## Explicação do projeto

O projeto `Producer` executa as seguintes ações:

#### Cria uma conexão com o RabbitMQ

#### Pede para o usuário selecionar uma opção de pagamento 
Ao selecionar uma opção, é definido o routing key, que será usado para enviar a mensagem para a fila correta.

#### Cria uma fila para mensagens mortas (pgto.dql)
A lógica é a seguinte: 
- Criamos uma fila para mensagens mortas (pgto.dql)
- Criamos uma exchange do tipo `fanout` (pgto.dql)
- Fazemos o bind da exchange com a fila para mensagens mortas.

#### Cria uma fila para mensagens normais (pagto)
- Criamos uma fila para mensagens normais (pgto). Porém, essa fila define uma regra de roteamento para mensagens mortas. isso é feito através do parâmetro `x-dead-letter-exchange` que define a exchange que receberá as mensagens mortas.
- Criamos uma exchange do tipo `topic` (pgto). Esse tipo de exchange é mais utilizado nos casos em que queremos fazer roteamento dinâmico de mensagens, com base no routing key.
- Fazemos o bind da exchange com a fila para mensagens normais.
- Se a mensagem não for processada em 10 segundos (definimos isso no momento de enviar a mensagem), ela é enviada para a exchange de mensagens mortas. E a fila de mensagens mortas irá processá-la.

#### Envia uma mensagem para a fila de mensagens normais
- A mensagem é enviada para a exchange de mensagens normais, com o routing key definido pelo usuário.
- A exchange de mensagens normais irá enviar a mensagem para a fila de mensagens normais, com base no routing key.
- Ao passar 10 segundos, se a mensagem não for processada, ela é enviada para a exchange de mensagens mortas. Isso vai acontecer se apenas o projeto `Producer` for executado. Se os projetos `Consumer.Pgto.Boleto` e `Consumer.Pgto.Cartao` forem executados, a mensagem será processada antes de 10 segundos.


Os projetos `Consumer.Pgto.Boleto` processa as mensagens com o routing key `boleto` e o projeto `Consumer.Pgto.Cartao` processa as mensagens com o routing key `cartao`.
E ainda tem o projeto `Consumer.Pgto` que processa as mensagens com o routing key `*`, ou seja, todas as mensagens que também foram processadas pelos projetos `Consumer.Pgto.Boleto` e `Consumer.Pgto.Cartao`.
<br>


## Neste projeto também temos uma API para gerenciar os recursos do RabbitMQ.

#### Init API

```bash
cd src/Admin.Api
dotnet run
```
<http://localhost:6601/swagger/index.html>



<br>


## Conceitos abordados:


#### Queue Arguments

- Durable: Significa que a fila será mantida mesmo após o servidor ser reiniciado.

- Exclusive: Significa que apenas um consumidor terá acesso a fila.

- Auto Delete: Significa que a fila será excluída quando não houver mais consumidores conectados a ela.


#### Exchange Arguments

- Direct: Envia a mensagem para a fila que tiver o mesmo nome da routing key da mensagem.

- Fanout: Envia a mensagem para todas as filas que estiverem conectadas ao exchange.

- Topic: Envia a mensagem para as filas que tiverem o mesmo nome da routing key ou que tenham o mesmo nome da routing key com um wildcard (# ou *).


#### Exchange Bindings

- Bind: Significa conectar uma fila a um exchange.

- Unbind: Significa desconectar uma fila de um exchange.


#### Publish Confirms

Um tipo de configuração onde o Producer/Publisher espera uma confirmação do Broker (comunicação síncrona) de que a mensagem foi publicada com sucesso.
Não é uma configuração recomendada para aplicações de alta performance, pois a cada mensagem publicada o Producer/Publisher aguarda a confirmação do Broker.
Essa abordagem não entregará mais do que algumas centenas de mensagens publicadas por segundo.

Uma estratégia para melhorar a performance é utilizar o Publish Confirms em conjunto com o Batch Publish, que é uma configuração onde o Producer/Publisher envia várias mensagens para o Broker de uma só vez.

<https://www.rabbitmq.com/tutorials/tutorial-seven-dotnet.html>


#### Recuperação de falhas

Verificar como:
- limitar quantidade de retentaivas de envio de mensagem.
- limitar tempo de espera para envio de mensagem.
- criação de fila e exchange de mensagens não entregues (dead letter queue):
    - Comprimento máximo da fila excedido.
    - Tempo de vida da mensagem excedido.
    - Mensagem rejeitada pelo consumidor.

