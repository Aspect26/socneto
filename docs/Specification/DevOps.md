This section describes what tools will be used in order to comply with software engineering best practises.


### Deployment
Services are deployed in form of container. For the deployment and following container management [Kubernetes](https://kubernetes.io/) orchestration system is employed. 

### Versioning
Code is versioned using Git repository.
- code reviews
- brancing model

### CI/CD and automation

CI/CD is implemented using [TeamCity](https://www.jetbrains.com/teamcity/). It allows for a deployment pipeline definition in which code in the repository get automatically tested, then containerized and deployed. This pipeline is triggered by push to a given branch of the repository.
