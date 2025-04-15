

# 🌍 Capital Finder App – Deploying a Dockerized .NET App to AWS EKS

Welcome to the **Capital Finder App** deployment guide! This document will help you containerize your `.NET` application and deploy it to **Amazon Elastic Kubernetes Service (EKS)** using **Amazon Elastic Container Registry (ECR)**.

> 🛠️ By the end of this guide, your app will be deployed to a production-grade Kubernetes cluster on AWS and accessible via a public Load Balancer.

---

## 📚 Table of Contents

1. [🧰 Prerequisites](#-prerequisites)  
2. [🐳 Step 1: Dockerize the .NET App](#-step-1-dockerize-the-net-app)  
3. [📦 Step 2: Push Image to AWS ECR](#-step-2-push-image-to-aws-ecr)  
4. [☁️ Step 3: Create an EKS Cluster](#️-step-3-create-an-eks-cluster)  
5. [🚀 Step 4: Deploy App to Kubernetes](#-step-4-deploy-app-to-kubernetes)  
6. [🌐 Step 5: Access Your Application](#-step-5-access-your-application)  
7. [🧹 Cleanup](#-cleanup)

---

## 🧰 Prerequisites

Before you start, install and configure the following tools:

| Tool        | Description                          | Install Link |
|-------------|--------------------------------------|--------------|
| [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html) | AWS Command Line Interface | ✅ |
| [Docker](https://docs.docker.com/get-docker/)      | Build & run containers      | ✅ |
| [kubectl](https://kubernetes.io/docs/tasks/tools/) | Manage Kubernetes clusters  | ✅ |
| [eksctl](https://eksctl.io/introduction/installation/) | Create and manage EKS clusters | ✅ |
| [.NET SDK](https://dotnet.microsoft.com/en-us/download) | Build the .NET app          | ✅ |

Also:

```bash
aws configure
```

Set up your AWS credentials (Access Key, Secret, and default region).

---

## 🐳 Step 1: Dockerize the .NET App

### 📄 1.1 Create a `Dockerfile` in your project root

```Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CapitalFinderApp.dll"]
```

### 🔨 1.2 Build the Docker Image

```bash
docker build -t capital-finder-app .
```

---

## 📦 Step 2: Push Image to AWS ECR

### 🧱 2.1 Create an ECR Repository

```bash
aws ecr create-repository --repository-name capital-finder-app
```

Copy the `repositoryUri` from the output:
```
123456789012.dkr.ecr.us-east-1.amazonaws.com/capital-finder-app
```

### 🔐 2.2 Authenticate Docker to ECR

```bash
aws ecr get-login-password --region us-east-1 | \
docker login --username AWS --password-stdin 123456789012.dkr.ecr.us-east-1.amazonaws.com
```

### 🏷️ 2.3 Tag & Push Your Image

```bash
docker tag capital-finder-app:latest 123456789012.dkr.ecr.us-east-1.amazonaws.com/capital-finder-app:latest

docker push 123456789012.dkr.ecr.us-east-1.amazonaws.com/capital-finder-app:latest
```




![Screenshot 2025-04-15 112332](https://github.com/user-attachments/assets/815c33b6-1837-4593-b730-d1a516f09f33)


---

## ☁️ Step 3: Create an EKS Cluster

### ⏳ 3.1 Provision the Cluster (takes ~15 mins)

```bash
eksctl create cluster \
--name capital-cluster \
--region us-east-1 \
--nodes 2 \
--node-type t3.medium \
--managed
```

Verify with:

```bash
kubectl get nodes
```




![Screenshot 2025-04-15 112302](https://github.com/user-attachments/assets/0e4b6b1c-a7a0-491e-b495-1551b3fce1cf)



![Screenshot 2025-04-15 112309](https://github.com/user-attachments/assets/7e8dfc6e-8861-498d-81e0-d781f282340b)

---

## 🚀 Step 4: Deploy App to Kubernetes

### 📝 4.1 Create a `deployment.yaml` file

```yaml
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
```

> ✅ Replace `image:` URI with your actual ECR image URI.

### 🚀 4.2 Apply the Configuration

```bash
kubectl apply -f deployment.yaml
```

Check your resources:

```bash
kubectl get pods
kubectl get svc
```

---

## 🌐 Step 5: Access Your Application

Run:

```bash
kubectl get svc capital-finder-service
```

Look for the `EXTERNAL-IP` column:

```
NAME                     TYPE           CLUSTER-IP       EXTERNAL-IP       PORT(S)        AGE
capital-finder-service   LoadBalancer   10.100.123.45    54.210.89.123     80:32444/TCP   2m
```

Open your browser:

```
http://<EXTERNAL-IP>
```

🎉 **Your .NET app is now live on AWS EKS!**

---

## 🧹 Cleanup

To avoid unnecessary AWS charges, clean up your resources:

```bash
eksctl delete cluster --name capital-cluster --region us-east-1
aws ecr delete-repository --repository-name capital-finder-app --force
```

---
