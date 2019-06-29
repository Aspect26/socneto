## Communication

- KAFKA is not dedicated to data only.

At this point, reader knows

- Modules with respective responsibilities
- Infrastructure
- Communication basic (job submit, data acquisition, analysis, storage, querying)
- Docker infrastructrue

### API

API is shared among type(input output messages)

### Topics and communication itself

<Lukas' cool picture>


[obsolete]
### Initialization

The system is implemented as docker containers (expecting single application in each container). Each container is given an address of a the job management service.

When a container starts it requests(pull) a network configuration from the job management service. The configuration contains input and output topics for a given module e.g. module must be registered before its first use. 

Also, when the container starts, it must register job configuration callback to receive configuration of each submited job.

### Job management

When user submits job on UI, it gets to a Job Management Service(foreman?). It will parse the job and **distributes**(push) respective configuration to all nodes.

### Configuration

Two types
- node [obsolete]
- job

[obsolete]
Node config contains[**TODO** remove]
- input topic(which the node listens to)
- output topic(into which it produces)


Job configs varies for each component type (if flexible pipeline then for each node)(acquirer, analyser and storage). When the job starts, each component receives tailored configuration(that's why they have to be registered).

*From top of my head*

Acquirer job config
- Networks credentials

Analyser
- dunno 

Storage
- dunno

### Extensibility

When user want to add analyser or data acquirer it need to implement registration on the framework startup.


[TODO Lukas]

Rules

- on startup
  - job config 2 node (node is alias for any component) channel is established
    - defined by name 
    - one way
  - node accepts configuration or control message (input output topic) per job subnmit (which also contains input and output topic **allows for flexible pipeline**)
- fixed configuration per node
  - NodeUniqueId (prefix for the topics + metrics identifier)
  - MetricCollectorTopic
- Metrics
  - collects events and failures


What should user know now
- 