import sys
import json
import kafka_client
import utils

class DataAcquirerTestCases:    
    def __init__(self, 
                kafka,
                data_acquirer_job_update_topic,
                credentials):
        self.cred=credentials
        self.kafka = kafka
        self.data_acquirer_job_update_topic = data_acquirer_job_update_topic

    def test_data_acquisition(
        self,
        posts_output_topic,
        jobId = "e3a7ce8b-2d10-46ff-ba91-925a5a99a3e9",
        attributes = {}):

        try:
            print("Create new job")
            self.kafka.produce_start_job(
                            self.data_acquirer_job_update_topic,
                            outputMessageBrokerChannel = posts_output_topic,
                            jobId=jobId,
                            attributes = attributes)

            print("Waits for acquired posts")        
            for post in self.kafka.consume_topic(posts_output_topic):
                keys = [
                    'originalPostId', 
                    'postId', 
                    'jobId', 
                    'text', 
                    'originalText', 
                    'language', 
                    'source', 
                    'authorId','dateTime'
                ]
                print("Check that the message is in the correct format")            
                for k in keys:
                    if k not in post:
                        raise Exception("Element {} not found in post {}".format(k,post))
                return (True,None)
            
        except Exception as e:
            return (False, "Testing data acquirer failed: {}".format(e))
        finally:
            print("Stop the job")
            self.kafka.produce_stop_job(self.data_acquirer_job_update_topic,jobId)
        
    def test(self):
        attributes = dict({'TopicQuery':'test'}, **self.cred)
        output_topic = 'my_test_topic'
        (result,message) = self.test_data_acquisition(output_topic,attributes = attributes)

        if not result:
            print("test {} failed: {}".format( 'test_data_acquisition', message))
        else:
            print("success")


def start_test():
    config_path = "./config.json"
    cred_path = "./cred.json"
    if(len(sys.argv)==3):
        config_path = sys.argv[1]
        cred_path = sys.argv[2]

    config=utils.load_config(config_path)
    kafka = kafka_client.KafkaClient(config['kafka']['kafka_server'])
    credentials=utils.resolve_credentials(cred_path)
    jobupdatetopic = config['topics']['data_acquirer_update_topic']
    testcases = DataAcquirerTestCases(kafka,jobupdatetopic, credentials)

    testcases.test()

start_test();




