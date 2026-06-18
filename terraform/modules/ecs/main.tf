data "aws_ecr_image" "consultations_backend" {
  repository_name = var.backend_config.ecr_repository_name
  image_tag       = var.backend_config.ecr_image_tag
}

data "aws_ecr_image" "consultations_frontend" {
  repository_name = var.frontend_config.ecr_repository_name
  image_tag       = var.frontend_config.ecr_image_tag
}

locals {
  container_definitions = {
    dev  = "${path.module}/container-definitions-dev.tftpl.json"
    production = "${path.module}/container-definitions-production.tftpl.json"
  }
}

resource "aws_ecs_task_definition" "consultations_td" {
  family                   = "consultations"
  requires_compatibilities = ["FARGATE"]
  runtime_platform {
    operating_system_family = "LINUX"
    cpu_architecture        = "X86_64"
  }
  network_mode = "awsvpc"
  cpu          = "1024"
  memory       = "2048"
  container_definitions = templatefile(local.container_definitions[var.environment],
  {
    aws_region = var.aws_region
    backend_image_uri = data.aws_ecr_image.consultations_backend.image_uri
    backend_container_port = var.backend_config.container_port
    backend_env_appsettings_environment_accountsenvironment = var.backend_config.appsettings_environment_accountsenvironment
    backend_env_appsettings_environment_name =  var.backend_config.appsettings_environment_name
    backend_env_appsettings_environment_ssrport = var.backend_config.appsettings_environment_ssrport
    backend_env_appsettings_environment_ssrhost = var.backend_config.appsettings_environment_ssrhost
    backend_env_appsettings_environment_corsorigin = var.backend_config.appsettings_environment_corsorigin
    backend_env_connectionstrings_defaultconnection = var.backend_config.connectionstrings_defaultconnection
    backend_env_consultationlist_downloadroles_adminroles = var.backend_config.consultationlist_downloadroles_adminroles
    backend_env_consultationlist_downloadroles_teamroles = var.backend_config.consultationlist_downloadroles_teamroles
    backend_env_encryption_iv = var.backend_config.encryption_iv
    backend_env_encryption_key = var.backend_config.encryption_key
    backend_env_feeds_cachedurationseconds = var.backend_config.feeds_cachedurationseconds
    backend_env_feeds_indevapikey = var.backend_config.feeds_indevapikey
    backend_env_feeds_indevbasepath = var.backend_config.feeds_indevbasepath
    backend_env_feeds_indevdraftpreviewchapterfeedpath = var.backend_config.feeds_indevdraftpreviewchapterfeedpath
    backend_env_feeds_indevdraftpreviewdetailfeedpath = var.backend_config.feeds_indevdraftpreviewdetailfeedpath
    backend_env_feeds_indevidamconfig_apiidentifier = var.backend_config.feeds_indevidamconfig_apiidentifier
    backend_env_feeds_indevidamconfig_clientid = var.backend_config.feeds_indevidamconfig_clientid
    backend_env_feeds_indevidamconfig_clientsecret = var.backend_config.feeds_indevidamconfig_clientsecret
    backend_env_feeds_indevidamconfig_domain = var.backend_config.feeds_indevidamconfig_domain
    backend_env_feeds_indevlistfeedpath = var.backend_config.feeds_indevlistfeedpath
    backend_env_feeds_indevpublishedchapterfeedpath = var.backend_config.feeds_indevpublishedchapterfeedpath
    backend_env_feeds_indevpublisheddetailfeedpath = var.backend_config.feeds_indevpublisheddetailfeedpath
    backend_env_feeds_indevpublishedpreviewdetailfeedpath = var.backend_config.feeds_indevpublishedpreviewdetailfeedpath
    backend_env_gilliam_getclaimsurl = var.backend_config.gilliam_getclaimsurl
    backend_env_gilliam_gilliambasepath = var.backend_config.gilliam_gilliambasepath
    backend_env_gilliam_gilliamclientcertificatebase64 = var.backend_config.gilliam_gilliamclientcertificatebase64
    backend_env_globalnav_cookiebannerscript = var.backend_config.globalnav_cookiebannerscript
    backend_env_globalnav_script = var.backend_config.globalnav_script
    backend_env_globalnav_scriptie8 = var.backend_config.globalnav_scriptie8
    backend_env_indevdraftpreviewdetailfeedpath = var.backend_config.indevdraftpreviewdetailfeedpath
    backend_env_logging_serilogminlevel = var.backend_config.logging_serilogminlevel
    backend_env_status_apikey = var.backend_config.status_apikey
    backend_env_webappconfiguration_apiidentifier = var.backend_config.webappconfiguration_apiidentifier
    backend_env_webappconfiguration_authorisationserviceuri = var.backend_config.webappconfiguration_authorisationserviceuri
    backend_env_webappconfiguration_redirecturi = var.backend_config.webappconfiguration_redirecturi
    backend_env_webappconfiguration_postlogoutredirecturi = var.backend_config.webappconfiguration_postlogoutredirecturi
    backend_env_webappconfiguration_clientid = var.backend_config.webappconfiguration_clientid
    backend_env_webappconfiguration_clientsecret = var.backend_config.webappconfiguration_clientsecret
    backend_env_webappconfiguration_domain = var.backend_config.webappconfiguration_domain
    backend_env_webappconfiguration_googletrackingid = var.backend_config.webappconfiguration_googletrackingid
    backend_env_webappconfiguration_redisserviceconfiguration_connectionstring = var.backend_config.webappconfiguration_redisserviceconfiguration_connectionstring
    backend_env_webappconfiguration_redisserviceconfiguration_enabled = var.backend_config.webappconfiguration_redisserviceconfiguration_enabled
    frontend_image_uri = data.aws_ecr_image.consultations_frontend.image_uri
    frontend_container_port = var.frontend_config.container_port
    frontend_env_api_url = var.frontend_config.env_api_url
    frontend_env_port = var.frontend_config.env_port
    frontend_env_public_url = var.frontend_config.env_public_url
    frontend_env_react_app_accounts_environment = var.frontend_config.env_react_app_accounts_environment
    frontend_env_react_app_global_nav_script = var.frontend_config.env_react_app_global_nav_script
    frontend_env_react_app_global_nav_script_ie8 = var.frontend_config.env_react_app_global_nav_script_ie8
    frontend_env_react_app_cookie_banner_script = var.frontend_config.env_react_app_cookie_banner_script
  })
  execution_role_arn = var.iam_role_arn
  task_role_arn      = var.iam_role_arn
}

resource "aws_ecs_cluster" "consulations_cluster" {
  name = "consultations-cluster"

  setting {
    name  = "containerInsights"
    value = "enabled"
  }
}

resource "aws_ecs_service" "consultations" {
  name                 = "consulations-service"
  cluster              = aws_ecs_cluster.consulations_cluster.arn
  force_new_deployment = true
  triggers = {
    redeployment = plantimestamp()
  }
  capacity_provider_strategy {
    capacity_provider = "FARGATE"
    base              = 1
    weight            = 100
  }
  platform_version    = "LATEST"
  task_definition     = aws_ecs_task_definition.consultations_td.arn
  scheduling_strategy = "REPLICA"
  desired_count       = 2
  deployment_controller {
    type = "ECS"
  }
  deployment_minimum_healthy_percent = 100
  deployment_circuit_breaker {
    enable   = true
    rollback = true
  }
  network_configuration {
    assign_public_ip = true
    subnets          = var.networking_config.load_balancer_subnets
    security_groups  = var.networking_config.load_balancer_security_groups
  }
  load_balancer {
    container_name   = "consultations-backend"
    container_port   = var.backend_config.container_port
    target_group_arn = var.target_group_arn
  }
  health_check_grace_period_seconds = 180
  enable_execute_command = true
}