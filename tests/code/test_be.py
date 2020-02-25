import sys
import json
import time

from requests import Response

import utils
import requests


class BackendTestCases:

    def test(self, config: dict) -> None:
        print("Waiting for platform components to start - 15 sec")
        time.sleep(15)

        self._test_platform_status(config["uris"]["backend_platform_status"])
        print("SUCCESS - platform status")

    def _test_platform_status(self, route: str):
        response = self._http_get(route)
        response_json = response.content.decode()
        response_dict = json.loads(response_json)

        self._assert_in("jms", response_dict)
        self._assert_in("storage", response_dict)
        self._assert_value(response_dict["jms"], "RUNNING")
        self._assert_value(response_dict["storage"], "RUNNING")

    @staticmethod
    def _http_post(route: str, data: dict) -> Response:
        body = json.dumps(data)
        return requests.post(route, body)

    @staticmethod
    def _http_get(route: str) -> Response:
        return requests.get(route)

    @staticmethod
    def _assert_value(a, b):
        if a != b:
            raise Exception("AssertError - Invalid value. actual:{} expected:{}".format(a, b))

    @staticmethod
    def _assert_in(a, b):
        if a not in b:
            raise Exception("AssertError - Value not in value:{} array:{}".format(a, b))


def start_test(config_path="../config.json") -> None:
    config = utils.load_config(config_path)

    be_tests = BackendTestCases()
    be_tests.test(config)

    print("SUCCESS ALL")


if __name__ == "__main__":
    if len(sys.argv) == 2:
        config_path = sys.argv[1]
        start_test(config_path)
    else:
        start_test()
