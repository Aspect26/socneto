import sys
import json
import kafka_client
import uuid
import utils
import requests
from threading import Thread
import time
import traceback

class JmsTestCases:    
    def __init__(self, kafka, jms_registration_topic):
        self.jms_registration_topic = jms_registration_topic
        self.kafka = kafka
    
    def test(
        self,
        jms_submit_job_uri):
        
        current_uuid = str(uuid.uuid4())
                
        print( "Registering new components")
        acquirer_component_id ="test_component_acquirer_"+current_uuid
        acquirer_update_channel = "topic_"+acquirer_component_id
        registration_request_da = self._register_acquirer(acquirer_component_id, acquirer_update_channel)
        
        analyser_component_id = "test_component_analyser_"+current_uuid
        analyser_update_channel = "topic_"+analyser_component_id        
        registration_request_an = self._register_analyser(analyser_component_id, analyser_update_channel)
        
        time.sleep(5)
        
        print("Submit job config")
        job_submit_request = {  
            "selectedDataAnalysers": [ analyser_component_id ],  
            "selectedDataAcquirers": [ acquirer_component_id ],  
            "topicQuery": "test query",  
            "jobName": "my test job",  
            "language": None,  
            "attributes": { "att1":"value1", "att2":"value2" }
        }
        data = json.dumps(job_submit_request)
        r = requests.post(jms_submit_job_uri, data)
        
        job_id = None
                
        print("Assert received job configs for each component")
        
        try:
            job_id = self._assert_data_acquirer_job_submit(
                job_submit_request["topicQuery"],
                registration_request_an['inputChannelName'],
                acquirer_update_channel
            )
        except Exception:
            return (False, traceback.format_exc())
             
        try:
            self._assert_analyser_job_submit(analyser_update_channel, job_id)
        except Exception:
            return (False, traceback.format_exc())
        
        print("Assert replayed data acquirer config")
        registration_request_da = self._register_acquirer(acquirer_component_id, acquirer_update_channel)
        try:
            job_id = self._assert_data_acquirer_job_submit(
                job_submit_request["topicQuery"],
                registration_request_an['inputChannelName'],
                acquirer_update_channel
            )
        except Exception:
            return (False, traceback.format_exc())
        
        registration_request_an = self._register_analyser(analyser_component_id, analyser_update_channel)
        try:
            self._assert_analyser_job_submit(analyser_update_channel, job_id)
        except Exception:
            return (False, traceback.format_exc())
        
        return (True,None)
    
    def _register_acquirer(self, acquirer_component_id, acquirer_update_channel):
        registration_request_da ={
            "componentId" : acquirer_component_id,
            "componentType" : "DATA_ACQUIRER",
            "updateChannelName" : acquirer_update_channel,
            "inputChannelName" : "n/a" 
        }
        self.kafka.produce_request(registration_request_da, self.jms_registration_topic)
        return registration_request_da
    
    def _register_analyser(self, analyser_component_id, analyser_update_channel):
        registration_request_an ={
            "componentId" : analyser_component_id,
            "componentType" : "DATA_ANALYSER",
            "attributes" : {
                "outputFormat" :{
                    "o1" : "numberValue"
                }
            },
            "updateChannelName" : analyser_update_channel,
            "inputChannelName" : "test_input_channel_analyser" 
        }
        self.kafka.produce_request(registration_request_an, self.jms_registration_topic)
        return registration_request_an
        
    def _assert_data_acquirer_job_submit(self,
                                         topic_query,
                                         analyser_input_channel_name,
                                         acquirer_update_channel):
        print("Asserting data acquirer job config")
        job_config = self.kafka.get_message_from_topic(acquirer_update_channel)
        if job_config is None:
            raise Exception("No job config received")
        if "jobId" not in job_config:
            raise Exception("Job config has no job id")

        self._assert_value(job_config['attributes']['TopicQuery'], topic_query)
        self._assert_in(analyser_input_channel_name, job_config['outputChannelNames'])

        return job_config["jobId"]
        
    def _assert_analyser_job_submit(self,
                                   analyser_update_channel,
                                   job_id):
        print("Asserting analyser job config")
            
        job_config = self.kafka.get_message_from_topic(analyser_update_channel)
        if job_config is None:
            raise Exception("No job config received")
        self._assert_value(job_config['jobId'], job_id)
        
                                    
    def _assert_value(self, a,b):
        if(a!=b):
            raise Exception("AssertError - Invalid value. actual:{} expected:{}".format(a,b))
            
    def _assert_in(self, a,b):
        if(a not in b):
            raise Exception("AssertError - Value not in array value:{} array:{}".format(a,b))


def start_test(config_path = "../config.json"):
    config=utils.load_config(config_path)
    kafka = kafka_client.KafkaClient(config['kafka']['kafka_server'])
    jms_registration = config['topics']['registration_channel_name']
    
    
    jms_tests = JmsTestCases(kafka,jms_registration)
    jms_submit_uri = config['uris']['jms_submit_uri']
    success,message = jms_tests.test(jms_submit_uri)
    if not success:
        error ="test {} failed: {}".format( 'jms_test', message)
        print(error)
        raise Exception(error)
    else :
        print("SUCCESS")
    

if __name__ == "__main__":
  if(len(sys.argv)==2):
      config_path = sys.argv[1]
      start_test(config_path);
  else:
      start_test();
