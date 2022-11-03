# Todo Project 


### Get Started


#### Init RabbitMQ Management

```bash
docker-compose up -d rabbitmq
```
<http://localhost:15672>


#### Init API

```bash
cd src/Admin.Api
dotnet run
```
<http://localhost:6601/swagger/index.html>


#### Init Producer

```bash
cd other/Producer
dotnet run
```


#### Init Consumer

```bash
cd other/Consumer
dotnet run
```


<br>



### Queue Management

#### Queue Arguments

Durable: Significa que a fila será mantida mesmo após o servidor ser reiniciado.

Exclusive: Significa que apenas um consumidor terá acesso a fila.

Auto Delete: Significa que a fila será excluída quando não houver mais consumidores conectados a ela.
