******TaaghcheDemo******

---Overview---

This project is a .NET Core Web API built using .NET 7.0. It serves as a middleware between client applications 
and a specific external API (Taaghche API). The API retrieves data from the Taaghche API using an HTTP client 
and provides caching services to improve performance and reduce load on the external API. Additionally, 
itincludesa consumer component that listens for update events from RabbitMQ and keeps the cache data fresh 
by updating or deleting outdated information.

---------------------------------------------------------------------------------------------------------------------

---Features---

-.NET Core Web API built with .NET 7.0
-Utilizes an HTTP client to fetch data from the Taaghche API
-Implements two layers of caching services:
    -Memory cache service
    -Redis cache service
-Consumer component listens for update events from RabbitMQ using MassTransit
-Automated cache expiration time configuration
-Dockerized deployment with Dockerfile and docker-compose.yaml

---------------------------------------------------------------------------------------------------------------------

---Technologies Used---

-.NET Core 7.0
-ASP.NET Core Web API
-Docker for containerization
-RabbitMQ for messaging
-MassTransit for RabbitMQ integration
-Memory caching
-Redis caching
-HttpClient for making HTTP requests

---------------------------------------------------------------------------------------------------------------------

---Installation and Setup---

1.Clone the repository using this bash command:

    git clone https://github.com/Puriatc10/TaaghcheDemo.git
    
2.Navigate to the project directory(usually: C:/Users/YourUser/source/repos/TaaghcheDemo)using this bash command:

    cd C:/Users/{YourPCUser}/source/repos/TaaghcheDemo
    
3. Run this docker-compose command:

    docker-compose up -d
   
---------------------------------------------------------------------------------------------------------------------

---Usage---

-Access the API endpoints to fetch data from the Taaghche API.
-Monitor the RabbitMQ consumer component for update events and cache refreshes.

---------------------------------------------------------------------------------------------------------------------

---Testing---

-Use the included .NET web application (MessagePublisher Web App) for testing purposes
ans Publish event messages to RabbitMQ to simulate data updates.

---------------------------------------------------------------------------------------------------------------------

---Configuration---

-Customize cache expiration times in the configuration files.
-Adjust Dockerfile and docker-compose.yaml for specific deployment requirements.

---------------------------------------------------------------------------------------------------------------------

***   Description about part 4 of given Task ***

The current method used to keep information *sync* in the cache service is to delete data that changed in other services from cache service.
Advantages: 
- The cached data is never different from the changed data(data is always *sync*).
- The total server response time has decreased. Because if the data was not cleared from the cache after changes in other web services,
     we would have to call api once for each request to make sure that the data is in sync, and also the cache service would be practically useless.
Disadvantages: 
- average response time per each user request increases because any change in the data of each book causes
  the previous cached data to be deleted from the cache service and we have to wait for the api to be called (in next request) every time to get the data.

Solution:

We can sacrifice total server response time to improve system performance for each request.
In this way, when receiving any message from RabbitMq, in addition to deleting the previous cached data,
we can call the api once and validate the data so that we don't have to wait for the next user request and the data is always *sync*.
Advantages:
- In this method, users wait less time on average because with more requests,
    more data is cached, all cached data is always *sync* and the response time for each request is significantly reduced.
Disadvantages:
- For each message from RabbitQM, an api is called from the server side, and if the number of data changes in other web services is high,
     the total server response time increases a lot and may even cause the server to go down.

Extra Solution:
In Previous Case,
 We can use RabbitMQ duplicate messages so that for each bookId, only one message representative of all its changes is published in the queue.

