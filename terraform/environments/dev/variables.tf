variable "aws_region" {
  type    = string
  default = "eu-west-1"
}

variable "environment" {
  type    = string
  validation {
    condition= contains(["dev", "production"], var.environment)
    error_message = "Invalid environment value."
  }
}

variable "backend_config" {
  description = "Backend configuration"
  type = object({
    ecr_repository_name = string
    ecr_image_tag       = string
    container_port      = number

    appsettings_environment_accountsenvironment = string
    appsettings_environment_name                = string
    appsettings_environment_ssrport             = string
    appsettings_environment_ssrhost             = string
    appsettings_environment_corsorigin          = string

    connectionstrings_defaultconnection = string

    consultationlist_downloadroles_adminroles = string
    consultationlist_downloadroles_teamroles  = string

    encryption_iv  = string
    encryption_key = string

    feeds_cachedurationseconds                = string
    feeds_indevapikey                         = string
    feeds_indevbasepath                       = string
    feeds_indevdraftpreviewchapterfeedpath    = string
    feeds_indevdraftpreviewdetailfeedpath     = string
    feeds_indevidamconfig_apiidentifier       = string
    feeds_indevidamconfig_clientid            = string
    feeds_indevidamconfig_clientsecret        = string
    feeds_indevidamconfig_domain              = string
    feeds_indevlistfeedpath                   = string
    feeds_indevpublishedchapterfeedpath       = string
    feeds_indevpublisheddetailfeedpath        = string
    feeds_indevpublishedpreviewdetailfeedpath = string

    gilliam_getclaimsurl                   = string
    gilliam_gilliambasepath                = string
    gilliam_gilliamclientcertificatebase64 = string

    globalnav_cookiebannerscript = string
    globalnav_script             = string
    globalnav_scriptie8          = string

    indevdraftpreviewdetailfeedpath = string

    logging_serilogminlevel = string

    status_apikey = string

    webappconfiguration_apiidentifier                              = string
    webappconfiguration_authorisationserviceuri                    = string
    webappconfiguration_redirecturi                                = string
    webappconfiguration_postlogoutredirecturi                      = string
    webappconfiguration_clientid                                   = string
    webappconfiguration_clientsecret                               = string
    webappconfiguration_domain                                     = string
    webappconfiguration_googletrackingid                           = string
    webappconfiguration_redisserviceconfiguration_connectionstring = string
    webappconfiguration_redisserviceconfiguration_enabled          = string
  })
}

variable "frontend_config" {
  description = "Frontend configuration"
  type = object({
    ecr_repository_name                 = string
    ecr_image_tag                       = string
    container_port                      = number,
    env_api_url                         = string,
    env_port                            = string,
    env_public_url                      = string,
    env_react_app_hotjarid              = string,
    env_react_app_accounts_environment  = string,
    env_react_app_global_nav_script     = string,
    env_react_app_global_nav_script_ie8 = string
    env_react_app_cookie_banner_script  = string
  })
}

variable "networking_config" {
  description = "Networking configuration"
  type = object({
    load_balancer_subnets         = list(string)
    load_balancer_security_groups = list(string)
    target_group_vpc_id           = string
    certificate_arn               = string
    hosted_zone_id                = string
    hostname                      = string
  })
}