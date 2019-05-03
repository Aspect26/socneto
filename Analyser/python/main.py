from time import sleep
import consumer
import producer
import argparse

if __name__ != "__main__":
    exit()

parser = argparse.ArgumentParser()
parser.add_argument("--server", type=str, help="kafka server")
parser.add_argument("--consume_topic", type=str, help="consumer topic name")
parser.add_argument("--produce_topic", type=str, help="producer topic name")

args = parser.parse_args()

kafka_producer = producer.Producer(args.server, args.produce_topic)
kafka_producer.subscribe_logging(lambda x: print(x))


def produce_mock_sentiment(data):
    # TODO add actual sentiment implementation
    sentiment_dic = {
        0: "negative",
        1: "positive"
    }
    sentiment_value = len(str(data)) % 2

    analysed_data = {
        "originalData": data,
        "analysis": {
            "sentimentText": sentiment_dic[sentiment_value],
            "sentimentVal": sentiment_value,
        }
    }

    kafka_producer.produce(analysed_data)


kafka_producer.produce({"info":"test produce"})

kafka_consumer = consumer.Consumer(args.server, args.consume_topic)
kafka_consumer.subscribe_logging(lambda x: print(x))
while True:
    kafka_consumer.consume(produce_mock_sentiment)
    sleep(1)
