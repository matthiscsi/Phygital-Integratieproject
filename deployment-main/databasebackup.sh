#!/bin/bash

# Source the deployment library and configuration
source lib/deploy_lib.sh
source lib/default_config.sh
check_config lib/config.sh

# Set the Google Cloud project
gcloud config set project $PROJECT_ID

# Create a backup of the SQL instance
gcloud sql backups create --instance=$SQL_INSTANCE_NAME

echo "Backup of the SQL instance $SQL_INSTANCE_NAME created."