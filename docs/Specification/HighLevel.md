```
This chapter is intended to describe the following

- High-level description
- basic user interaction
- Data flow
```

## High level description

Socneto is an extensible framework allowing user to analyse vast socials networks's content. Analysis and social network data collector can be provided by user. 

Social networks offers free access to data, although the amount is limited by number of records per minute and age of the post which restrict us from downloading and analyzing large amount of historical data. To overcome those limitation, Socneto focuses on continuous analysis. It downloads data as they are published and analyses them immediately offering user up-to-date analysis.

<pic1>

The framework runs on premises which gives the user total control over its behavior. (This environment relies less on a user friendly UI and security. Also the extensibility is done by skilled operator and requires less attention of developers)

## Use case 

Generally, a user specifies a topic for analysis and selects data sources and type of analyses to be used. The system then starts collecting and analyzing data. User can then see summary in form of sentiment chart, significant keywords or post examples with the option to explore and search through them. 

Sample use cases might be studying sentiment about a public topic (traffic, medicine etc.) after an important press conference, tracking the opinion evolution about a new product on the market, or comparing stock market values and the general public sentiment peaks of a company of interest. This project does not serve to any specific user group, it tackles the problem of creating concise overview and designing a multi-purpose platform. It aims to give the user an ability to get a glimpse of prevailing public opinion concerning a given topic in a user-friendly form.

## Architecture - overview 

The framework consists of multiple modules forming a pipeline with modules dedicated to data acquisition, analysis, persistance and also to module managing the pipeline's behavior.

<Lukas's simplified picture>




## Goals

Main goal of this project is to build the platform allowing extensibility and flexibility to users and offer them basic analyses and a complex one proving its usability.



