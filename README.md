# Auto-Scheduler

Final Uni project

## Summary

Full-stack application with .Net Web API backend, Vue.js with TS frontend, MSSQL Database.
Allows users to automatically generate timesheets for large groups in certain halls with certain presenters for a a set number of activites. Different resources, related to the schedules can be managed, such as halls, presenters, activity templates, group definitions. The timesheet generation takes into account halls and presenters' busy hours and the structure of the groups, included in it (one can be a child, i.e. part of another), as well as the set requirements for the activities such as duration, type of hall, group that it's made for, etc.

## Installation
Summary for setting up and running the project locally
### Requirements
To containerize the application:
- Docker Engine under a Linux environment, or under Docker Desktop, WSL, colima, etc.

Alternatively, for building different components locally:
- .Net 8 SDK
- SQL Server
- Node.js package manager
### Steps 
After cloning the repo, copy the contents of the example .env.example file into another .env at the project root directory.

***To containerize the app with Docker:***

Provided you have cloned the repo into a Unix environment and after making any changes to the source, open a terminal at the root project directory and run 
```bash
docker compose up --build -d
```
to build the fe, be, db images and start the containers. alternatively, leave out the **--build** flag.
And to stop the containers:
```bash
docker compose down
```

***To build seperate components:***
TODO

Using the .env.example provided, the frontend SPA can be accessed at

[localhost:5173](http://localhost:5173/)

And the Swagger UI at

[http://localhost:5157/swagger/index.html](http://localhost:5157/swagger/index.html)

In a dev environment, data is beeing seeded when initially running the api for user roles and users with the following credentials:
```
Admin:
Email: testadmin@mail.com
Password: TestPassword123!

ResourceManager:
Email: testmng@mail.com
Password: TestPassword123!

ScheduleManager:
Email: testsched@mail.com
Password: TestPassword123!
```
Data across all entites can also be bootstrapped from .csv files containing information about halls and Aztivity requirements for timesheet generation by putting CSVs with the following names in **AutoSchedulerBE/AutoScheduler.API** :

<organization_name>.Requirements.csv

<organization_name>.Halls.csv


![image](https://github.com/user-attachments/assets/11b68f27-b63a-4ddd-9330-f1c74bfd200b)
![image](https://github.com/user-attachments/assets/0c4c2be6-10e8-4727-a2cb-938008d8e924)
![image](https://github.com/user-attachments/assets/43e8f166-d4b9-4406-9b64-fa248501a86d)
![image](https://github.com/user-attachments/assets/d1627dea-5af6-47d1-a54d-719e44ad1f03)


Generated timesheets are displayed as tables with columns for time range and rows for number of subgroupsand can be saved. Seperate pages contain information and ability to create, update delete activity templates, halls, presenters, including the availability of different halls and presenters.
![image](https://github.com/user-attachments/assets/0e1ec2e7-d255-4e8b-be48-21ae121cc904)
![image](https://github.com/user-attachments/assets/b19b680d-582e-4524-8e67-6b8710dcb349)


Contain basic authentication and authorization on top of ASP.Net Identity.
![image](https://github.com/user-attachments/assets/077c1788-f7c7-45f5-963a-6d721a883aab)![image](https://github.com/user-attachments/assets/e8adb658-f81f-49de-8de3-e913d6784687)

