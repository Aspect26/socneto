import uuid
import json
import requests

class StorageClient:
    def __init__(self,server_uri):
        self._server_uri = server_uri
    
    def get_components(self):
        uri = "{}/components".format(self._server_uri)
        r = requests.get(uri)
        # todo unsuccessful code
        return  r.text
    
    def get_component(self,component_id):
        uri = "{}/components/{}".format(self._server_uri,component_id)
        r = requests.get(uri)
        
        if r.status_code == 404:
            return None
        
        return  r.text
    
    def insert_component(self, 
             component_id,
             component_type = 'DATA_ACQUIRER',
             input_channel = 'input_channel.test',
             update_channel = 'update_channel.test',
             attributes = {} ) :
        component={
            "componentId": component_id,
            "type": component_type,
            "inputChannelName": input_channel,
            "updateChannelName": update_channel,
            "attributes": attributes
        }
        
        uri = "{}/components".format(self._server_uri)
        body =json.dumps(component)
        print(body)
        headers = {'Content-type': 'application/json'}
        r = requests.post(uri,body,headers=headers)
        
        if r.status_code != 200:
            raise Exception(r.text)
    
    def get_jobs(self):
        uri = "{}/jobs".format(self._server_uri)
        r = requests.get(uri)
        # todo unsuccessful code
        return  r.text
    
    def get_job(self, job_id):
        uri = "{}/jobs/{}".format(self._server_uri, job_id)
        r = requests.get(uri)
        if r.status_code == 404:
            return None
        
        return  r.text
    
    def insert_job(self, 
            job_name = "test job name",
            user_name="test_user",
            finished_date = None,
            started_date= "2020-02-02T02:02:00",
            topic_query="foo bar query",
            language=None,
            status="Running"):
        
        job_id = str(uuid.uuid4())
        job={
            "jobId":job_id,
            "jobName":job_name,
            "username":user_name,
            "finished":finished_date,
            "startedAt":started_date,
            "topicQuery":topic_query,
            "language":language,
            "status":status,
        }
        
        uri = "{}/jobs".format(self._server_uri)
        body =json.dumps(job)
        headers = {'Content-type': 'application/json'}
        r = requests.post(uri,body,headers=headers)
        
        if r.status_code != 200:
            raise Exception(r.text)
        return job_id
    
    def update_job(self,c ):
        job={
            # TODO
        }
        
        uri = "{}/jobs".format(self._server_uri)
        body =json.dumps(job)
        headers = {'Content-type': 'application/json'}
        r = requests.put(uri,body,headers=headers)
        
        if r.status_code != 204:
            raise Exception(r.text)
    
    def get_component_config(self, component_id):
        uri = "{}/components/{}/configs".format(self._server_uri, component_id)
        r = requests.get(uri)
        
        if r.status_code == 404:
            return None
    
    def insert_component_config(self, component_id):
        component_config = {
            
        }
        
        uri = "{}/components/{}/configs".format(self._server_uri, component_id)
        body =json.dumps(component_config)
        headers = {'Content-type': 'application/json'}
        r = requests.put(uri,body,headers=headers)
        
        if r.status_code != 204:
            raise Exception(r.text)
    