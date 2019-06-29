FROM google/dart

RUN mkdir /app

ADD /frontend/pubspec.yaml /app/pubspec.yaml
ADD /frontend/web /app/web
ADD /frontend/lib /app/lib
ADD /frontend/build /app/build

WORKDIR "/app"

RUN pub get
RUN pub global activate webdev 2.0.6

ENV PATH $PATH:/root/.pub-cache/bin

EXPOSE 8080

# TODO: should be using flag --release, but it is not working with dart2js compiler for now :(
CMD ["webdev", "serve",  "web:8080", "--hostname", "0.0.0.0"]
