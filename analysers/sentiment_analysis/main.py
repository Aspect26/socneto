#!/usr/bin/python
import argparse
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
formatter = logging.Formatter(
    '%(asctime)s - %(name)s - %(levelname)s - %(message)s')
fh.setFormatter(formatter)
ch.setFormatter(formatter)
logger.addHandler(fh)
logger.addHandler(ch)

model_path = "distilbert_predictor"
analysis = a.Analysis(model_path)


def register_itself(topic, input_topic, componentId, producer):
    request = {
        "ComponentId": componentId,
        "ComponentType": "DATA_ANALYSER",
        "UpdateChannelName": "job_management.job_configuration.DataAnalyser_sentiment",
        "InputChannelName": input_topic,
        "attributes": {
            "outputFormat": {
                "polarity": "numberValue",
                "accuracy": "numberValue"
            }
        }
    }

    json_request = json.dumps(request)
    post_bytes = json_request.encode('utf-8')
    future = producer.send(topic, post_bytes)
    future.get(timeout=60)
    logger.info("Sent registration request: " + json_request)


def analyse(text):
    polarity, confidence = analysis.get_text_sentiment(text)
    return {
        "polarity": {
            "numberValue": polarity
        },
        "accuracy": {
            "numberValue": confidence
        }
    }


def process_acquired_data(config, producer):
    consumer = KafkaConsumer(
        config['input_topic'], bootstrap_servers=config['kafka_server'])

    while True:
        try:
            for msg in consumer:
                try:
                    # validate that message is unipost
                    payload = msg.value
                    logger.info("received {}".format(payload))
                    post = json.loads(payload)
                    text = post["text"]
                    analysis = analyse(text)
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
                    logger.info("analysis. Text:{} results:{}".format(
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
                    default="job_management.component_data_input.DataAnalyser_sentiment")
parser.add_argument('--output_topic', type=str, required=False, help='address of the kafka server',
                    default="job_management.component_data_analyzed_input.storage_db")
parser.add_argument('--registration_topic', type=str, required=False,
                    help='address of the kafka server', default="job_management.registration.request")
parser.add_argument('--component_id', type=str, required=False,
                    help='id of the component', default="DataAnalyser_sentiment")
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
    print("Waiting 30 seconds")
    time.sleep(30)

main(default_config)
