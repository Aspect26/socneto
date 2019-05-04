from kafka import KafkaConsumer
from json import loads


class Consumer:
    def __init__(self, server, topic):
        self._server = server
        self._topic = topic
        self._logger = lambda x: x
        self.consumer = KafkaConsumer(
            self._topic,
            bootstrap_servers=[self._server],
            auto_offset_reset='earliest',
            enable_auto_commit=True,
            group_id='analyser-group',
            value_deserializer=lambda x: self._deserialize_record(x))

    def _deserialize_record(self, record):
        try:
            byte_array = record
            self._logger(byte_array)
            text_json = byte_array.decode('utf-8')
            self._logger(text_json)
            js = loads(text_json)
            self._logger(js)
            return js
        except:
            self._logger("unable to decode {}".format(record))
            return None

    def consume(self, on_consume_action):
        self._logger("consuming at {} started".format(self._topic))

        for message in self.consumer:
            if message.value is None:
                self._logger("None message encountered!")
                continue
            self._logger("consumed: {}".format(message.value))
            on_consume_action(message.value)

        self._logger("stopped consuming")

    def subscribe_logging(self, logger):
        self._logger = logger
