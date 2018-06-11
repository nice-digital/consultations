# Feature: Comment on a Consultation
#   As a user of consultations
#   I want to be able to login to make a comment
# 	I want to be able to comment at the consultation level

# Background:
#     Given I open the url "1/1/introduction"
#     And I refresh
# 		And I wait on element ".page-header" to exist
# 		And I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL" and password "ACCOUNTS_PASSWORD"
# 		Then I wait on element "body .page-header" for 10000ms to exist

# Scenario: User makes a comment at consultation level
#     When I click on the button "#root > div > div > div > main > div.page-header > p:nth-child(1) > button"
#     Then I expect that element "body #sidebar-panel" contains the text "Comment on: consultation"
# 		When I add "This is a Consultation comment" to the inputfield ".form__input form__input--textarea"
# 		And I click on the button "#sidebar-panel > div > ul > li > section > form > input"
# 		Then I expect that element "#sidebar-panel > div > ul > li > section > form > input" contains the text "Saved"
