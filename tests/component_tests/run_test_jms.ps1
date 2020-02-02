. ./utils.ps1

$context = "../../job-management/"
$dockerfile = "$context/Dockerfile"
$component_name = "jms"

echo "Build images"
BuildRun-Docker $component_name $dockerfile $context
echo "wait 15s for acquirer"
Start-Sleep -s 15

# act & assert
echo "start testing"
python ../code/test_jms.py ../code/config.json

# clean
echo "cleaning"
Clean-Docker $component_name