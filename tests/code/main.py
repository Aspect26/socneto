import json
import kafka_client
import utils
from da_testcase import test_data_acquisition

class SocnetoTestCases:    
    def __init__(self, 
                kafka,
                data_acquirer_job_update_topic,
                credentials):
        self.cred=credentials
        self.kafka = kafka
        self.data_acquirer_job_update_topic = data_acquirer_job_update_topic
        
    def test_data_acquirer(self):
        attributes = dict({'TopicQuery':'test'}, **self.cred)
        output_topic = 'my_test_topic'
        jobupdatetopic = self.data_acquirer_job_update_topic
        (result,message) = test_data_acquisition(self.kafka,jobupdatetopic,output_topic,attributes = attributes)

        if not result:
            print("test {} failed: {}".format( 'test_data_acquisition', message))
        else:
            print("success")


def test(config, kafka,credentials):
    
    jobupdatetopic = config['topics']['data_acquirer_update_topic']
    testcases = SocnetoTestCases(kafka,jobupdatetopic, credentials)
    
    testcases.test_data_acquirer()
    #  TODO other tests

if __name__ == "__main__": 
    config=utils.load_config("config.json")
    kafka = kafka_client.KafkaClient(config['kafka']['kafka_server'])
    cred_path = "./cred.json"
    credentials=utils.resolve_credentials(cred_path)
    test(config, kafka, credentials)


