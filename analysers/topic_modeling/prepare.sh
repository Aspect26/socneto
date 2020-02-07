set -e
pip install --default-timeout=120 --no-cache-dir --trusted-host pypi.python.org -r requirements.txt
wget -O en_core_web_lg-2.2.5.tar.gz https://github.com/explosion/spacy-models/releases/download/en_core_web_lg-2.2.5/en_core_web_lg-2.2.5.tar.gz
pip install en_core_web_lg-2.2.5.tar.gz

