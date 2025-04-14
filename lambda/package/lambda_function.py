import boto3

def lambda_handler(event, context):
    message = "Hello from Lambda, how are you?"
    print(message)
    return {
        'statusCode': 200,
        'body': message
    }
