import requests
import json
import base64

endpoint = 'https://api.ximilar.com/tagging/generic/v2/tags/'
headers = {
    'Authorization': "Token 7b0d0dbe4f2840573e510c2527e0aea205a145fe",
    'Content-Type': 'application/json'
}

# First way to load base64
with open("processme.bmp", "rb") as image_file:
    encoded_string = base64.b64encode(image_file.read()).decode('utf-8')

data = {
    'records': [{"_base64": encoded_string}],
    'task_id': "5296671c-b463-4236-bdc1-e6c631131427",
    'version': "6",
}

response = requests.post(endpoint, headers=headers, data=json.dumps(data))
#print(response.text)
print(json.dumps(response.json()))
