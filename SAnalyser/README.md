POC `SAnalyser` doc draft
===

## preface

This document contains basic info about the poc. The code represents a base for to-be framework for *social network* (SN) data analysis.

## requirements

- .Net core 2.2

## Interaction

It is a .net core web-api application. 

Responds only to https request to `/api/submit` with `Application/json` MIME type. 

### input

Expects input in following format
~~~json
{
	Topic:"apple",
	FromDate:"2017-05-05T18:20:26.000Z",
	ToDate:"2017-05-05T18:20:26.000Z"
}
~~~

From/To Date must follow w3c format.

### output

Returns list of string in json format.

## Implementation

App consists of following parts:

- `Analyser` - represent to-be remote procedure analyzing SN data. Currently counting top-ten keyword
- `DataCollector` - represent to-be data collector. Returns 10k tweets from file. 
- `Domain` - in other words: Simplified Coordinator

 In this case, data are mocked using file with 10k tweets twitter.

## design

Main part is a Domain. Which is a coordinator. It exposes two important interfaces

- `IDataCollector`
- `IAnalyser`

And expects their proper implementation. In a near future, they will serve as an RPC caller to other services

Project employs **IoC/DI** that .net core offers. 

## Conclusion

This project is under development. Everything there is subject to changes. 



