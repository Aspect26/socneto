# Storing metadata

## Store job definition

stored to `job_management.job_definition.storage_db`

## Store component registration request


Jobs are immutable

Data acquirer command added
```c#
[JsonProperty("command")]
public string Command { get; set; }
```

prerequisites

- messaging
- components
- jms
  - registration flow
    - it is idenpotent (must be)


# What the system does



# Basic platform architecture


## Components


## Messaging



# Implementation 

## Basic component rules

- register itself
- use kafka for communication

## Messaging in details

Async

Kafka (ref TODO)

Why

# Components in detail

## jms
## acquirers
## analysers

