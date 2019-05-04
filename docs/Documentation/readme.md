Socneto Documentation
---

This document will contain final documentation of SOCNETO project

## Intro

It runs on premise!

## Prerequisities

### Kafka

Kafka must run
```bash
cd /usr/local/kafka
bin/zookeeper-server-start.sh config/zookeeper.properties
bin/kafka-server-start.sh config/server.properties
```

Kafka has to have following topics
* AcquireDataTopic
* databaseRaw
* analyser
* StoreAnalysisTopic
```bash
bin/kafka-topics.sh --create --zookeeper localhost:2181 --replication-factor 1 --partitions 1 --topic <topicName>
```

## Infrastructure and flow

There are 4 major components

- Coordinator - processes user requests and submit a task for a data acquirer.
- Data acquirer - loads posts and sends them to get indexed in a data storage
- Data storage - indexes data and give them to a analyser module 
- Analyser - analyses data and sends them back to the database

The whole application communicates via kafka topics

|from|to|topic name|description|
|:--|:--|:--|:--|
|Coordinator|Data acquirer|AcquireDataTopic| task definition, contains topic|
|Data acquirer| Storage| databaseRaw| data gets indexed|
|Storage|Analyser|analyser|indexed data gets analysed|
|Analyser|Storage|StoreAnalysisTopic|analysed data gets stored|

![topic flow image][topic-flow]

### Hardware 

Currently, the project uses five machines on ip from `10.3.4.101` to `10.3.4.105`.

* `10.3.4.101` - Hosts coordinator at port `6010` which can be accessed from the outside via `acheron.ms.mff.cuni.cz:39103`. It also hosts Data acquisition console app.
* `10.3.4.102` - Hosts db storage
* `10.3.4.103` - Hosts analyser
* `10.3.4.104` - Hosts teamcity which exposes port `8111`. It can be accessed from the outside using `acheron.ms.mff.cuni.cz:39107`
* `10.3.4.105` - Hosts Kafka(9092) which in future will be accessible from the outside using `acheron.ms.mff.cuni.cz:39108`


[topic-flow]: images/topic-flow.png "Topic flow image"
## Configs



### Analyser
Python has input parameters instead of config 

```bash
python3 main.py --server '10.3.4.105:9092' --consume_topic analyser --produce_topic StoreAnalysisTopic
```
* server - place where kafka runs
* consume_topic - topic on which analyser listens
* produce_topic - topic to which analyser produces

### DataAcquisition

Stored at `appsettings.json`

```json
{

  "Coordinator": {
    "TaskOptions": {
      "ConsumeTaskTopic": "AcquireDataTopic",
      "ProduceTopic": "databaseRaw"
    },
    "KafkaOptions": {
      "ServerAddress": "10.3.4.105:9092"
    },

    "Networks.Twitter": {
      "ConsumerKey": "",
      "ConsumerSecret": ""
    }
  }
}
```

### Coordinator

pretty much the same as Data acquisition


## Examples

### Submit job
The app is interacted with via following command

```
curl --header "Content-Type: application/json"  --request POST  --data '{"Topic":"CharlesTonSon"}'  http://acheron.ms.mff.cuni.cz:39103/api/submit
```

### Start analyser

* python app
* at `10.3.4.103`

```bash
python3 main.py --server '10.3.4.105:9092' --consume_topic analyser --produce_topic StoreAnalysisTopic
```

### Start coordinator
* .NET WebApp
* at `10.3.4.101`
* runs only in debug
  
```bash 
cd /home/knotek/sa/SWProject/Coordinator/Socneto/ConsoleApp/
dotnet run --urls "http://*:6010"
```

*NOTE*: For those who uses MobaXTerm, you have to create two sessions to server and close the first one after. Session 1 opens at 6010 and the second at 6011, closing the first one releases the port for the application.


### Start data acquisition

* .Net console app
* at `10.3.4.101`
* runs only in debug
```bash
cd /home/knotek/sa/SWProject/DataAcquisition/Socneto.DataAcquisition/ConsoleApp/
dotnet run
```

### start db storage

* java 
* at `10.3.4.102`

```bash
java -jar producer-consumer-1.0.0-SNAPSHOT.jar
```

## Goodies

[Kafka](
https://tecadmin.net/install-apache-kafka-ubuntu/)


[How to run stuff on background](https://unix.stackexchange.com/questions/8469/how-can-i-close-a-terminal-without-killing-its-children-without-running-screen)
```
ctrl+z
bg
jobs -l
disown -h <id>
```