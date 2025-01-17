#!/bin/bash

# Update the package lists
sudo apt-get update

# Install Python and pip
sudo apt-get install -y python3

sudo apt-get install -y python3-pip

sudo apt-get upgrade

sudo apt-get install firefox

# Install the necessary Python libraries
pip3 install RPi.GPIO requests

pip3 install requests

# Run the sensor script in the background
python3 sensor_script.py &
firefox --kiosk https://phygital.programmersinparis.net
