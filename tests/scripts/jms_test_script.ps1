./ConsoleApi.exe localhost:9094 produce job_management.registration.request '{    \"componentType\":\"DATA_ANALYSER\",    \"componentId\":\"analyser1\",    \"inputChannelName\":\"channel.input.analyser1\",    \"updateChannelName\":\"channel.update.analyser1\",    \"attributes\":{\"outputFormat\":{\"p1\":\"v1\"}}}'
./ConsoleApi.exe localhost:9094 produce job_management.registration.request '{    \"componentType\":\"DATA_ACQUIRER\",    \"componentId\":\"acquirer1\",    \"inputChannelName\":\"channel.input.acquirer1\",    \"updateChannelName\":\"channel.update.acquirer1\",    \"attributes\":{\"foo\":2}}'


$body = @{ "selectedDataAnalysers"=@("analyser1"); "selectedDataAcquirers"=@("acquirer1");    "topicQuery"="query1";    "language"="en";    "jobName" = "name";    "attributes" = @{        "attribute1"="aaa"    }}|ConvertTo-Json
$response = Invoke-WebRequest -Method Post -Body $body -ContentType "application/json"   "http://localhost:6009/api/job/submit"
$js =  ConvertFrom-Json $response.Content

Invoke-WebRequest "http://localhost:8888/jobs/$($js.jobId)"