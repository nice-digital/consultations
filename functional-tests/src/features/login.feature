Feature: Consultations Login
  As a user of consultations
  I want to be able to login to make a comment

Background:
    Given I open the url "1/1/introduction"
    And I refresh

Scenario: User not signed in signs in
    Given I am logged out of NICE accounts
		When I wait on element ".page-header" to exist
    When I click on the button "body [data-qa-sel='open-commenting-panel']"
    Then I expect that element "body #sidebar-panel" contains the text "Sign in to your NICE account to comment on this consultation. Don't have an account?"
		When I click on the button "body [data-qa-sel='close-commenting-panel']"
		And I pause for 1000ms
		When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL" and password "ACCOUNTS_PASSWORD"
		Then I wait on element "body .page-header" for 10000ms to exist
		When I click on the button "body [data-qa-sel='close-onboarding-modal']"
		And I refresh
		And I click on the element "body [data-qa-sel='open-commenting-panel']"
		Then I expect that element "body #sidebar-panel" contains the text "No comments yet"
