# Getting started

There are two ways you can currently use our project, the first and preffered way is by
installing and using Mosquitto. 

The other way is by making use of our MQTTMockupClient class. This class supports the basic 
functions of our MQTT server. It also publishes messages to a few hardcoded topics every x seconds.
Only use this if you can't setup Mosquitto or if it is to much effort for grading our project. 

## Download and install
Download Mosquitto https://mosquitto.org/download/
Install Mosquitto

## Running and configuring Mosquitto
For Windows users open CMD and navigate to the Mosquitto install folder.
To start Mosquitto for the first time use the following command: 
* mosquitto -v -c mosquitto.conf     

After doing this your Mosquitto server/broker should be running on your IP4 addres.
To get this addres open another CMD and type in ipconfig. 
Copy or remember your IP4 addres for later use.

## Subscribing and publishing
To test if your Mosquitto server is propperly running, or if you want to send some custom messages
to the Mosquitto server run the following commands:
* mosquitto_sub -t YOURTOPIC
* mosquitto_pub -r -t YOURTOPIC -m "YOURMESSAGE"   


## Using the MQTTMockupClient
To use the mock client all you need to do is navigate to the folder where you are running our project from,
go to the App.cs and in the constructor and replace:
* App.Client = new MQTTClient();
With:
* App.Client = new MQTTMockClient();

# Running the project

Since none of us are using an Iphone we decided to develop for Android only.
Make sure to have an Android emulator installed. 

## Conncecting
When the app launches you are prompted to fill in an IP addres and a port.
The IP addres is the IP4 addres on which your Mosquitto server is running. 
The port is 1883 by default. 

If you are using the MQTTMockupClient it doesn't matter which IP and Port you use.

## Failed to connect?
If you are sure Mosquitto is propperly setup but you can't reach the server with the app
try and enable TCP on port 1883(If you are using another port make the rule for that specific port) 
in your firewall. If this still doesn't work make the rule for TCP && UDP, if its still broken
make use of the MQTTMockupClient. 
