#!/usr/bin/python
import Analysis as a
import logging
from kafka import KafkaConsumer, KafkaProducer
import json
import time
from datetime import datetime

logger = logging.getLogger('sentiment_analysis')

logger.setLevel(logging.INFO)
ts = datetime.now().strftime('%Y-%m-%dT%H%M%S')
fh = logging.FileHandler('sentiment_analyser' + ts + '.log')
fh.setLevel(logging.DEBUG)
ch = logging.StreamHandler()
ch.setLevel(logging.INFO)
formatter = logging.Formatter('%(asctime)s - %(name)s - %(levelname)s - %(message)s')
fh.setFormatter(formatter)
ch.setFormatter(formatter)
logger.addHandler(fh)
logger.addHandler(ch)

model_path = "model_1574374374.795886.bin"
analysis = a.Analysis(model_path)

def register_itself(topic, input_topic,producer):
    request =     {
        "ComponentId": "DataAnalyser_sentiment",
        "ComponentType": "DATA_ANALYSER",
        "UpdateChannelName": "job_management.job_configuration.DataAnalyser_sentiment",
        "InputChannelName": input_topic
    }
    producer = KafkaProducer(bootstrap_servers=config['kafka_server'])
    
    json_request = json.dumps(request)
    post_bytes = json_request.encode('utf-8')
    future = producer.send(topic,post_bytes)
    future.get(timeout=60)
    logger.info("Sent registration request: " + json_request)
    
def analyse(text):
    polarity, confidence = analysis.get_text_sentiment(text)
    return {
        "sentiment_analysis":{
            "polarity":{
                "value":polarity,
                "type":"number"
            },
            # "strength":{
            #     "value":0.1,
            #     "type":"number"
            # },
            "confidence":{
                "value":confidence,
                "type":"number"
            }
        }
    }

def process_acquired_data(config, producer):
    consumer = KafkaConsumer(config['input_topic'],bootstrap_servers=config['kafka_server'])
    
    while True:
        try:
            for msg in consumer:     
                try:
                    #validate that message is unipost
                    payload = msg.value
                    logger.info("received {}".format(payload))
                    post = json.loads(payload)
                    analysis= analyse(post["Text"])
                    json_analysis = json.dumps(analysis)
                    logger.info("analysed {}".format(json_analysis))
                    bytes_analysis = json_analysis.encode('utf-8')
                    future = producer.send(config['output_topic'],bytes_analysis)
                    future.get(timeout=60)
                except Exception as e:
                    logger.error(e)
                
        except Exception as e:
            print(e)
            time.sleep(5)
    
def main(config):
    producer = KafkaProducer(bootstrap_servers=config['kafka_server'])

    register_itself(config['registration_topic'], config['input_topic'], producer)

    process_acquired_data(config,producer)

config = {
    "input_topic":"job_management.component_data_input.DataAnalyser_sentiment",
    "output_topic":"job_management.component_data_analyzed_input.storage_db",
    "kafka_server":"localhost:9094",
    "registration_topic":"job_management.registration.request"
}

main(config)

