resource "aws_ssm_parameter" "reference_data_token" {
  name  = "/housing-tl/pre-production/reference-data-token"
  type  = "String"
  value = "to_be_set_manually"

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}
