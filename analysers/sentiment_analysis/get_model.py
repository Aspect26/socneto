from minio import Minio
from minio.error import ResponseError
import sys

import os.path
from os import path

client = Minio('acheron.ms.mff.cuni.cz:39111',
               access_key='socnetoadmin',
               secret_key='Tajn0Heslo',
               secure = False)

# Get a full object
model_name = sys.argv[1]

if path.exists(model_name):
    print("Model already downloaded")
    sys.exit()

try:
    print("Downloading model (500MiB)")
    client.fget_object('models', model_name, model_name)
except ResponseError as err:
    print(err)

