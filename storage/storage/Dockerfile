# Alpine Linux with OpenJDK JRE
FROM openjdk:11-jdk
EXPOSE 8080
# copy WAR into image
COPY ./storage-web-1.0.0-SNAPSHOT.jar /app.jar 
# run application with this command line 
CMD ["java", "-jar", "/app.jar"]