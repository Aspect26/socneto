_How does it look inside_

The framework has on service oriented architecture (acronym SOA, see [wiki](https://en.wikipedia.org/wiki/Service-oriented_architecture)). SOA allows for versatility, separation of concerns and future extension.

Communication among the services is provided by distributed message broker Kafka (see [Documentation](https://kafka.apache.org/documentation/)). It will allow us to create flexible data flow pipeline since components are not required to know predecessor or successor.

**TODO picture**

### Components
_What pieces is the system composed of_

Each service has unique responsibility given by functional requirements (ref ...).


* API gateway - Exposes [REST](https://en.wikipedia.org/wiki/Representational_state_transfer) api allowing users to submit jobs and query results of their jobs.
* Data Acquirer - downloads requested data from a given source
  * This module can be extended using client-implemented adapters 
* Analyser - performs sentiment analysis of a given text
* Database storage - stores analysed data along with source text. Also stores application data.



### Communication

_
TODO 
- apis between components
- topics