from storage_client import StorageClient

server_uri= "http://localhost:8888"
storage = StorageClient(server_uri)

print(storage.get_component("my_test"))
print(storage.insert_component("my_test"))
print(storage.get_components())
## jobs
print(storage.get_jobs())
job_id=storage.insert_job()
print(job_id)
print(storage.get_job(job_id))
