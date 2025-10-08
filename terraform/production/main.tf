# INSTRUCTIONS:
# 1) ENSURE YOU POPULATE THE LOCALS
# 2) ENSURE YOU REPLACE ALL INPUT PARAMETERS, THAT CURRENTLY STATE 'ENTER VALUE', WITH VALID VALUES
# 3) YOUR CODE WOULD NOT COMPILE IF STEP NUMBER 2 IS NOT PERFORMED!
# 4) ENSURE YOU CREATE A BUCKET FOR YOUR STATE FILE AND YOU ADD THE NAME BELOW - MAINTAINING THE STATE OF THE INFRASTRUCTURE YOU CREATE IS ESSENTIAL - FOR APIS, THE BUCKETS ALREADY EXIST
# 5) THE VALUES OF THE COMMON COMPONENTS THAT YOU WILL NEED ARE PROVIDED IN THE COMMENTS
# 6) IF ADDITIONAL RESOURCES ARE REQUIRED BY YOUR API, ADD THEM TO THIS FILE
# 7) ENSURE THIS FILE IS PLACED WITHIN A 'terraform' FOLDER LOCATED AT THE ROOT PROJECT DIRECTORY

terraform {
  required_providers {
    aws = {
      source = "hashicorp/aws"
    }
  }
}

provider "aws" {
  region = "eu-west-2"
}

data "aws_caller_identity" "current" {}

data "aws_region" "current" {}

locals {
  parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
  esDomain        = "https://${module.elasticsearch_db_production.es_endpoint_url}"
}

terraform {
  backend "s3" {
    bucket  = "terraform-state-disaster-recovery"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/reference-data-api/state"
  }
}


/*    ELASTICSEARCH SETUP    */

data "aws_vpc" "production_vpc" {
  tags = {
    Name = "disaster-recovery-prod"
  }
}

data "aws_subnets" "production" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.production_vpc.id]
  }
  filter {
    name   = "tag:Type"
    values = ["private"]
  }
}

module "elasticsearch_db_production" {
  source                 = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/elasticsearch"
  vpc_id                 = data.aws_vpc.production_vpc.id
  environment_name       = "production"
  port                   = 443
  domain_name            = "reference-data-api-es"
  subnet_ids             = data.aws_subnets.production.ids
  project_name           = "reference-data-api"
  es_version             = "7.8"
  encrypt_at_rest        = "true"
  instance_type          = "t3.medium.elasticsearch"
  instance_count         = "2"
  ebs_enabled            = "true"
  ebs_volume_size        = "60"
  region                 = data.aws_region.current.name
  account_id             = data.aws_caller_identity.current.account_id
  create_service_role    = false
  zone_awareness_enabled = true
}

resource "aws_ssm_parameter" "reference_data_elasticsearch_domain" {
  name  = "/reference-data-api/production/elasticsearch-domain"
  type  = "String"
  value = local.esDomain
}

module "reference_data_api_cloudwatch_dashboard" {
  source                  = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/cloudwatch/dashboards/api-dashboard"
  environment_name        = var.environment_name
  api_name                = "reference-data-api"
  include_sns_widget      = false
  include_dynamodb_widget = false
  no_sns_widget_dashboard = false
}

# data "aws_ssm_parameter" "cloudwatch_topic_arn" {
#   name = "/housing-tl/${var.environment_name}/cloudwatch-alarms-topic-arn"
# }

# module "api-alarm" {
#   source           = "sgithub.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/cloudwatch/api-alarm"
#   environment_name = var.environment_name
#   api_name         = "reference-data-api"
#   alarm_period     = "300"
#   error_threshold  = "1"
#   sns_topic_arn    = data.aws_ssm_parameter.cloudwatch_topic_arn.value
# }

/*EC2 bastion instance for ElasticSearch access via Session Manager*/
module "ec2s" {
  source = "github.com/LBHackney-IT/ce-aws-ec2-lbh"
  tags = {
    AutomationBuildUrl = "https://circleci.com/gh/LBHackney/reference-data-api"
    Environment        = "prod"
    TeamEmail          = "developmentteam@hackney.gov.uk"
    Department         = "Housing"
    Application        = "reference-data-api"
    Phase              = "production"
    Stack              = "application"
    Project            = "reference-data-api"
    Confidentiality    = "Internal"
  }
  prevent_termination = false
  vpc_id              = data.aws_vpc.production_vpc.id
  subnet_ids          = data.aws_subnets.production.ids
  ec2_instances = {
    "bastion" = {
      "ami"               = "ami-0d29e1f6d5d739940"
      "ebs_block_devices" = {}

      # Allow all outbound traffic (needed for ElasticSearch HTTPS)
      "egress_rules" = ["all-all"]
      "allow_egress" = true

      "instance_type" = "t3.micro"

      "root_block_device_volume_size" = 20
    }
  }
}