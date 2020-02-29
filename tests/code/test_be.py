import sys
import json
import time

from requests import Response

import kafka_client
import utils
import requests
import base64


class BackendTestCases:

    _MOCK_ACQUIRER_ID = "acquirer"
    _MOCK_ANALYZER_ID = "analyzer"

    def __init__(self, config):
        self.kafka = kafka_client.KafkaClient(config['kafka']['kafka_server'])
        self.jms_registration_topic = config['topics']['registration_channel_name']

    def test(self, config: dict) -> None:
        print("Waiting for platform components to start - 15 sec")
        time.sleep(15)

        print("registering components")
        self._register_components()

        self._test_heart_beat(config["uris"]["backend_heart_beat"])
        print("SUCCESS - heart beat")

        self._test_platform_status(config["uris"]["backend_platform_status"])
        print("SUCCESS - platform status")

        self._test_login(config["uris"]["backend_login"])
        print("SUCCESS - login")

        self._test_create_job(config["uris"]["backend_create_job"])
        print("SUCCESS - create job")

    def _register_components(self):
        acquirer_update_channel = f"topic_{self._MOCK_ACQUIRER_ID}"
        self._register_acquirer(self._MOCK_ACQUIRER_ID, acquirer_update_channel)

        analyser_update_channel = f"topic_{self._MOCK_ANALYZER_ID}"
        self._register_analyser(self._MOCK_ANALYZER_ID, analyser_update_channel)

        time.sleep(10)

    def _register_acquirer(self, acquirer_component_id, acquirer_update_channel):
        registration_request_da = {
            "componentId": acquirer_component_id,
            "componentType": "DATA_ACQUIRER",
            "updateChannelName": acquirer_update_channel,
            "inputChannelName": "n/a"
        }
        self.kafka.produce_request(registration_request_da, self.jms_registration_topic)
        return registration_request_da

    def _register_analyser(self, analyser_component_id, analyser_update_channel):
        registration_request_an ={
            "componentId": analyser_component_id,
            "componentType": "DATA_ANALYSER",
            "attributes": {
                "outputFormat": {
                    "o1": "numberValue"
                }
            },
            "updateChannelName": analyser_update_channel,
            "inputChannelName": "test_input_channel_analyser"
        }
        self.kafka.produce_request(registration_request_an, self.jms_registration_topic)
        return registration_request_an

    def _test_heart_beat(self, route: str) -> None:
        response = self._http_get(route)
        response_json = response.content.decode()
        response_dict = json.loads(response_json)

        self._assert_not_none(response_dict["timeStamp"])

    def _test_platform_status(self, route: str):
        response = self._http_get(route)
        response_json = response.content.decode()
        response_dict = json.loads(response_json)

        self._assert_in("jms", response_dict)
        self._assert_in("storage", response_dict)
        self._assert_value(response_dict["jms"], "RUNNING")
        self._assert_value(response_dict["storage"], "RUNNING")

    def _test_login(self, route: str) -> None:
        body = {"username": "admin", "password": "admin"}
        response = self._http_post(route, body)
        response_json = response.content.decode()
        response_dict = json.loads(response_json)

        self._assert_in("username", response_dict)
        self._assert_value(response_dict["username"], "admin")

    def _test_create_job(self, route: str) -> None:
        body = {"job_name": "HammerFall", "topic_query": "hammerfall", "selected_acquirers": [self._MOCK_ACQUIRER_ID], "selected_analysers": [self._MOCK_ANALYZER_ID]}
        response = self._http_post_authenticated(route, body)
        response_json = response.content.decode()

        print(response_json)
        print(response.status_code)

        response_dict = json.loads(response_json)

        self._assert_not_none(response_dict["job_id"])
        self._assert_value(response_dict["status"], "Running")

    @staticmethod
    def _http_post(route: str, data: dict) -> Response:
        body = json.dumps(data)
        return requests.post(route, body, headers={"Content-Type": "application/json"})

    @staticmethod
    def _http_post_authenticated(route: str, data: dict) -> Response:
        body = json.dumps(data)
        auth_token = base64.b64encode(b'admin:admin').decode("utf-8")
        return requests.post(route, body, headers={"Authorization": f"Basic {auth_token}", "Content-Type": "application/json" })

    @staticmethod
    def _http_get(route: str) -> Response:
        return requests.get(route)

    @staticmethod
    def _assert_value(a, b):
        if a != b:
            raise Exception(f"AssertError - Invalid value. actual:{a} expected:{b}")

    @staticmethod
    def _assert_in(a, b):
        if a not in b:
            raise Exception(f"AssertError - Value not in value:{a} array:{b}")

    @staticmethod
    def _assert_not_none(a):
        if a is None:
            raise Exception(f"AssertError - Value is None:{a}")


def start_test(config_path="../config.json") -> None:
    config = utils.load_config(config_path)

    be_tests = BackendTestCases(config)
    be_tests.test(config)

    print("SUCCESS ALL")


if __name__ == "__main__":
    if len(sys.argv) == 2:
        config_path = sys.argv[1]
        start_test(config_path)
    else:
        start_test()
