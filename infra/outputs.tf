output "app_server_public_ip" {
  value       = aws_instance.app_server.public_ip
  description = "The public IP address of the app server EC2 instance."
}

output "database_endpoint" {
  value       = aws_db_instance.sqlserver.endpoint
  description = "The connection endpoint for the SQL Server database."
}
