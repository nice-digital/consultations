resource "aws_ecr_repository" "consultations_backend" {
  name                 = var.backend_config.ecr_repository_name
  image_tag_mutability = "MUTABLE"
  encryption_configuration {
    encryption_type = "AES256"
  }
}

resource "aws_ecr_repository" "consultations_frontend" {
  name                 = var.frontend_config.ecr_repository_name
  image_tag_mutability = "MUTABLE"
  encryption_configuration {
    encryption_type = "AES256"
  }
}

data "aws_ecr_image" "consultations_backend" {
  repository_name = var.backend_config.ecr_repository_name
  image_tag       = var.backend_config.ecr_image_tag
}

data "aws_ecr_image" "consultations_frontend" {
  repository_name = var.frontend_config.ecr_repository_name
  image_tag       = var.frontend_config.ecr_image_tag
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
  container_definitions = jsonencode([
    {
      name      = "consultations-backend"
      image     = data.aws_ecr_image.consultations_backend.image_uri
      essential = true
      portMappings = [
        {
          containerPort = var.backend_config.container_port
          name          = "consultations-backend-tcp"
          protocol      = "tcp"
          appProtocol   = "http"
        }
      ]
      environment = [
        {
          name  = "AppSettings__Environment__AccountsEnvironment"
          value = var.backend_config.appsettings_environment_accountsenvironment
        },
        {
          name  = "AppSettings__Environment__Name"
          value = var.backend_config.appsettings_environment_name
        },
        {
          name  = "AppSettings__Environment__SsrPort"
          value = var.backend_config.appsettings_environment_ssrport
        },
        {
          name  = "AppSettings__Environment__SsrHost"
          value = var.backend_config.appsettings_environment_ssrhost
        },
        {
          name  = "AppSettings__Environment__CorsOrigin"
          value = var.backend_config.appsettings_environment_corsorigin
        },
        {
          name  = "ConnectionStrings__DefaultConnection"
          value = var.backend_config.connectionstrings_defaultconnection
        },
        {
          name  = "ConsultationList__DownloadRoles__AdminRoles"
          value = var.backend_config.consultationlist_downloadroles_adminroles
        },
        {
          name  = "ConsultationList__DownloadRoles__TeamRoles"
          value = var.backend_config.consultationlist_downloadroles_teamroles
        },
        {
          name  = "Encryption__IV"
          value = var.backend_config.encryption_iv
        },
        {
          name  = "Encryption__Key"
          value = var.backend_config.encryption_key
        },
        {
          name  = "Feeds__CacheDurationSeconds"
          value = var.backend_config.feeds_cachedurationseconds
        },
        {
          name  = "Feeds__IndevApiKey"
          value = var.backend_config.feeds_indevapikey
        },
        {
          name  = "Feeds__IndevBasePath"
          value = var.backend_config.feeds_indevbasepath
        },
        {
          name  = "Feeds__IndevDraftPreviewChapterFeedPath"
          value = var.backend_config.feeds_indevdraftpreviewchapterfeedpath
        },
        {
          name  = "Feeds__IndevDraftPreviewDetailFeedPath"
          value = var.backend_config.feeds_indevdraftpreviewdetailfeedpath
        },
        {
          name  = "Feeds__IndevIDAMConfig__APIIdentifier"
          value = var.backend_config.feeds_indevidamconfig_apiidentifier
        },
        {
          name  = "Feeds__IndevIDAMConfig__ClientId"
          value = var.backend_config.feeds_indevidamconfig_clientid
        },
        {
          name  = "Feeds__IndevIDAMConfig__ClientSecret"
          value = var.backend_config.feeds_indevidamconfig_clientsecret
        },
        {
          name  = "Feeds__IndevIDAMConfig__Domain"
          value = var.backend_config.feeds_indevidamconfig_domain
        },
        {
          name  = "Feeds__IndevListFeedPath"
          value = var.backend_config.feeds_indevlistfeedpath
        },
        {
          name  = "Feeds__IndevPublishedChapterFeedPath"
          value = var.backend_config.feeds_indevpublishedchapterfeedpath
        },
        {
          name  = "Feeds__IndevPublishedDetailFeedPath"
          value = var.backend_config.feeds_indevpublisheddetailfeedpath
        },
        {
          name  = "Feeds__IndevPublishedPreviewDetailFeedPath"
          value = var.backend_config.feeds_indevpublishedpreviewdetailfeedpath
        },
        {
          name  = "Gilliam__GetClaimsUrl"
          value = var.backend_config.gilliam_getclaimsurl
        },
        {
          name  = "Gilliam__GilliamBasePath"
          value = var.backend_config.gilliam_gilliambasepath
        },
        {
          name  = "Gilliam__GilliamClientCertificateBase64"
          value = var.backend_config.gilliam_gilliamclientcertificatebase64
        },
        {
          name  = "GlobalNav__CookieBannerScript"
          value = var.backend_config.globalnav_cookiebannerscript
        },
        {
          name  = "GlobalNav__Script"
          value = var.backend_config.globalnav_script
        },
        {
          name  = "GlobalNav__ScriptIE8"
          value = var.backend_config.globalnav_scriptie8
        },
        {
          name  = "IndevDraftPreviewDetailFeedPath"
          value = var.backend_config.indevdraftpreviewdetailfeedpath
        },
        {
          name  = "Logging__SerilogMinLevel"
          value = var.backend_config.logging_serilogminlevel
        },
        {
          name  = "Status__APIKey"
          value = var.backend_config.status_apikey
        },
        {
          name  = "WebAppConfiguration__ApiIdentifier"
          value = var.backend_config.webappconfiguration_apiidentifier
        },
        {
          name  = "WebAppConfiguration__AuthorisationServiceUri"
          value = var.backend_config.webappconfiguration_authorisationserviceuri
        },
        {
          name  = "WebAppConfiguration__RedirectUri"
          value = var.backend_config.webappconfiguration_redirecturi
        },
        {
          name  = "WebAppConfiguration__PostLogoutRedirectUri"
          value = var.backend_config.webappconfiguration_postlogoutredirecturi
        },
        {
          name  = "WebAppConfiguration__ClientId"
          value = var.backend_config.webappconfiguration_clientid
        },
        {
          name  = "WebAppConfiguration__ClientSecret"
          value = var.backend_config.webappconfiguration_clientsecret
        },
        {
          name  = "WebAppConfiguration__Domain"
          value = var.backend_config.webappconfiguration_domain
        },
        {
          name  = "WebAppConfiguration__GoogleTrackingId"
          value = var.backend_config.webappconfiguration_googletrackingid
        },
        {
          name  = "WebAppConfiguration__RedisServiceConfiguration__ConnectionString"
          value = var.backend_config.webappconfiguration_redisserviceconfiguration_connectionstring
        },
        {
          name  = "WebAppConfiguration__RedisServiceConfiguration__Enabled"
          value = var.backend_config.webappconfiguration_redisserviceconfiguration_enabled
        }
      ],
      logConfiguration = {
        logDriver = "awslogs"
        "options" = {
          awslogs-create-group  = "true"
          awslogs-group         = "/ecs/consultations"
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = "ecs"
        }
      },
      healthCheck = {
        command  = ["CMD-SHELL", "wget --spider -q http://0.0.0.0:80 || exit 1"]
        interval = 120
        timeout  = 60
        retries  = 3
      },
      dependsOn = [
        {
          containerName : "consultations-database",
          condition : "START"
        },
        {
          containerName : "consultations-redis",
          condition : "START"
        },
        {
          containerName : "consultations-frontend",
          condition : "START"
        }
      ]
    },
    {
      name      = "consultations-frontend"
      image     = data.aws_ecr_image.consultations_frontend.image_uri
      essential = true
      portMappings = [
        {
          hostPort      = var.frontend_config.container_port
          containerPort = var.frontend_config.container_port
          name          = "consultations-frontend-tcp"
          protocol      = "tcp"
          appProtocol   = "http"
        }
      ]
      environment = [
        {
          name  = "API_URL"
          value = aws_lb.consulations_lb.dns_name
        },
        {
          name  = "PORT"
          value = var.frontend_config.env_port
        },
        {
          name  = "PUBLIC_URL"
          value = var.frontend_config.env_public_url
        },
        {
          name  = "REACT_APP_HOTJARID"
          value = var.frontend_config.env_react_app_hotjarid
        },
        {
          name  = "REACT_APP_ACCOUNTS_ENVIRONMENT"
          value = var.frontend_config.env_react_app_accounts_environment
        },
        {
          name  = "REACT_APP_GLOBAL_NAV_SCRIPT"
          value = var.frontend_config.env_react_app_global_nav_script
        },
        {
          name  = "REACT_APP_GLOBAL_NAV_SCRIPT_IE8"
          value = var.frontend_config.env_react_app_global_nav_script_ie8
        },
        {
          name  = "REACT_APP_COOKIE_BANNER_SCRIPT"
          value = var.frontend_config.env_react_app_cookie_banner_script
        },
      ],
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-create-group  = "true"
          awslogs-group         = "/ecs/consultations"
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = "ecs"
        }
      }
    },
    {
      name      = "consultations-database"
      image     = "mcr.microsoft.com/mssql/server:2019-latest"
      essential = true
      portMappings = [
        {
          hostPort      = 1433
          containerPort = 1433
          name          = "consultations-database-tcp"
          protocol      = "tcp"
          appProtocol   = "http"
        }
      ]
      environment = [
        {
          name  = "ACCEPT_EULA"
          value = "Y"
        },
        {
          name  = "MSSQL_PID"
          value = "Express"
        },
        {
          name  = "SA_PASSWORD"
          value = "ABcd1234#"
        },
        {
          name  = "SA_USERNAME"
          value = "sa"
        },
        {
          name  = "COMPOSE_HTTP_TIMEOUT"
          value = "1000"
        },
      ],
      logConfiguration = {
        logDriver = "awslogs"
        "options" = {
          awslogs-create-group  = "true"
          awslogs-group         = "/ecs/consultations"
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = "ecs"
        }
      }
    },
    {
      name      = "consultations-redis"
      image     = "redis:alpine3.15"
      essential = true
      portMappings = [
        {
          hostPort      = 6379
          containerPort = 6379
          name          = "consultations-redis-tcp"
          protocol      = "tcp"
          appProtocol   = "http"
        }
      ]
      logConfiguration = {
        logDriver = "awslogs"
        "options" = {
          awslogs-create-group  = "true"
          awslogs-group         = "/ecs/consultations"
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = "ecs"
        }
      }
    },
  ])
  execution_role_arn = aws_iam_role.consultations_role.arn
  task_role_arn      = aws_iam_role.consultations_role.arn
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
    target_group_arn = aws_lb_target_group.consultations_tg.arn
  }
  health_check_grace_period_seconds = 180
  enable_execute_command = true
}


