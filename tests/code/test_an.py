import sys
import json
import kafka_client
import utils

class AnalyserTestCases:
    def __init__(self, kafka):
        self.kafka = kafka

    def test_analysis(
            self,
            topic,
            output_topic="job_management.component_data_analyzed_input.storage_db"):
        try:
            print("Produce post")
            self.kafka.produce_post(
                topic, text="In machine learning and natural language processing, a topic model is a type of statistical model for discovering the abstract 'topics' that occur in a collection of documents. Topic modeling is a frequently used text-mining tool for discovery of hidden semantic structures in a text body.")
            print("Post produced - waiting for the analysis")
            post = self.kafka.get_message_from_topic(output_topic)
            print(post)
            if post is not None and 'results' in post:
                # print(post)
                return (True, None)
            else:
                return (False,"No post was returned")

        except Exception as e:
            return (False, "Testing analyser failed: {}".format(e))

    def test(self, topic):        
        (result, message) = self.test_analysis(topic)

        if not result:
            error = "test {} failed: {}".format(
                'test_analysis', message)
            print(error)
            raise Exception(error)
        else:
            print("success")


def start_test(config_path, topic):
    
    config=utils.load_config(config_path)
    kafka_server = config['kafka']['kafka_server']

    print(topic)
    print("Kafka server : '{}'".format(kafka_server))

    kafka = kafka_client.KafkaClient(kafka_server)
    testcases = AnalyserTestCases(kafka)
    testcases.test(topic)

if __name__ == "__main__":
    if(len(sys.argv) == 3):
        config_path = sys.argv[1]
        topic = sys.argv[2]
        start_test(config_path, topic)
    else:
        raise Exception("Please include topic argument")
