Feature: Consultations Login
  As a user of consultations
  I want to be able to login to make a comment

Background:
    Given I open the url "1/1/introduction"
    And I refresh

Scenario: User not signed in signs in
    Given I am logged out of NICE accounts
		When I wait on element ".page-header" to exist
    When I click on the button "body #js-drawer-toggleopen"
    Then I expect that element "body #sidebar-panel" contains the text "Sign in to your NICE account to comment on this consultation. Don't have an account?"
		# When I click on the button "body #js-drawer-toggleopen"
		# And I refresh
		# When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL" and password "ACCOUNTS_PASSWORD"
		# Then I wait on element "body .page-header" for 10000ms to exist
		# And I open the url "1/1/introduction"
		# And I refresh
		# And I click on the element "body #js-drawer-toggleopen"
		# Then I expect that element "body #sidebar-panel" contains the text "No comments yet"

Scenario: User makes a comment at consultation level
    When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL" and password "ACCOUNTS_PASSWORD"
		When I wait on element ".page-header" to exist
		When I click on the button "[data-qa-sel='comment-on-whole-consultation']"
		And I pause for 1000ms
		Then I wait on element "body .CommentBox__title" for 10000ms to exist
    Then I expect that element ".CommentBox__title" contains the text "Comment on:"
		When I add "This is a Consultation comment" to the inputfield "#Comment0"
		And I click on the button "#sidebar-panel > div > ul > li > section > form > input"
		Then I expect that element "#Comment0" contains the text "This is a Consultation comment"
		Then I expect that element "#sidebar-panel > div > ul > li > section > form > input" contains the text "Saved"