# networking
resource "aws_lb" "consulations_lb" {
  name               = "consultations-lb"
  load_balancer_type = "application"
  subnets            = var.networking_config.load_balancer_subnets
  security_groups    = var.networking_config.load_balancer_security_groups
}

resource "aws_lb_target_group" "consultations_tg" {
  name             = "consultations-tg"
  target_type      = "ip"
  protocol         = "HTTP"
  port             = var.backend_config.container_port
  ip_address_type  = "ipv4"
  vpc_id           = var.networking_config.target_group_vpc_id
  protocol_version = "HTTP1"
  stickiness {
    enabled = true
    type    = "lb_cookie"
  }
  health_check {
    enabled  = true
    interval = 180
    timeout  = 120
    matcher = "200-302"
  }
}

resource "aws_lb_listener" "consultations_listener" {
  load_balancer_arn = aws_lb.consulations_lb.arn
  port              = 80
  protocol          = "HTTP"

  default_action {
    target_group_arn = aws_lb_target_group.consultations_tg.arn
    type             = "forward"
  }
}

resource "aws_iam_role" "consultations_role" {
  name        = "consultations-execution-role"
  description = "Consultations execution role"
  path        = "/service-role/"
  assume_role_policy = jsonencode(
    {
      "Version" : "2012-10-17",
      "Statement" : [
        {
          "Sid" : "EcsTaskPolicy",
          "Effect" : "Allow",
          "Principal" : {
            "Service" : ["ecs-tasks.amazonaws.com"]
          },
          "Action" : "sts:AssumeRole"
        }
      ]
    }
  )
}

resource "aws_iam_role_policy" "consultations_ecs_task_execution" {
  name = "consultations-ecs-task-execution"
  role = aws_iam_role.consultations_role.id

  policy = jsonencode({
    "Version" : "2012-10-17",
    "Statement" : [
      {
        "Effect" : "Allow",
        "Action" : [
          "ecr:GetAuthorizationToken",
          "ecr:BatchCheckLayerAvailability",
          "ecr:GetDownloadUrlForLayer",
          "ecr:BatchGetImage",
          "logs:CreateLogStream",
          "logs:CreateLogGroup",
          "logs:PutLogEvents"
        ],
        "Resource" : "*"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "consultations_ecs_policy_attachment" {
  role       = aws_iam_role.consultations_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}