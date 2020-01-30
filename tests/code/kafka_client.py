from kafka import KafkaConsumer, KafkaProducer
import uuid
import json

class KafkaClient:
    def __init__(self,server_address):
        self._server_address = server_address
        self.producer = KafkaProducer(bootstrap_servers=server_address)

    def produce_request(self, request,topic):
        json_request = json.dumps(request)
        post_bytes = json_request.encode('utf-8')
        future = self.producer.send(topic, post_bytes)
        future.get(timeout=60)

    def consume_topic(self, topic):
        consumer = KafkaConsumer( topic, bootstrap_servers=self._server_address)
        while True:
            for msg in consumer:
                try:
                    payload = msg.value
                    yield json.loads(payload)                    
                except Exception as ex:
                    print(ex)
            


    def produce_start_job(self,
                        topic,
                        outputMessageBrokerChannels = [],
                        outputMessageBrokerChannel = None,
                        jobId = "e3a7ce8b-2d10-46ff-ba91-925a5a99a3e9",
                        attributes = {} ):
        
        if( outputMessageBrokerChannel is not None):
            outputMessageBrokerChannels = [outputMessageBrokerChannel]
        
        request = {        
            'jobId':jobId,
            'command':'Start',
            'attributes':attributes,
            'outputMessageBrokerChannels':outputMessageBrokerChannels
        }
        self.produce_request(request,topic)

    def produce_stop_job(self,
        topic,
        jobId = "e3a7ce8b-2d10-46ff-ba91-925a5a99a3e9"):

        request = {        
            'jobId':jobId,
            'command':'Stop'
        }
        self.produce_request(request,topic)

    def produce_post(
        self, 
        topic, 
        originalPostId = "1234567",
        postId = "0bb89a04-2f35-41bc-aa17-3d9dc159052d",
        jobId = "e3a7ce8b-2d10-46ff-ba91-925a5a99a3e9",
        text="test text",
        originalText = None,
        language = "en",
        source = "manual",
        authorId = "0",
        dateTime = "2020-01-01T00:00"):
        
        request={
            "originalPostId":originalPostId,
            "postId":postId,
            "jobId":jobId,
            "text":text,
            "originalText":originalText,
            "language":language,
            "source":source,
            "authorId":authorId,
            "dateTime":dateTime
        }
        self.produce_request(request,topic)