#!/usr/bin/env python3

import Jetson.GPIO as GPIO
import time
import requests

# Use the board pin numbering
GPIO.setmode(GPIO.BOARD)

# Use the board pin numbering
GPIO.setmode(GPIO.BOARD)

# Set the sensor pin as input
sensor_pin = 7
GPIO.setup(sensor_pin, GPIO.IN)

# Server URL
server_url = "https://phygital.programmersinparis.net/Sensor/movement-detected"

while True:
    # Read sensor data
    if GPIO.input(sensor_pin):
        print("Movement detected!")

        # Send a request to the server
        try:
            response = requests.post(server_url)
            response.raise_for_status()
        except requests.exceptions.HTTPError as errh:
            print ("Http Error:",errh)
        except requests.exceptions.ConnectionError as errc:
            print ("Error Connecting:",errc)
        except requests.exceptions.Timeout as errt:
            print ("Timeout Error:",errt)
        except requests.exceptions.RequestException as err:
            print ("Something went wrong",err)

    # Wait for a while before reading the data again
    time.sleep(1)