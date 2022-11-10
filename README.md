# Todo Project 

No link abaixo gravamos um exemplo prático que serve de base para esse projeto.

[RabbitMQ Projeto](https://www.canva.com/design/DAFRUv2rHdY/O6XmU5bD-NfQIzXWtCFaMA/watch?utm_content=DAFRUv2rHdY&utm_campaign=designshare&utm_medium=link&utm_source=publishsharelink)


Segue abaixo o link para o simulador de filas do RabbitMQ que foi demonstrado no vídeo acima:

[RabbitMQ Simulator](http://tryrabbitmq.com/)



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


