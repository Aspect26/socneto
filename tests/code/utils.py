import json

def resolve_credentials(cred_path):
    try:
        with open(cred_path,"r")as f:
            cred = json.load(f)
            print("credentials loaded successfully")
            return cred
    except:
        print("could not load credentials")
        return {}

def load_config(config_path = '../config.json'):
    try:
        with open(config_path,"r")as f:
            config = json.load(f)
            print("config loaded successfully")
            return config
    except:
        print("could not load config")
        return {}