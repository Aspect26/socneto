git checkout master
git pull

sudo docker-compose up -d
sudo docker build -t twitteracquirer -f acquisition/DataAcquirer/Dockerfile.twitter acquisition/DataAcquirer
sudo docker build -t kf_test -f acquisition/DataAcquirer/Dockerfile.producer acquisition/DataAcquirer

sleep 10s

sudo docker run --network host --restart always --detach twitteracquirer
mkdir output_data
sudo docker run --network host --restart always --detach  --mount source=output_data,target=/app/output_data kf_test output_data





