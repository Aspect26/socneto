function BuildRun-Docker {
    param (
        $component_name,
        $dockerfile,
        $context,
        $network="component_tests_default"
    )
    $component_image_name = "image_$component_name"
    $container_name = "container_$component_name"

    docker build -t $component_image_name -f $dockerfile $context
    docker-compose up -d

    Write-Host "wait 10s for kafka"
    Start-Sleep -s 10
    
    echo "run container $container_name with network $network"
    docker run --network $network -d --name $container_name $component_image_name 
}

function Clean-Docker {
    param (
        $component_name
    )

    $component_image_name = "image_$component_name"
    $container_name = "container_$component_name"

    docker kill $container_name
    docker container rm $container_name
    docker-compose down
    docker image rm -f $component_image_name
}