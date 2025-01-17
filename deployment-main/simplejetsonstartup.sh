#!/bin/bash

# Run the sensor script in the background
python3 sensor_script.py &
firefox --kiosk https://phygital.programmersinparis.net
