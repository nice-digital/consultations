variable "backend_config" {
  description = "Backend configuration"
  type = object({
    container_port = number
  })
}

variable "networking_config" {
  description = "Networking configuration"
  type = object({
		assign_public_ip              = string
    load_balancer_subnets         = list(string)
    load_balancer_security_groups = list(string)
    target_group_vpc_id           = string
    certificate_arn               = string
    hosted_zone_id                = string
    hostname                      = string
		environment_name              = string
		application_name              = string
  })
}

