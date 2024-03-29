. ./utils.ps1

function Test-Da {
    param(
        $context,
        $dockerfile,
        $component_name,
        $credentials_path
    )
    echo "Build images"
    BuildRun-Docker $component_name $dockerfile $context
    
    echo "wait 180s for acquirer to start"
    Start-Sleep -s 180

    # act & assert
    echo "start testing"
    python ../code/test_da.py ../code/config.json $credentials_path

    # clean
    echo "cleaning"
    Clean-Docker $component_name
}

$da_context = "../../acquisition/DataAcquirer"

echo "Running twitter test"
$twitter_dockerfile = "$da_context/Dockerfile.twitter"
Test-Da $da_context $twitter_dockerfile "twitter_da" "../code/twitter.cred"

echo "Running reddit test"
$reddit_dockerfile = "$da_context/Dockerfile.reddit"
Test-Da $da_context $reddit_dockerfile "reddit_da" "../code/reddit.cred"