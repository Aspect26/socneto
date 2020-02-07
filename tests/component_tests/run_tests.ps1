echo "testing job management service"
. ./run_test_jms.ps1

echo "testing data acquirers"
. ./run_test_da.ps1

echo "testing analysers"
. ./run_test_an.ps1