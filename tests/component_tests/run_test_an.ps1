. ./utils.ps1

function Test-An {  
    param(
        $context,
        $dockerfile,
        $component_name,
        $input_topic_name
    )
    echo "Build images"
    BuildRun-Docker $component_name $dockerfile $context
    
    echo "wait 40s for analyser to start"
    Start-Sleep -s 40
    
    # act & assert
    echo "start testing"
    python ../code/test_an.py ../code/config.json $input_topic_name 

    # clean
    echo "cleaning"
    Clean-Docker $component_name
}

echo "Running topic modelling test"
$topic_context = "../../analysers/topic_modeling" 
$topic_dockerfile = "$topic_context/dockerfile"
Test-An $topic_context $topic_dockerfile "topic_modelling" "job_management.component_data_input.DataAnalyser_topics"

$sentiment_topic="job_management.component_data_input.DataAnalyser_sentiment"
$sentiment_context="../../analysers/sentiment_analysis"
$sentiment_dockerfile = "$sentiment_context/dockerfile"
Test-An $sentiment_context $sentiment_dockerfile "sentiment" $sentiment_topic

