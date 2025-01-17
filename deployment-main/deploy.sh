#!/bin/bash

# ASCII Intro, welcome to our deployment script!
echo "  _____  _                 _ _        _ "
echo " |  __ \| |               (_) |      | |"
echo " | |__) | |__  _   _  __ _ _| |_ __ _| |"
echo " |  ___/| '_ \| | | |/ _  | | __/ _  | |"
echo " | |    | | | | |_| | (_| | | || (_| | |"
echo " |_|    |_| |_|\__, |\__, |_|\__\__,_|_|"
echo "               __/ | __/ |             "
echo "              |___/ |___/              "

echo "  __  ______     __  _____ _____ _____  __  "
echo " / / |  _ \ \   / / |  __ \_   _|  __ \ \ \ "
echo "| |  | |_) \ \_/ /  | |__) || | | |__) | | |"
echo "| |  |  _ < \   /   |  ___/ | | |  ___/  | |"
echo "| |  | |_) | | |    | |    _| |_| |      | |"
echo "| |  |____/  |_|    |_|   |_____|_|      | |"
echo " \_\                                    /_/ "
                                              

# Source the deployment library, configuration and parameters
source lib/deploy_lib.sh

PROJECT_ID=$1
REGION=$2
ZONE="$2-b"
MIN_REPLICAS=$3
MAX_REPLICAS=$4

# Get the project name from the project ID
PROJECT_NAME=$(gcloud projects describe $PROJECT_ID --format="value(name)")

export SERVICE_ACCOUNT_EMAIL="deploy@$PROJECT_NAME.iam.gserviceaccount.com"

# If no command-line parameters are given, source the variables from default_config.sh
if [[ -z "$PROJECT_ID" && -z "$REGION" && -z "$MIN_REPLICAS" && -z "$MAX_REPLICAS" ]]; then
  source lib/default_config.sh
fi
echo "BEFORE DEPLOYING, GOOGLE CLOUD WILL UPDATE. PLEASE SAY YES TO ANY PROMPTS"
sleep 5  
# Update gcloud if possible
gcloud components update

# Set the Google Cloud project
gcloud config set project $PROJECT_ID


# This function will create a service account and create a service key. 
# This is needed for authentication of various Google Cloud services, aswell as this script.
function configure_service_key(){

  # Print a message to the terminal
  echo "You need to create a service account called 'deploy' inside your project if not already done, then create a key file for that service account. Rename that file to 'key.json' and put it inside this folder."
  echo "Press any key to continue..."
  read -n 1 -s

  # Enable the Secret Manager API
  gcloud services enable secretmanager.googleapis.com

  # Enable the Cloud Resource Manager API
  gcloud services enable cloudresourcemanager.googleapis.com


  # Authenticate with the service account key
  gcloud auth activate-service-account --key-file=./key.json --quiet 

# Grant the necessary roles to the service account
gcloud projects add-iam-policy-binding $PROJECT_ID --member "serviceAccount:$SERVICE_ACCOUNT_EMAIL" --role "roles/compute.admin"
gcloud projects add-iam-policy-binding $PROJECT_ID --member "serviceAccount:$SERVICE_ACCOUNT_EMAIL" --role "roles/storage.admin"
gcloud projects add-iam-policy-binding $PROJECT_ID --member "serviceAccount:$SERVICE_ACCOUNT_EMAIL" --role "roles/owner"
gcloud projects add-iam-policy-binding $PROJECT_ID --member "serviceAccount:$SERVICE_ACCOUNT_EMAIL" --role "roles/secretmanager.admin"

# Check if the servicekey secret already exists
if ! gcloud secrets describe $SECRET_NAME_SERVICE_KEY > /dev/null 2>&1; then
    # Create the secret
    gcloud secrets create $SECRET_NAME_SERVICE_KEY --replication-policy="automatic"
    echo "Created secret $SECRET_NAME_SERVICE_KEY."
else
    echo "Secret $SECRET_NAME_SERVICE_KEY already exists. Adding a new version..."
fi

# Add a version of this service account to the secret manager for later use.
gcloud secrets versions add $SECRET_NAME_SERVICE_KEY --data-file=./key.json

# Remove the service account key after authentication & secret upload
rm ./key.json
}

