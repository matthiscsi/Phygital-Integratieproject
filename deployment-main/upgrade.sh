
#!/bin/bash

# Source the deployment library and configuration
source lib/deploy_lib.sh
source lib/default_config.sh
check_config lib/config.sh

# List the instances in the instance group and extract the instance IDs
INSTANCE_IDS=$(gcloud compute instance-groups managed list-instances $INSTANCE_GROUP_NAME --zone $ZONE --format="value(instance.basename())")

# Restart the instances one by one
for INSTANCE_ID in $INSTANCE_IDS; do
  echo "Starting upgrade for instance $INSTANCE_ID..."
  gcloud compute instances stop $INSTANCE_ID --zone $ZONE
  echo "Instance $INSTANCE_ID successfully restarted."
  echo "Waiting ~5 minutes to let the instance's startup script propagate."
  sleep 330
done