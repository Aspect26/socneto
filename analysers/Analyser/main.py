from time import sleep
import json
import threading
import consumer
import producer
import Analyser.language_recognition.language_recognizer as lr
import Analyser.sentiment_analysis.sentiment_analyser as sa

if __name__ != "__main__":
    exit()

with open('config.json', 'r') as file:
    config = json.load(file)
serverAddress = config['kafkaOptions']['serverAddress']
registrationTopicName= config['registrationRequestOptions']['registrationChannelName']

kafka_producer = producer.Producer(serverAddress)
update_channel_name = config["componentOptions"]["updateChannelName"]

kafka_producer.subscribe_logging(lambda x: print(x))

# the similarity is not intentional
registrationRequest = config['componentOptions']

kafka_producer.produce(registrationTopicName, registrationRequest)

def analyse_post(data):
    text = data['text']
    sentiment_dic = {
        0: "negative",
        1: "positive",
        2: "neutral"
    }
    sentiment_value = sa.get_sentiment(text)

    language = lr.detect_language(text)

    return {
        "originalData": data,
        "analysis": {
            "sentimentText": sentiment_dic[sentiment_value],
            "sentimentVal": sentiment_value,
            "keywords":["kw1","kw2"],
            "language": language
        }
    }

def produce_analysis(post):
    analysed = analyse_post(post)
    producer.produce(update_channel_name, analysed)

def process_submitted_job(job_config):
    def start_job():
        input_channel = job_config['inputMessageBrokerChannel']
        job_consumer = consumer.Consumer(serverAddress, input_channel)
        while True:
            job_consumer.subscribe_logging(lambda x: print(x))
            job_consumer.consume(produce_analysis)
    thread1 = threading.Thread(target = start_job)
    thread1.start()

config_consumer = consumer.Consumer(serverAddress, update_channel_name)
while True:
    config_consumer.subscribe_logging(lambda x: print(x))
    config_consumer.consume(process_submitted_job)

