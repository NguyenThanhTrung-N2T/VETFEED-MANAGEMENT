variable "aws_region" {
  type        = string
  description = "AWS deployment region"
  default     = "ap-southeast-1" # Singapore
}

variable "environment" {
  type        = string
  description = "Application deployment environment"
  default     = "production"
}

variable "instance_type" {
  type        = string
  description = "EC2 Instance Size"
  default     = "t3.micro"
}

variable "ssh_key_name" {
  type        = string
  description = "SSH key pair name"
  default     = "vetfeed-production-key"
}

variable "db_username" {
  type        = string
  description = "Database administrator username"
  default     = "sa"
}

variable "db_password" {
  type        = string
  description = "Database administrator password"
  sensitive   = true
  default     = "ThanhTrung@1912935"
}
