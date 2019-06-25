## Platform

The framework has on service oriented architecture (acronym SOA, see [wiki](https://en.wikipedia.org/wiki/Service-oriented_architecture)). SOA allows for versatility, separation of concerns and future extension.

Communication between the services is provided by distributed publish/subscribe message broker Kafka (see [Documentation](https://kafka.apache.org/documentation/)). It will allow us to create flexible data flow pipeline since components are not required to know predecessor or successor.

<basic kafka flow picture>

### Components
Each service has unique responsibility given by functional requirements (ref ...).


* API gateway - Exposes [REST](https://en.wikipedia.org/wiki/Representational_state_transfer) api allowing users to submit jobs and query results of their jobs.
* Data Acquirer - downloads requested data from a given source
  * This module can be extended using client-implemented adapters 
* Analyser - performs sentiment analysis of a given text
* Database storage - stores analysed data along with source text. Also stores application data.
* Job foreman(předák) - contains all jobs necessary to successful job execution.


### Communication/Cooperation

As was previously stated, data are exchanged using message broker. The main reason to adopt it was its suitability to event driven systems. The framework fires multiple events to which multiple (at the same time) components can react. In our case, data can be acquired from multiple data sources at the same time and send to multiple analysis modules. This complex can be implemented using standard request/response model but more elegant solution is to use publish/subscriber model.

It offers components to subscribe to a topic, to which producer sends data. Multiple producers can publish data to the same topic and kafka makes sure to deliver them to all subscriber. It also keeps track of which message was delivered and which was not delivered yet, so if any component temporarily disconnects, it can continue at work once the connection is up again. 

<event diagram opposing to http approach >

Another benefit of message broker is that particular services does not aware of a sender of its input data and of receiver of its output. It makes it easy to configure data flow.


### What happens when the job gets submitted

TODO Lukas

### Extensibility

TODO Lukas

### Metrics

TODO Lukas