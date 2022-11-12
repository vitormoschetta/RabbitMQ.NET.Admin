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
cd src/Producer
dotnet run
```


#### Init Consumers

```bash
cd src/consumer.Pgto
dotnet run

cd src/Consumer.Pgto.Boleto
dotnet run

cd src/Consumer.Pgto.Cartao
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

- Quando o RabbitMQ é reiniciado, as filas e exchanges são perdidas. Para evitar isso, podemos definir as filas e exchanges como `durable`.


#### Dead Letter Exchange

- É uma exchange que recebe mensagens que não foram processadas por um consumidor. Essas mensagens são enviadas para a dead letter exchange quando o TTL (Time To Live) da mensagem expira ou quando o consumidor rejeita a mensagem.

- Para configurar uma dead letter exchange, precisamos definir o parâmetro `x-dead-letter-exchange` na criação da fila. Esse parâmetro define o nome da exchange que receberá as mensagens mortas.


#### Mover mensagens de uma fila para outra

É necessário habilitar o plugin `rabbitmq_shovel` no RabbitMQ. Executar dentro do container do RabbitMQ:

```bash
rabbitmq-plugins enable rabbitmq_shovel rabbitmq_shovel_management
```

Obs: Temos um endpoint com exemplo de como mover mensagens de uma fila para outra. Olhar o arquivo `src/Admin.Api/Controllers/QueueController.cs`.


#### Alta disponibilidade (High Availability - HA)

Já que o Broker é o coração da arquitetura, precisamos garantir que ele esteja sempre disponível. Para isso, podemos utilizar o cluster RabbitMQ.

Para criar um cluster RabbitMQ, execute o comando abaixo:
    
```bash
docker-compose -f docker-compose-cluster.yaml up -d
```

com o comando acima subimos 3 instâncias do RabbitMQ, cada uma em um container, e uma porta diferente.

Para configurar o cluster, execute o comando abaixo:

```bash
docker exec -it rabbitmq2 rabbitmqctl stop_app
docker exec -it rabbitmq2 rabbitmqctl reset
docker exec -it rabbitmq2 rabbitmqctl join_cluster rabbit@rabbitmq1
docker exec -it rabbitmq2 rabbitmqctl start_app

docker exec -it rabbitmq3 rabbitmqctl stop_app
docker exec -it rabbitmq3 rabbitmqctl reset
docker exec -it rabbitmq3 rabbitmqctl join_cluster rabbit@rabbitmq1
docker exec -it rabbitmq3 rabbitmqctl start_app
```

O que fizemos acima foi:
- Parar a aplicação do RabbitMQ no container `rabbitmq2`.
- Resetar o container `rabbitmq2`.
- Juntar o container `rabbitmq2` ao cluster `rabbit@rabbitmq1`.
- Iniciar a aplicação do RabbitMQ no container `rabbitmq2`.

O mesmo foi feito para o container `rabbitmq3`.

Para verificar o status do cluster, execute o comando abaixo:

```bash
docker exec -it rabbitmq1 rabbitmqctl cluster_status
```

Com tudo isso o nosso cluster ainda não está com alta disponibilidade. Para isso precisamos atualizar a política de HA do cluster (informar vhost, exchange e queue):

```bash
docker exec -it rabbitmq1 rabbitmqctl set_policy ha-all "^" '{"ha-mode":"all"}' --priority 1 --apply-to all --vhost adminvhost
```

Agora sim, nosso cluster está com alta disponibilidade. Se exisitir uma falha em um dos nós do cluster, o RabbitMQ irá automaticamente redirecionar as mensagens para os outros nós do cluster.

<br>

## Referências
<https://www.rabbitmq.com/clustering.html>