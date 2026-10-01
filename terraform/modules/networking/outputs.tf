output "target_group_arn" {
  description = "Target Group Consultations"
  value       = aws_lb_target_group.consultations_tg.arn
}
