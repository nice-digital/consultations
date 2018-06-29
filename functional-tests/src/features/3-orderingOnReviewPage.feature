Feature: Comment on a Consultation
  As a user of consultations
  I want to be able to login to make a comment
	I want to be able to comment at the consultation level

Background:
    Given I open the url "1/1/introduction"
    And I refresh
		When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL" and password "ACCOUNTS_PASSWORD"
		When I wait on element ".page-header" to exist
		And I pause for 1000ms

Scenario: User makes a multiple comments
		When I click on the button "[data-qa-sel='comment-on-whole-consultation']"
		And I pause for 1000ms
		And I add "1" to the inputfield "#Comment0"
		And I click on the button "[data-qa-sel='submit-button']"
		And I click on the button "[data-qa-sel='comment-on-consultation-document']"
		And I pause for 1000ms
		And I add "2" to the inputfield "#Comment0"
		And I click on the button "[data-qa-sel='submit-button']"
		And I click on the button ".chapter > .title [data-qa-sel='in-text-comment-button']"
		And I pause for 1000ms
		And I add "3" to the inputfield "#Comment0"
		And I click on the button "[data-qa-sel='submit-button']"
		When I click on the button "[data-qa-sel='review-all-comments']"
		And I pause for 1000ms
		Then I expect that element "#Comment0" contains the text "3"
		And I expect that element "#Comment1" contains the text "2"
		And I expect that element "#Comment2" contains the text "1"
		Then I click on the button "body [data-qa-sel='delete-comment-button']"
		And I pause for 1000ms
		Then I click on the button "body [data-qa-sel='delete-comment-button']"
		And I pause for 1000ms
		Then I click on the button "body [data-qa-sel='delete-comment-button']"


