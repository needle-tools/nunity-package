# Development setup


## Cloud Storage access:
1) Generate a access key here: https://console.cloud.google.com/iam-admin/serviceaccounts/details/100019773737101810166;edit=true/keys?folder=&organizationId=&project=needle-nuget (IAM & Admin / AppEngine default service account / Keys)
2) Place the key file as ``development-gcloud-credentials.json`` at the root of this project (next to .sln file)
3) Done, you should now be able to access cloud storage objects locally