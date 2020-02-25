. ./utils.ps1

$context = "../../backend/"
$dockerfile = "$context/Dockerfile"
$component_name = "backend"

echo "Build images"
BuildRun-Docker $component_name $dockerfile $context "component_tests_default" "docker-compose-be-test.yml"

# act & assert
echo "start testing"
python3 --version
python3 ../code/test_be.py ../code/config.json

# clean
echo "cleaning"
Clean-Docker $component_name
