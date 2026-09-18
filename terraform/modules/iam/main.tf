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