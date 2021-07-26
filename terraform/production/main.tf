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
            source  = "hashicorp/aws"
            version = "~> 3.0"
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
    esDomain = "https://${module.elasticsearch_db_production.es_endpoint_url}"
}

data "aws_iam_role" "ec2_container_service_role" {
  name = "ecsServiceRole"
}

data "aws_iam_role" "ecs_task_execution_role" {
  name = "ecsTaskExecutionRole"
}

terraform {
  backend "s3" {
    bucket  = "terraform-state-housing-production"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/reference-data-api/state"
  }
}


/*    ELASTICSEARCH SETUP    */

data "aws_vpc" "production_vpc" {
  tags = {
    Name = "vpc-housing-production"
  }
}

data "aws_subnet_ids" "production" {
  vpc_id = data.aws_vpc.production_vpc.id
  filter {
    name   = "tag:Type"
    values = ["private"]
  }
}

module "elasticsearch_db_production" {
  source                = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/elasticsearch"
  vpc_id                = data.aws_vpc.production_vpc.id
  environment_name      = "production"
  port                  = 443
  domain_name           = "reference-data-api-es"
  subnet_ids            = data.aws_subnet_ids.production.ids
  project_name          = "reference-data-api"
  es_version            = "7.8"
  encrypt_at_rest       = "false"
  instance_type         = "t3.small.elasticsearch"
  instance_count        = "2"
  ebs_enabled           = "true"
  ebs_volume_size       = "30"
  region                = data.aws_region.current.name
  account_id            = data.aws_caller_identity.current.account_id
  create_service_role   = false
  zone_awareness_enabled = true
}

resource "aws_ssm_parameter" "reference_data_elasticsearch_domain" {
  name = "/reference-data-api/production/elasticsearch-domain"
  type = "String"
  value = local.esDomain
}

