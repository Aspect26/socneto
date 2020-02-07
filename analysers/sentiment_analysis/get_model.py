from minio import Minio
from minio.error import ResponseError

client = Minio('acheron.ms.mff.cuni.cz:39107',
               access_key='socnetoadmin',
               secret_key='Tajn0Heslo',
               secure = False)

# Get a full object
model_name = "model_1574374374.795886.bin"
try:
    print("Downloading model (500MiB)")
    client.fget_object('models', model_name, model_name)
except ResponseError as err:
    print(err)