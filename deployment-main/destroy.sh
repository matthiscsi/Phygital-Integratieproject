#!/bin/bash

# Source the deployment library and configuration
source lib/deploy_lib.sh
source lib/default_config.sh
check_config lib/config.sh


# Set variables
if [ -z "$1" ]; then
  echo "No Google Cloud Project ID was passed to the script. Setting default Project ID..."
  PROJECT_ID="phygitalapp-417714"
else
  PROJECT_ID="$1"
fi

# Authenticate with the service account key
echo "$SERVICE_ACCOUNT_KEY" | gcloud auth activate-service-account --key-file="/tmp/service-account-key.json" --quiet

# Set the Google Cloud project
gcloud config set project $PROJECT_ID --quiet

# Ask the user whether to delete the SQL instance
read -p "Do you want to delete the SQL instance? (yes/y | no/n/enter): " DELETE_SQL

# Delete the PostgreSQL Cloud SQL instance if the user input is "yes", "y", or "true"
if [[ "$DELETE_SQL" == "yes" || "$DELETE_SQL" == "y" || "$DELETE_SQL" == "true" ]]; then
# Disable deletion protection
  gcloud sql instances patch $SQL_INSTANCE_NAME --quiet --no-deletion-protection

  gcloud sql instances delete $SQL_INSTANCE_NAME --quiet
fi

# Ask the user whether to delete the SQL instance
read -p "Do you want to delete storage bucket? (yes/y | no/n/enter): " DELETE_BUCKET

# Delete the PostgreSQL Cloud SQL instance if the user input is "yes", "y", or "true"
if [[ "$DELETE_BUCKET" == "yes" || "$DELETE_BUCKET" == "y" || "$DELETE_BUCKET" == "true" ]]; then

# Delete everything from the bucket
echo "Deleting everything from the bucket..."
gsutil rm -r gs://phygitalmediabucket/*

# Delete the bucket
  echo "Deleting the bucket..."
  gsutil rb gs://phygitalmediabucket
fi

# Ask the user whether to delete the Redis instance
read -p "Do you want to delete the Redis instance? (yes/y | no/n/enter): " DELETE_REDIS

# Delete the Redis instance if the user input is "yes", "y", or "true"
if [[ "$DELETE_REDIS" == "yes" || "$DELETE_REDIS" == "y" || "$DELETE_REDIS" == "true" ]]; then
  echo "Deleting the Redis instance..."
  gcloud redis instances delete phygitalredisinstance --region=$REGION
fi

# Delete the forwarding rule
gcloud compute forwarding-rules delete $FORWARDING_RULE_NAME --global -q

# Delete the target HTTP proxy
gcloud compute target-https-proxies delete $HTTPS_PROXY_NAME -q

# Delete the Loadbalancer URL map
gcloud compute url-maps delete $LOADBALANCER_NAME -q

# Delete the backend service
gcloud compute backend-services delete $BACKEND_SERVICE_NAME --global -q

# Delete the health check
gcloud compute health-checks delete $HEALTH_CHECK_NAME -q


# Delete the (managed) instance group
gcloud compute instance-groups managed delete $INSTANCE_GROUP_NAME --zone $ZONE -q

# Delete the instance template
gcloud compute instance-templates delete $TEMPLATE_NAME -q

# Delete the firewall rule
gcloud compute firewall-rules delete allow-lb-health-checks -q

# Delete the reserved IP address
gcloud compute addresses delete $IP_ADDRESS_NAME --global -q

# Delete the SSL-certificate
# WARNING: Certificates may take an hour or longer to work after creating a new one.
gcloud compute ssl-certificates delete $SSL_CERT_NAME

read -p "Do you want to delete the domain zone? This may have unwanted consequences (y/yes for yes / enter for no): " DELETE_DNS
# If the user input is "yes" or "y", configure DNS
if [[ "$DELETE_DNS" == "yes" || "$DELETE_DNS" == "y" ]]; then
  
  # Start a new transaction
  gcloud dns record-sets transaction start --zone=programmersinparis

  # Remove the existing A record 
  gcloud dns record-sets transaction remove --zone=$DNS_ZONE_NAME \
    --name="phygital.programmersinparis.net." \
    --type=A \
    --ttl=300

  # Execute the transaction
  gcloud dns record-sets transaction execute --zone=programmersinparis

else
  # Start a new transaction
  gcloud dns record-sets transaction start --zone=programmersinparis

  # Get the IP address of the existing A record
  IP_ADDRESS_TO_REMOVE=$(gcloud dns record-sets list --zone=programmersinparis --name="phygital.programmersinparis.net." --type=A --format="value(rrdatas[0])")

  # Remove the existing A record to prevent a new deploy being on the wrong IP address
  gcloud dns record-sets transaction remove --zone=programmersinparis \
    --name="phygital.programmersinparis.net." \
    --type=A \
    --ttl=300 \
    "$IP_ADDRESS_TO_REMOVE"

  # Execute the transaction
  gcloud dns record-sets transaction execute --zone=programmersinparis
fi


read -p "Do you want to delete the service account ? This may have unwanted consequences (y/yes for yes / enter for no): " DELETE_SERVICEACC
# If the user input is "yes" or "y", configure DNS
if [[ "$DELETE_SERVICEACC" == "yes" || "$DELETE_SERVICEACC" == "y" ]]; then
  # Delete the service account
  gcloud iam service-accounts delete ${SERVICE_ACCOUNT_NAME}@${PROJECT_ID}.iam.gserviceaccount.com --quiet
fi