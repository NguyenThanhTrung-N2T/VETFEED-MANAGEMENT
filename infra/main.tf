# Terraform configuration for VetFeed Backend AWS Infrastructure (Mock)

terraform {
  required_version = ">= 1.5.0"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  region = var.aws_region
}

# 1. Virtual Private Cloud (VPC)
resource "aws_vpc" "vetfeed_vpc" {
  cidr_block           = "10.0.0.0/16"
  enable_dns_hostnames = true
  enable_dns_support   = true

  tags = {
    Name        = "vetfeed-vpc"
    Environment = var.environment
  }
}

# 2. Security Group for Application VM (EC2)
resource "aws_security_group" "app_sg" {
  name        = "vetfeed-app-sg"
  description = "Allow inbound web traffic"
  vpc_id      = aws_vpc.vetfeed_vpc.id

  # HTTP
  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # HTTPS
  ingress {
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # VetFeed API port
  ingress {
    from_port   = 8080
    to_port     = 8080
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # SSH Access
  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # All outbound traffic
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name        = "vetfeed-app-sg"
    Environment = var.environment
  }
}

# 3. Virtual Machine (EC2 Instance)
resource "aws_instance" "app_server" {
  ami           = "ami-0c7217cdde317cfec" # Ubuntu Server 22.04 LTS
  instance_type = var.instance_type
  key_name      = var.ssh_key_name

  vpc_security_group_ids = [aws_security_group.app_sg.id]

  user_data = <<-EOF
              #!/bin/bash
              sudo apt-get update
              sudo apt-get install -y docker.io docker-compose
              sudo systemctl start docker
              sudo systemctl enable docker
              EOF

  tags = {
    Name        = "vetfeed-app-server"
    Environment = var.environment
  }
}

# 4. Database Security Group
resource "aws_security_group" "db_sg" {
  name        = "vetfeed-db-sg"
  description = "Allow database access from App VM"
  vpc_id      = aws_vpc.vetfeed_vpc.id

  ingress {
    from_port       = 1433
    to_port         = 1433
    protocol        = "tcp"
    security_groups = [aws_security_group.app_sg.id]
  }

  tags = {
    Name        = "vetfeed-db-sg"
    Environment = var.environment
  }
}

# 5. RDS SQL Server Database Instance
resource "aws_db_instance" "sqlserver" {
  identifier           = "vetfeed-db-production"
  engine               = "sqlserver-ex" # SQL Server Express
  engine_version       = "15.00"
  instance_class       = "db.t3.medium"
  allocated_storage    = 20
  db_name              = null # SQL Server does not support db_name at instance creation
  username             = var.db_username
  password             = var.db_password
  parameter_group_name = "default.sqlserver-ex-15.0"
  skip_final_snapshot  = true

  vpc_security_group_ids = [aws_security_group.db_sg.id]

  tags = {
    Name        = "vetfeed-db"
    Environment = var.environment
  }
}
