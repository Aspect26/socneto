#
# Build stage
#
FROM maven:3.6.0-jdk-11-slim AS build
COPY analysis-results /home/app/analysis-results
COPY internal-storage /home/app/internal-storage
COPY storage-messaging /home/app/storage-messaging
COPY storage-web /home/app/storage-web
COPY pom.xml /home/app

RUN mvn -f /home/app/pom.xml clean package


#
# Package stage
#
FROM openjdk:11-jre-slim
COPY --from=build /home/app/storage-web/target/storage-web-1.0.0-SNAPSHOT.jar /usr/local/lib/storage.jar
EXPOSE 8080
ENTRYPOINT ["java","-jar","/usr/local/lib/storage.jar"]