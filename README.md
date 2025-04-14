# Lambda CI/CD with GitHub Actions

This repository demonstrates how to deploy an AWS Lambda function using a CI/CD pipeline with GitHub Actions. The Lambda function prints a simple message and returns it as a response.

## Prerequisites

Before you begin, ensure that you have the following tools installed on your local machine:

- **Python 3.9+**: [Download Python](https://www.python.org/downloads/)
- **pip**: Python package manager. Ensure it's installed by running `pip --version`.
- **AWS CLI**: Install the AWS Command Line Interface (CLI) to interact with AWS services. [AWS CLI Installation Guide](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html)
- **Git**: [Install Git](https://git-scm.com/downloads)
- **Visual Studio Code**: [VS Code](https://code.visualstudio.com/Download)
- **GitHub Account**: [Sign up for GitHub](https://github.com/join)
- **AWS Account with Programmatic Access**: [Create AWS Account](https://aws.amazon.com/)

## Setup Instructions

### Part 1: Set Up Lambda Function

1. **Create Lambda Folder Structure**:
    - Create a folder named `COUNTRYCAPITAL`.
    - Inside this folder, create a subfolder called `lambda`.

2. **Create Lambda Function**:
    - Inside the `lambda` folder, create a file named `lambda_function.py` with the following content:

    ```python
    import boto3

    def lambda_handler(event, context):
        message = "Hello from Lambda, how are you?"
        print(message)
        return {
            'statusCode': 200,
            'body': message
        }
    ```

3. **Create Requirements File**:
    - Inside the `lambda` folder, create a `requirements.txt` file with the following content:

    ```
    boto3
    ```

### Part 2: Prepare Deployment Package

1. Open your terminal in Visual Studio Code and run the following commands:

    ```bash
    cd lambda
    mkdir package
    pip install -r requirements.txt -t package/
    cp lambda_function.py package/
    cd package
    zip -r ../function.zip .
    cd ..
    ```

2. This creates a `function.zip` file that contains your Lambda function and dependencies, which can be deployed to AWS Lambda.

### Part 3: Create IAM Role for Lambda

1. Go to the **IAM Console** in the AWS Management Console.
2. Create a role with the **AWS Lambda** use case and attach the **AWSLambdaBasicExecutionRole** policy.
3. Name the role `lambda-basic-role` and create it.
4. Copy the **Role ARN** of this role for the next step.

### Part 4: Deploy Lambda Function Using AWS CLI

1. Run the following AWS CLI command to create the Lambda function, replacing `YOUR_ROLE_ARN_HERE` with the Role ARN you copied earlier:

    ```bash
    aws lambda create-function \
      --function-name HelloLambda \
      --runtime python3.9 \
      --role YOUR_ROLE_ARN_HERE \
      --handler lambda_function.lambda_handler \
      --zip-file fileb://function.zip
    ```

### Part 5: Create GitHub Repository

1. Initialize Git in your project folder:

    ```bash
    git init
    git add .
    git commit -m "Initial Lambda commit"
    ```

2. Create a new repository on GitHub.
3. Add the remote repository and push your code:

    ```bash
    git remote add origin https://github.com/shreeshtareddy/lambda-ci-cd.git
    git branch -M main
    git push -u origin main
    ```

### Part 6: Set Up GitHub Actions for CI/CD

1. In the root of your project, create the following directory structure: `.github/workflows/`.
2. Inside the `.github/workflows/` folder, create a file named `lambda-deploy.yml` with the following content:

    ```yaml
    name: Deploy to Lambda

    on:
      push:
        branches: [ main ]

    jobs:
      deploy:
        runs-on: ubuntu-latest

        steps:
        - name: Checkout Code
          uses: actions/checkout@v3

        - name: Set up Python
          uses: actions/setup-python@v4
          with:
            python-version: '3.9'

        - name: Install Dependencies and Zip
          run: |
            pip install -r lambda/requirements.txt -t lambda/package
            cp lambda/lambda_function.py lambda/package/
            cd lambda/package
            zip -r ../function.zip .
            cd ../..

        - name: Deploy Lambda
          uses: appleboy/lambda-action@master
          with:
            aws_access_key_id: ${{ secrets.AWS_ACCESS_KEY_ID }}
            aws_secret_access_key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
            region: us-east-1
            function_name: HelloLambda
            zip_file: lambda/function.zip
    ```

### Part 7: Add GitHub Secrets

1. Go to **Settings > Secrets and variables > Actions** in your GitHub repository.
2. Add the following secrets:
    - **`AWS_ACCESS_KEY_ID`**: Your AWS Access Key ID.
    - **`AWS_SECRET_ACCESS_KEY`**: Your AWS Secret Access Key.

### Part 8: Commit and Push Changes to GitHub

After setting up the GitHub Actions workflow, commit and push the changes:

```bash
git add .github/
git commit -m "Add GitHub Actions workflow for Lambda deployment"
git push




Part 9: Verify Deployment
Go to the Actions tab in your GitHub repository.

You will see the workflow running. Once the workflow completes, go to the AWS Lambda Console.

Select the HelloLambda function.

Click Test to create a test event and trigger the function.

You should see the following response:

json
Copy
Edit
{
  "statusCode": 200,
  "body": "Hello from Lambda, how are you?"
}

 Expose Lambda via API Gateway
✅ Steps to Create the API
Go to API Gateway Console

Create a new REST API

Create a resource /hello

Add a GET method

Integration type: Lambda Function

Enable Lambda Proxy Integration

Enter your Lambda name: HelloLambda

Deploy the API

Stage name: Demoapi

🌍 Public API Endpoint
Your Lambda is now available here:

➡️ https://l0bspy8ru7.execute-api.us-east-1.amazonaws.com/Demoapi/hello

💬 Response:
json
Copy
Edit
{
  "statusCode": 200,
  "body": "Hello from Lambda, how are you?"
}
🧪 Test it with curl
bash
Copy
Edit
curl https://l0bspy8ru7.execute-api.us-east-1.amazonaws.com/Demoapi/hello
