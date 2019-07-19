FROM google/dart

RUN mkdir /app
ADD /frontend /app

WORKDIR "/app"

RUN pub get
ENV PATH $PATH:/root/.pub-cache/bin
RUN pub global activate webdev 2.0.6

RUN webdev build --release

EXPOSE 8080

CMD ["webdev", "serve",  "web:8080", "--hostname", "0.0.0.0", "--release"]
