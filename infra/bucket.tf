resource "aws_s3_bucket" "bucket_fiap_fase2" {
  bucket = var.bucket_name
TESTE_TERRAFORM_ERRADO
  lifecycle {
    prevent_destroy = true # Como o próprio tfstate está no bucket, previnimos que ele seja destruído acidentalmente
  }

  tags = {
    Name              = "Bucket ${var.project_name}"
    Environment       = var.environment
    ProjectIdentifier = var.project_identifier
  }
}
