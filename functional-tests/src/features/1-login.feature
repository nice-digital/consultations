Feature: Consultations Login
  As a user of consultations
  I want to be able to login to make a comment

Background:
    Given I open the url "1/1/introduction"
    And I refresh

Scenario: User not signed in signs in
    Given I am logged out of NICE accounts
		When I wait on element "[data-qa-sel='comment-on-consultation-document']" to exist
		When I wait on element "body [data-qa-sel='open-commenting-panel']" to exist
    When I click on the button "body [data-qa-sel='open-commenting-panel']"
		When I wait on element "body [data-qa-sel='sign-in-banner']" to exist
		And I pause for 1000ms
    Then I expect that element "body [data-qa-sel='sign-in-banner']" contains the text "Sign in to your NICE account to comment on this consultation."
		And I pause for 1000ms
		When I click on the button "body [data-qa-sel='open-commenting-panel']"
		And I pause for 1000ms
		When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL" and password "ACCOUNTS_PASSWORD"
		And I pause for 1000ms
		Then I wait on element "body .page-header" for 10000ms to exist
		Then I expect that element "body [data-qa-sel='sign-in-banner']" does not exist
