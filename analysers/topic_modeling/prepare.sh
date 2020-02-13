set -e

echo "installing requirements"
pip install --default-timeout=120 --no-cache-dir --trusted-host pypi.python.org -r requirements.txt

echo "downloading model"
wget -O en_core_web_lg-2.2.5.tar.gz https://github.com/explosion/spacy-models/releases/download/en_core_web_lg-2.2.5/en_core_web_lg-2.2.5.tar.gz

echo "installing en_core_web_lg"
pip install en_core_web_lg-2.2.5.tar.gz

