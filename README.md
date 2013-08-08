# AWS DEV Server Manager

Simple utility for starting and stopping AWS servers 

## GUI

Shows a list of AWS servers, their running status and a button to Start or Stop them
![GUI screenshot](https://f.cloud.github.com/assets/227505/930861/64125ca2-0019-11e3-95c6-b337d1a95f98.PNG)

## CLI

Enables you to start / stop AWS servers from the command line.

```
aws-dev-server-manager-cli --action=stop --instances="i-272586d,i-234586d,i-643586d,i-432586d"
```

Useful to run as a Scheduled build on your Build Server to shut down AWS dev instances at the end of the work day.

## Configuration

### AWS credentials/region

Add your `AWSAccessKey`, `AWSSecretKey` and `AWSRegion` to the `App.config`, see 
[Configuring Credentials for Your Application](http://docs.aws.amazon.com/AWSSdkDocsNET/latest/DeveloperGuide/net-dg-config-creds.html) for details.

* If you happen to run this utility from an Amazon EC2 instance started with an 
[IAM Role for Amazon EC2](http://docs.aws.amazon.com/AWSEC2/latest/UserGuide/iam-roles-for-amazon-ec2.html), 
you can also omit these explicit credentials (just set `value` to an empty string), which would be automatically picked up from that IAM role then.

### EC2 instance pool selection

By default, the utility makes all EBS backed EC2 instances in the configured account available for starting/stopping.

You can also restrict the available instances via [Filters](http://docs.aws.amazon.com/AWSEC2/latest/UserGuide/Using_Filtering.html#Resource_Filters), 
most commonly used by [Tagging Your Amazon EC2 Resources](http://docs.aws.amazon.com/AWSEC2/latest/UserGuide/Using_Tags.html) - see 
[Filtering the List of Resources by Tags](http://docs.aws.amazon.com/AWSEC2/latest/UserGuide/Using_Tags.html#filtering-the-list-of-resources-by-tags-cli) 
for examples and [Using Filtering](http://docs.aws.amazon.com/AWSEC2/latest/UserGuide/Using_Filtering.html) for more details on the concept as such.

There are two available parameters in `App.config` for this purpose, which are both evaluated together at runtime and support the wildcard character `*`:

* `InstanceTags` - one or more semicolon separated key/value pairs of tags, e.g. `Name=WebServer-*` or  `Name=WebServer-*;Owner=TeamA`
* `InstanceFilters` - one or more semicolon separated key/value pairs of filters, e.g. `instance-type=m1.xlarge` or  `instance-type=m1.*;image-id=ami-1a2b3c4d`, see
section _Supported Filters_ within [DescribeInstances](http://docs.aws.amazon.com/AWSEC2/latest/APIReference/ApiReference-query-DescribeInstances.html) for details.
 * **Note**: This allows to lock down the list of instances to specific ones, e.g. `instance-id=i-1a2b3c4d;instance-id=i-5a6b7c8d`
 * **Note**: You can also just use `InstanceFilters` for tags as well by means of the `tag:` prefix, e.g. `instance-type=m1.xlarge;tag:Owner=TeamA`.

### IAM permissions

Here are a few example policies for the required [Identity and Access management (IAM)](http://aws.amazon.com/iam/) 
permissions to operate the EC2 instances - for more details on the more advanced policies you may want to read
[Resource-level Permissions for EC2 � Controlling Management Access on Specific Instances](http://blogs.aws.amazon.com/security/post/Tx29HCT3ABL7LP3/Resource-level-Permissions-for-EC2-Controlling-Management-Access-on-Specific-Ins).

#### All instances in all regions

This is the most simple IAM policy, which allows to operate all existing instances in all regions:
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": [
        "ec2:DescribeInstances",
        "ec2:DescribeRegions",
        "ec2:RebootInstances",
        "ec2:StartInstances",
        "ec2:StopInstances"
      ],
      "Resource": "*",
      "Effect": "Allow"
    }
  ]
}
```

#### Specific instances in their respective region

This explicitly names pre-setup instance(s) by means of their id, e.g:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": [
        "ec2:DescribeInstances",
        "ec2:DescribeRegions"
      ],
      "Resource": "*",
      "Effect": "Allow"
    },
    {
      "Action": [
        "ec2:RebootInstances",
        "ec2:StartInstances",
        "ec2:StopInstances"
      ],
      "Resource": [
        "arn:aws:ec2:us-east-1:1234567890:instance/i-96d811fe",
        "arn:aws:ec2:eu-west-1:1234567890:instance/i-12d81142"
      ],
      "Effect": "Allow"
    }
  ]
}
```

* The `Resource` ARN has the following format (`<region>` and `<instance id>` can also be `*`, see alternative below):    
```
arn:aws:ec2:<region>:<account id>:instance/<instance id>
```

#### All instances matching a tag in one region

Alternatively, if you want a more dynamic solution based on tags, you could apply something like this:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": [
        "ec2:DescribeInstances",
        "ec2:DescribeRegions"
      ],
      "Resource": "*",
      "Effect": "Allow"
    },
    {
      "Action": [
        "ec2:RebootInstances",
        "ec2:StartInstances",
        "ec2:StopInstances"
      ],
      "Condition": {
        "StringEquals": {
          "ec2:ResourceTag/pool":"developers"
        }
      },
      "Resource": [
        "arn:aws:ec2:*:1234567890:instance/*"
      ],
      "Effect": "Allow"
    }
  ]
}
```

* The `Resource` ARN has the following format (`<region>` and `<instance id>` can also be `*`, see alternative below):    
```
arn:aws:ec2:<region>:<account id>:instance/<instance id>
```

Since the permission excludes `ec2:CreateTags`, nobody could adjust the instance list manually, 
yet every instance added/removed to/from this tag based pool `developers` would be reflected automatically.
