Data acquirer
---

Types
- twitter
- static file
- \[tba\] reddit

in case of social network, credentials are expected to be provided in job definition

**NOTE** the front end can still have some defaults. 




# Tech details

DAs shares the same solution. They differ in entry point, the separate project is dedicated to each DA.

They are supposed to run in docker, for that purpose, all of them have dedicated `Dockerfile.<type>` in `DataAcquirer` solution

You have to specify the file to build id
```bash
docker build --file .\Dockerfile.mock .
```

