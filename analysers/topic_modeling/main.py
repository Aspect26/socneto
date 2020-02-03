#!/usr/bin/python
import argparse
from LDA import LDAAnalysis
import logging
from kafka import KafkaConsumer, KafkaProducer
import json
import time
from datetime import datetime
import os.path
import numpy as np

logger = logging.getLogger('topic_modelling')

logger.setLevel(logging.INFO)
ts = datetime.now().strftime('%Y-%m-%dT%H%M%S')
fh = logging.FileHandler('topic_modelling' + ts + '.log')
fh.setLevel(logging.DEBUG)
ch = logging.StreamHandler()
ch.setLevel(logging.INFO)
formatter = logging.Formatter(
    '%(asctime)s - %(name)s - %(levelname)s - %(message)s')
fh.setFormatter(formatter)
ch.setFormatter(formatter)
logger.addHandler(fh)
logger.addHandler(ch)

def analyse_text(lda, text):
    a = lda.get_topic_keywords(text)

    dic = {}
    for topic,prob in a:
        if topic not in dic:
            dic[topic]=prob
        # else:
        #     dic[topic] = dic[topic] + prob

    # topics =np.array(a[0])
    # probs = np.array(a[1])
    # zipped = zip(topics.flatten(), probs.flatten())


    return list(dic.keys())

def register_itself(topic, input_topic, componentId, producer):
    request = {
        "ComponentId": componentId,
        "ComponentType": "DATA_ANALYSER",
        "UpdateChannelName": "job_management.job_configuration.DataAnalyser_topics",
        "InputChannelName": input_topic,
        "attributes": {
            "outputFormat": {
                "topics": "textListValue"
            }
        }
    }

    json_request = json.dumps(request)
    post_bytes = json_request.encode('utf-8')
    future = producer.send(topic, post_bytes)
    future.get(timeout=60)
    logger.info("Registration request sent: " + json_request)


def analyse(lda, text):
    array = analyse_text(lda,text)
    return {
        "topics": {
            "textListValue": array
        },
    }

def process_acquired_data(config, producer):
    consumer = KafkaConsumer(
        config['input_topic'], bootstrap_servers=config['kafka_server'])

    lda = LDAAnalysis()
    while True:
        try:
            for msg in consumer:
                try:
                    # validate that message is unipost
                    payload = msg.value
                    logger.info("received {}".format(payload))
                    post = json.loads(payload)
                    text = post["text"]
                    analysis = analyse(lda,text)
                    result = {
                        "postId": post["postId"],
                        "jobId": post["jobId"],
                        "componentId": config["componentId"],
                        "results": analysis
                    }
                    
                    json_result = json.dumps(result)
                    bytes_analysis = json_result.encode('utf-8')
                    future = producer.send(
                        config['output_topic'], bytes_analysis)
                    future.get(timeout=60)
                    logger.info("Analysis produced Text:{} results:{}".format(
                        text, json_result))
                except Exception as e:
                    logger.error(e)

        except Exception as e:
            print(e)
            time.sleep(5)


def main(config):
    logger.info("input config: {}".format(config))

    producer = None
    while True:
        try:
            producer = KafkaProducer(bootstrap_servers=config['kafka_server'])
            logger.info("producer connected")
            break
        except Exception as e:
            print(e)

    register_itself(config['registration_topic'],
                    config['input_topic'],
                    config['componentId'],
                    producer)

    process_acquired_data(config, producer)


parser = argparse.ArgumentParser(description='Configure kafka options')
parser.add_argument('--server_address', type=str, required=False,
                    help='address of the kafka server', default="localhost:9094")
parser.add_argument('--input_topic', type=str, required=False, help='name of the input topic',
                    default="job_management.component_data_input.DataAnalyser_topics")
parser.add_argument('--output_topic', type=str, required=False, help='address of the kafka server',
                    default="job_management.component_data_analyzed_input.storage_db")
parser.add_argument('--registration_topic', type=str, required=False,
                    help='address of the kafka server', default="job_management.registration.request")
parser.add_argument('--component_id', type=str, required=False,
                    help='id of the component', default="DataAnalyser_topics")
parser.add_argument('--sleep_on_startup', action='store_true', required=False,
                    help='id of the component', default=False)

args = parser.parse_args()

default_config = {
    "input_topic": args.input_topic,
    "output_topic": args.output_topic,
    "kafka_server": args.server_address,
    "registration_topic": args.registration_topic,
    "componentId": args.component_id
}

if args.sleep_on_startup:
    print("Waiting a while before registering")
    time.sleep(180)

main(default_config)
