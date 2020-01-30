# arrange
$image_name="my_jms_test"
echo "building jms $image_name"
docker build -t $image_name -f "../job-management/Dockerfile" "../job-management"

echo "start kafka"
docker-compose up -d

echo "wait 15s for kafka"
Start-Sleep -s 15

$network="tests_default"
$container_name="analyser_test_container"
echo "run container $container_name with network $network"
docker run --network $network -d --name $container_name $image_name 

echo "wait 15s for jms"
Start-Sleep -s 15

# act & assert
echo "start testing"

python code/test_jms.py ./code/config.json

# clean
echo "cleaning"
docker kill $container_name
docker container rm $container_name
docker-compose down
docker image rm -f $image_name
