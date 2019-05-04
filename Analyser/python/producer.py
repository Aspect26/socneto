from json import dumps
from kafka import KafkaProducer


class Producer:
    def __init__(self, server, topic):
        self._server = server
        self._topic = topic
        self._logger = lambda x: x
        self.producer = KafkaProducer(bootstrap_servers=[server],
                                      value_serializer=lambda x: dumps(x).encode('utf-8'))

    def produce(self, data_dic):
        self._logger("producing to {}".format(self._topic))
        self.producer.send(self._topic, value=data_dic)
        self._logger("produced {}".format(data_dic))

    def subscribe_logging(self, logger):
        self._logger = logger
