

# Capital Finder App – Deploying a Dockerized .NET App to AWS EKS

This guide walks you through deploying your Dockerized `.NET` application to Amazon EKS using **ECR**, **kubectl**, and **eksctl**.

---

## ✅ Prerequisites

Before you begin, ensure the following are installed and configured:

- **AWS CLI** (`aws configure`)
- **Docker**
- **kubectl**
- **eksctl**
- An AWS IAM user with permissions for **ECR**, **EKS**, and **EC2**

---

## ✅ Step 1: Create an ECR Repository

```bash
aws ecr create-repository --repository-name capital-finder-app



Copy the repositoryUri from the output, e.g.:


123456789012.dkr.ecr.us-east-1.amazonaws.com/capital-finder-app




✅ Step 2: Build and Push Docker Image to ECR
2.1 Login to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789012.dkr.ecr.us-east-1.amazonaws.com




Replace 123456789012 with your AWS account ID.




2.2 Build the Docker Image
docker build -t capital-finder-app .




2.3 Tag the Image for ECR
docker tag capital-finder-app:latest 123456789012.dkr.ecr.us-east-1.amazonaws.com/capital-finder-app:latest




2.4 Push the Image to ECR
docker push 123456789012.dkr.ecr.us-east-1.amazonaws.com/capital-finder-app:latest




✅ Step 3: Create an EKS Cluster
eksctl create cluster --name capital-cluster --region us-east-1 --nodes 2




This will take around 15 minutes. Once complete, kubectl will be configured for your new cluster.




✅ Step 4: Create Kubernetes Deployment YAML
Create a file named deployment.yaml and paste the following:


apiVersion: apps/v1
kind: Deployment
metadata:
  name: capital-finder-deployment
spec:
  replicas: 1
  selector:
    matchLabels:
      app: capital-finder
  template:
    metadata:
      labels:
        app: capital-finder
    spec:
      containers:
        - name: capital-finder
          image: 123456789012.dkr.ecr.us-east-1.amazonaws.com/capital-finder-app:latest
          ports:
            - containerPort: 80
---
apiVersion: v1
kind: Service
metadata:
  name: capital-finder-service
spec:
  type: LoadBalancer
  selector:
    app: capital-finder
  ports:
    - protocol: TCP
      port: 80
      targetPort: 80




Be sure to replace the image URI with your actual ECR image URL.




✅ Step 5: Deploy to EKS
kubectl apply -f deployment.yaml




✅ Step 6: Access Your App
kubectl get svc



Look for a line like:


capital-finder-service   LoadBalancer   <EXTERNAL-IP>   80:...   ...



Once <EXTERNAL-IP> is available, open it in your browser to access your app.