# This function will configure the Google Cloud Storage.
# It uploads all files from the local media folder to the phygitalmediabucket bucket.
# The application will also pull from this bucket, so verify this bucket is created and the files are uploaded after deployment.
function configure_gcloud_storage(){

# Create a bucket named "phygitalmediabucket"
gsutil mb gs://phygitalmediabucket

# Upload all images from the /media folder to the bucket
for file in /media/*; do
  if [[ -f "$file" ]]; then
    gsutil cp "$file" gs://phygitalmediabucket
  fi
done

}

# This function will set up all needed secrets (credentials) in the Google Cloud Secret Manager module.
# Without these secrets configured and set in the cloud, the application will not be able to function correctly.
# It's recommended to keep the secret manager off-limits and not share it with anyone but the deployment team as it contains sensitive information.
function configure_secret_manager(){
  # Check if the admin secret already exists
  if ! gcloud secrets describe $SECRET_NAME_APP_USER_ADMIN > /dev/null 2>&1; then
    # Create the secret
    gcloud secrets create $SECRET_NAME_APP_USER_ADMIN --replication-policy="automatic"
    echo "Created secret $SECRET_NAME_APP_USER_ADMIN."
  else
    echo "Secret $SECRET_NAME_APP_USER_ADMIN already exists. Adding a new version..."
  fi

  # Add a version to the secret
  echo -n $SECRET_VALUE_APP_USER_ADMIN | gcloud secrets versions add $SECRET_NAME_APP_USER_ADMIN --data-file=-



  # Check if the companion secret already exists
  if ! gcloud secrets describe $SECRET_NAME_APP_USER_COMPANION > /dev/null 2>&1; then
    # Create the secret
    gcloud secrets create $SECRET_NAME_APP_USER_COMPANION --replication-policy="automatic"
    echo "Created secret $SECRET_NAME_APP_USER_COMPANION."
  else
    echo "Secret $SECRET_NAME_APP_USER_COMPANION already exists. Adding a new version..."
  fi

  # Add a version to the secret
  echo -n $SECRET_VALUE_APP_USER_COMPANION | gcloud secrets versions add $SECRET_NAME_APP_USER_COMPANION --data-file=-




  # Check if the subplatform admin secret already exists
  if ! gcloud secrets describe $SECRET_NAME_APP_USER_SUBPLATFORM_ADMIN > /dev/null 2>&1; then
    # Create the secret
    gcloud secrets create $SECRET_NAME_APP_USER_SUBPLATFORM_ADMIN --replication-policy="automatic"
    echo "Created secret $SECRET_NAME_APP_USER_SUBPLATFORM_ADMIN."
  else
    echo "Secret $SECRET_NAME_APP_USER_SUBPLATFORM_ADMIN already exists. Adding a new version..."
  fi

  # Add a version to the secret
  echo -n $SECRET_VALUE_APP_USER_SUBPLATFORM_ADMIN | gcloud secrets versions add $SECRET_NAME_APP_USER_SUBPLATFORM_ADMIN --data-file=-




  # Check if the gitlab secret already exists
  if ! gcloud secrets describe $SECRET_NAME_GITLAB_KEY > /dev/null 2>&1; then
    # Create the secret
    gcloud secrets create $SECRET_NAME_GITLAB_KEY --replication-policy="automatic"
    echo "Created secret $SECRET_NAME_GITLAB_KEY."
  else
    echo "Secret $SECRET_NAME_GITLAB_KEY already exists. Adding a new version..."
  fi

  # Add a version to the secret
  echo -n $SECRET_VALUE_GITLAB_KEY| gcloud secrets versions add $SECRET_NAME_GITLAB_KEY --data-file=-




  # Check if the connectionstring secret already exists
  if ! gcloud secrets describe $SECRET_NAME_CONNECTION_STRING > /dev/null 2>&1; then
    # Create the secret
    gcloud secrets create $SECRET_NAME_CONNECTION_STRING --replication-policy="automatic"
    echo "Created secret $SECRET_NAME_CONNECTION_STRING."
  else
    echo "Secret $SECRET_NAME_CONNECTION_STRING already exists. Adding a new version..."
  fi

  # Add a version to the secret
  echo -n $SECRET_VALUE_ | gcloud secrets versions add $SECRET_NAME_CONNECTION_STRING  --data-file=-

  # Another secret called Servicekey is already handled in the servicekey function.
  # All secrets are now created and set in the secret manager.
}

function configure_sql(){
# Check if the PostgreSQL Cloud SQL instance already exists
if ! gcloud sql instances describe $SQL_INSTANCE_NAME > /dev/null 2>&1; then
  # Create a PostgreSQL Cloud SQL instance
  gcloud sql instances create $SQL_INSTANCE_NAME  \
  --database-version=POSTGRES_13 \
  --tier=db-f1-micro \
  --region=$REGION

 
  DB_PASS=$(openssl rand -base64 18)
  echo "Warning! THIS IS THE ONLY TIME THE PASSWORD WILL EVER BE SHOWN DIRECTLY!"
  echo "- To view the password again later on, you will have to view the connectionstring secret in the Google Cloud Secret Manager."
  echo "Password for database: $DB_PASS"
  
  # Make the instance public
  gcloud sql instances patch $SQL_INSTANCE_NAME --authorized-networks=0.0.0.0/0

  # Set the password for the postgres user
  gcloud sql users set-password postgres --instance=$SQL_INSTANCE_NAME --password=$DB_PASS

  # Get the public IP address of the Cloud SQL instance
  SQL_IP_ADDRESS=$(gcloud sql instances describe phygitaldb --format="get(ipAddresses[].type,ipAddresses[].ipAddress)" | grep "PRIMARY" | awk '{print $2}' | cut -d ';' -f 1)
  
  # Create the connection string
  CONNECTION_STRING="Host=$SQL_IP_ADDRESS; Database=$DB_NAME; Username=$DB_USERNAME; Password=$DB_PASS;"

  # Check if the connectionstring already exists
  if gcloud secrets describe connectionstring > /dev/null 2>&1; then
    # Add a new version to the existing connectionstring
    echo -n "$CONNECTION_STRING" | gcloud secrets versions add connectionstring --data-file=-
  else
    # Create a new connectionstring
    echo -n "$CONNECTION_STRING" | gcloud secrets create connectionstring --data-file=- --replication-policy="automatic"
  fi

  # Enable deletion protection for the Cloud SQL instance
  gcloud sql instances patch $SQL_INSTANCE_NAME --deletion-protection
fi
}

function configure_redis(){
    # Enable the Redis API
    gcloud services enable redis.googleapis.com

    # Create a Redis instance
    gcloud redis instances create phygitalredisinstance --size=1 --region=$REGION --redis-version=redis_5_0

    # Get the IP address of the Redis instance
    IP_ADDRESS_REDIS=$(gcloud redis instances describe phygitalredisinstance --region=$REGION --format="value(host)")

     # Add a new version to the secret named "redisaddress" with the IP address of the Redis instance
    echo -n $IP_ADDRESS_REDIS | gcloud secrets versions add redisaddress --data-file=-
}

function configure_compute_engine(){
# Enable the Compute Engine API
gcloud services enable compute.googleapis.com

# Create an SSL-Certificate, which is needed for frontend https
# NOTE: VERIFYING THE SSL CERTIFICATE WILL TAKE SOME MINUTES AFTER DEPLOYMENT FINISHES
# IT IS RECOMMENDED TO NOT DELETE THE SSL CERTIFICATE AFTER FIRST CREATION
gcloud compute ssl-certificates create $SSL_CERT_NAME \
  --domains phygital.programmersinparis.net


export SERVICE_ACCOUNT_KEY=$(gcloud secrets versions access latest --secret=servicekey)
export GITLAB_ACCESS_TOKEN=$(gcloud secrets versions access latest --secret=gitlabaccesstoken)


# Create an instance template, which is a blueprint for the instances in the instance group
# It defines the machine type which affects performance, operating system, and other instance properties
gcloud compute instance-templates create $TEMPLATE_NAME \
  --image-family=ubuntu-2004-lts \
  --image-project=ubuntu-os-cloud \
  --machine-type=e2-medium \
  --scopes="userinfo-email,cloud-platform" \
  --metadata-from-file=startup-script=lib/startup.sh \
  --metadata gitlab-access-token=$GITLAB_ACCESS_TOKEN,gitlab-repo=$GITLAB_REPO,DOTNET-CLI-HOME=$DOTNET_CLI_HOME \
  --tags=http-server,https-server \
  --scopes=https://www.googleapis.com/auth/cloud-platform

# Create a managed instance group with 2 instances
gcloud compute instance-groups managed create $INSTANCE_GROUP_NAME \
  --base-instance-name $BASE_INSTANCE_NAME \
  --size 1 \
  --template $TEMPLATE_NAME \
  --zone $ZONE

# Create an autoscaler
gcloud compute instance-groups managed set-autoscaling $INSTANCE_GROUP_NAME \
  --max-num-replicas $MAX_REPLICAS \
  --min-num-replicas $MIN_REPLICAS \
  --target-cpu-utilization 0.6 \
  --cool-down-period 100 \
  --zone $ZONE

# Create a health check
gcloud compute health-checks create http $HEALTH_CHECK_NAME \
  --global \
  --port=8080 
  
# Wait for the health check to propagate
sleep 10

# Add a named port to the instance group
gcloud compute instance-groups managed set-named-ports $INSTANCE_GROUP_NAME \
  --named-ports http:8080 \
  --zone $ZONE  \
  || { echo "Failed to set named ports"; exit 1; }


# Create a backend service and attach the instance group
gcloud compute backend-services create $BACKEND_SERVICE_NAME \
  --protocol=HTTP \
  --health-checks=$HEALTH_CHECK_NAME \
  --session-affinity=GENERATED_COOKIE \
  --global \
  || { echo "Failed to create backend service"; exit 1; }

# Add the instance group to the backend service
gcloud compute backend-services add-backend $BACKEND_SERVICE_NAME \
  --instance-group=$INSTANCE_GROUP_NAME \
  --instance-group-zone=$ZONE \
  --global \
  || { echo "Failed to add backend to backend service"; exit 1; }

# Reserve a static IP address
gcloud compute addresses create $IP_ADDRESS_NAME --global \
|| { echo "Failed to reserve IP address"; exit 1; }

# Get the static IP address and set it to a variable
IP_ADDRESS=$(gcloud compute addresses describe $IP_ADDRESS_NAME --global --format="get(address)")

# Create a URL map that routes all traffic to the backend service
gcloud compute url-maps create $LOADBALANCER_NAME \
  --default-service $BACKEND_SERVICE_NAME \
  || { echo "Failed to create URL map"; exit 1; }

# Create a target HTTPS proxy to route requests to your URL map
gcloud compute target-https-proxies create $HTTPS_PROXY_NAME \
  --ssl-certificates=$SSL_CERT_NAME \
  --url-map $LOADBALANCER_NAME \
  --global \
  || { echo "Failed to create HTTPS proxy"; exit 1; }


# Create a forwarding rule to handle and route incoming requests
gcloud compute forwarding-rules create $FORWARDING_RULE_NAME \
  --address $IP_ADDRESS \
  --global \
  --target-https-proxy $HTTPS_PROXY_NAME \
  --ports 443 \
  || { echo "Failed to create forwarding rule"; exit 1; }

# Create a firewall rule that allows incoming traffic from the source ranges. 
gcloud compute firewall-rules create allow-lb-health-checks \
  --allow tcp:8080 \
  --source-ranges $SOURCE_RANGES \
  --target-tags http-server \
|| { echo "Failed to create firewall rule"; exit 1; }
}



# This function does all of the domain configuration. 
# This is totally optional, as you can just use the external IP of the loadbalancer to connect to the website. 
# This function will create a managed zone for the domain, create an A record for the domain, and update the
# A record if it already exists.
function configure_domain(){
# Ask the user whether to configure DNS
echo "ATTENTION: DNS WILL REQUIRE FURTHER MANUAL CONFIGURATION TO THE PHYGITAL DOMAIN"
read -p "Do you already have a domain zone? (Y / N): " CONFIGURE_DNS

# If the user input is "ni" or "n", configure DNS
if [[ "$CONFIGURE_DNS" == "no" || "$CONFIGURE_DNS" == "n" ]]; then

# Register the domain (this step needs to be done manually)
echo "If not already done, please register the domain 'programmersinparis.net' using the google cloud domains registrar before proceeding."
read -p "Press any key to continue... "
  # Create a managed zone for the domain
  gcloud dns managed-zones create programmersinparis \
    --description="" \
    --dns-name="programmersinparis.net" \
    --visibility="public" \
    --dnssec-state="off"

  # Start a new transaction
  gcloud dns record-sets transaction start --zone=programmersinparis

  # Create a DNS record 'phygital' for this zone
  gcloud dns record-sets create phygital.programmersinparis.net. \
    --zone="programmersinparis" \
    --type="A" \
    --ttl="300" \
    --rrdatas="$IP_ADDRESS"

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
  

    gcloud dns record-sets transaction start --zone=programmersinparis

    # Add a new A record
    gcloud dns record-sets transaction add --zone=programmersinparis \
    --name="phygital.programmersinparis.net." \
    --type=A \
    --ttl=300 \
    "$IP_ADDRESS"

     gcloud dns record-sets transaction execute --zone=programmersinparis
fi
}


# The main menu of the deployment script.
# All cli user interface options are listed here, as well as what functions they call.
while true; do
  echo -e "\nPlease select an option:"
  echo -e "1. Start up Phygital from scratch (automatically)"
  echo -e "2. Upgrade running application to work with new code version"
  echo -e "3. Delete all cloud resources"
  echo -e "4. Back up the database"
  echo -e "5. Developer tools"
  echo -e "6. Exit"
  read -p "Option: " option
  echo -e "\n"
  case $option in
    1)
      echo "Starting up Phygital..."
      # Call all functions to start up phygital from scratch
      configure_service_key
      configure_gcloud_storage
      configure_sql
      configure_secret_manager
      configure_redis
      configure_compute_engine
      configure_domain
      ;;
    2)
      echo "Running destroy script..."
          # Runs the destroy script from this directory, which will delete all cloud resources
          bash $(dirname "$0")/destroy.sh
      ;; 
    3)
      echo "Running destroy script..."
          # Runs the destroy script from this directory, which will delete all cloud resources
          bash $(dirname "$0")/destroy.sh
      ;;
    4)
      echo "Running database backup script..."
          # Runs the databasebackup script from this directory, which will back up the database
          bash $(dirname "$0")/databasebackup.sh
      ;;
    5)
    while true; do
      echo -e "\nDeveloper Tools:"
      # The developer tools are a handy list of extra options to manually configure some parts of the deployment if needed.
      echo -e "1. Configure Service Key"
      echo -e "2. Configure Cloud Storage"
      echo -e "3. Configure Cloud SQL"
      echo -e "4. Configure Secret Manager"
      echo -e "5. Configure Redis Server"
      echo -e "6. Configure Compute Engine"
      echo -e "7. Configure Domain"
      echo -e "8. Destroy script"
      echo -e "9. Upgrade script"
      echo -e "10. Jetson nano setup (ONLY for the jetson nano board inside of the physical installation!)"
      echo -e "11. Back up the Cloud SQL database"
      echo -e "12. Return to Main Menu"
      read -p "Option: " dev_option
      echo -e "\n"
      case $dev_option in
        1)
          echo "Configuring Service Key..."
          configure_service_key
          ;;
        2)
          echo "Configuring Cloud Storage..."
          configure_gcloud_storage
          ;;
        3)
          echo "Configuring Cloud SQL..."
          configure_sql
          ;;
        4)
          echo "Configuring Secret Manager..."
          configure_secret_manager
          ;;
	      5)
          echo "Configuring Redis.."
          configure_redis
          ;;
        6)
          echo "Configuring Compute Engine..."
          configure_compute_engine
          ;;
        7)
          echo "Configuring Domain..."
          configure_domain
          ;;
        8)
          echo "Running destroy script..."
          bash $(dirname "$0")/destroy.sh
          ;;
        9)
          echo "Running upgrade script..."
          bash $(dirname "$0")/upgrade.sh
          ;;
        10)
          echo "Setting up Jetson nano..."
          bash $(dirname "$0")/jetsonnanosetup.sh
          ;;
        11)
          echo "Backing up Cloud SQL database..."
          bash $(dirname "$0")/jetsonnanosetup.sh
          ;;
        12)
          echo "Returning to Main Menu..."
          break
          ;;
        *)
          echo "Invalid option. Please try again."
          ;;
      esac
    done
    ;;
    6)
      echo "Exiting..."
      exit 1
      ;;
  esac
done
