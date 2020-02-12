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

    def get_message_from_topic(self, topic, repeat_times = 10):
        consumer = KafkaConsumer(
            bootstrap_servers=self._server_address, 
            auto_offset_reset='earliest',
            group_id = 'tester_consumer_'+topic,
            consumer_timeout_ms=1000)

        consumer.subscribe([topic])
        i =0
        
        while repeat_times==0 or i < repeat_times:
            for msg in consumer:
                try:
                    payload = msg.value
                    return json.loads(payload)    
                except Exception as ex:
                    print(ex)
                    return None
                finally:
                    consumer.close()                    
            i+=1
    
    def register_component(
        self,
        topic = "job_management.registration.request",
        component_id= "test_acquirer_id",
        component_type = "DATA_ACQUIRER",
        input_channel_name = "input_channel",
        update_channel_name = "update_channel",
        attributes = {}):
   
        request = {
            "componentType" : component_type,
            "componentId":component_id,
            "inputChannelName":input_channel_name,
            "updateChannelName":update_channel_name,
            "attributes":attributes
        }

        self.produce_request(request ,topic)

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
            'outputChannelNames':outputMessageBrokerChannels
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

    def __del__(self):
        self.producer.close()  