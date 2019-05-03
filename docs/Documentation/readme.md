Socneto Documentation
---

This document will contain final documentation of SOCNETO project

## Prerequisities

Topics:
* `AcquireDataTopic`
* `databaseRaw`

```

cd /usr/local/kafka
bin/zookeeper-server-start.sh config/zookeeper.properties
bin/kafka-server-start.sh config/server.properties

bin/kafka-topics.sh --create --zookeeper localhost:2181 --replication-factor 1 --partitions 1 --topic <topicName>

```

## Goodies

Kafka 
https://tecadmin.net/install-apache-kafka-ubuntu/

```
https://unix.stackexchange.com/questions/8469/how-can-i-close-a-terminal-without-killing-its-children-without-running-screen

ctrl+z
bg
jobs -l
disown -h <id>
```

## Configs

```

```

## Examples

### Submit


```
curl --header "Content-Type: application/json"  --request POST  --data '{"Topic":"CharlesTonSon"}'  http://acheron.ms.mff.cuni.cz:39103/api/submit
```



