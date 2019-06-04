FROM google/dart

RUN mkdir /app

ADD /pubspec.yaml /app/pubspec.yaml
ADD /web /app/web
ADD /lib /app/lib
ADD /build /app/build

WORKDIR "/app"

RUN pub get
RUN pub global activate webdev 2.0.5

ENV PATH $PATH:/root/.pub-cache/bin

# TODO: should be using flag --release, but it is not working with dart2js compiler for now :(
CMD ["webdev", "serve",  "web:8080", "--hostname", "0.0.0.0"]
