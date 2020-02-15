DEPLOY PROCESS:

# INSTALL DOCKER ^18.0.9.7

# INSTALL DOCKER COMPOSE ^1.23.2
sudo curl -L "https://github.com/docker/compose/releases/download/1.24.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose


# INSTALL SOCNETO

mkdir socneto
cd ./socneto
git clone https://github.com/Aspect26/socneto.git
cd ./socneto
sudo docker-compose up


PROBLEMS:
1. can't download sentiment analyser's model (urllib3.exceptions.MaxRetryError: HTTPConnectionPool(host='acheron.ms.mff.cuni.cz', port=39107): Max retries exceeded with url: /models?location= (Caused by NewConnectionError('<urllib3.connection.HTTPConnection object at 0x7fcae774e9d0>: Failed to establish a new connection: [Errno 113] No route to host')))



acheron.ms.mff.cuni.cz:39103
