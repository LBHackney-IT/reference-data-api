resource "aws_dynamodb_table" "referencedataapi_dynamodb_table" {
    name                  = "ReferenceData"
    billing_mode          = "PROVISIONED"
    read_capacity         = 10
    write_capacity        = 10
    hash_key              = "category"
    range_key             = "id"


    attribute {
        name              = "id"
        type              = "S"
    }
    
    attribute {
        name              = "category"
        type              = "S"
    }

    tags = {
        Name              = "reference-data-api-${var.environment_name}"
        Environment       = var.environment_name
        terraform-managed = true
        project_name      = var.project_name
    }

    point_in_time_recovery {
        enabled           = true
    }
}
