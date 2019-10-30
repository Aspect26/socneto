container_name="socneto/fe"
docker build -t ${container_name} .
docker run -p "8080:8080" ${container_name}
