Feature: Comment on a Consultation
  As a user of consultations
  I want to be able to login to make a comment
	I want to be able to comment at the consultation level

Background:
    Given I open the url "1/1/introduction"
    And I refresh

Scenario: User makes a comment at consultation level
    When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL" and password "ACCOUNTS_PASSWORD"
		When I wait on element ".page-header" to exist
		When I click on the button ".onboarding__closeButton"
		And I pause for 1000ms
		When I click on the button "[data-qa-sel='comment-on-whole-consultation']"
		And I pause for 1000ms
		Then I wait on element "body .CommentBox__title" for 10000ms to exist
    Then I expect that element ".CommentBox__title" contains the text "Comment on:"
		When I add "This is a Consultation comment" to the inputfield "#Comment0"
		And I click on the button "#sidebar-panel > div > ul > li > section > form > input"
		And I pause for 1000ms
		Then I expect that element "#Comment0" contains the text "This is a Consultation comment"
		Then I expect that element "#sidebar-panel > div > ul > li > section > form > input" contains the text "Saved"
