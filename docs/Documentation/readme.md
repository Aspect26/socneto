Socneto Documentation
---

This document will contain final documentation of SOCNETO project

## Intro

It runs on premise!

## Infrastructure and flow

There are 4 major components

- Coordinator - processes user requests and submit a task for a data acquirer.
- Data acquirer - loads posts and sends them to get indexed in a data storage
- Data storage - indexes data and give them to a analyser module 
- Analyser - analyses data and sends them back to the database

![topic flow image][topic-flow]

### Hardware 

Currently, the project uses five machines on ip from `10.3.4.101` to `10.3.4.105`.

* `10.3.4.101` - Hosts coordinator at port `6010` which can be accessed from the outside via `acheron.ms.mff.cuni.cz:39103`. It also hosts Data acquisition console app.
* `10.3.4.102` - Hosts db storage
* `10.3.4.103` - Hosts analyser
* `10.3.4.104` - Hosts teamcity which exposes port `8111`. It can be accessed from the outside using `acheron.ms.mff.cuni.cz:39107`
* `10.3.4.105` - Hosts Kafka(9092) which in future will be accessible from the outside using `acheron.ms.mff.cuni.cz:39108`


[topic-flow]: images/topic-flow.png "Topic flow image"




## Goodies


### Kafka related
[Kafka](
https://tecadmin.net/install-apache-kafka-ubuntu/)


[How to run stuff on background](https://unix.stackexchange.com/questions/8469/how-can-i-close-a-terminal-without-killing-its-children-without-running-screen)
```
ctrl+z
bg
jobs -l
disown -h <id>
```

