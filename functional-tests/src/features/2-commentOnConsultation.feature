Feature: Comment on a Consultation
  As a user of consultations
  I want to be able to login to make a comment
	I want to be able to comment at the consultation level

Background:
    Given I open the url "1/1/introduction"
    And I refresh
		When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL" and password "ACCOUNTS_PASSWORD"
		When I wait on element ".page-header" to exist
		When I click on the button "body [data-qa-sel='close-onboarding-modal']"
		And I pause for 1000ms

Scenario: User makes a comment at consultation level
		When I click on the button "[data-qa-sel='comment-on-whole-consultation']"
		And I pause for 1000ms
		Then I wait on element "body [data-qa-sel='comment-box-title']" for 10000ms to exist
    Then I expect that element "[data-qa-sel='comment-box-title']" contains the text "consultation"
		When I add "This is a Consultation comment" to the inputfield "[data-qa-sel='Comment-text-area']"
		And I click on the button "[data-qa-sel='submit-button']"
		And I pause for 1000ms
		Then I expect that element "[data-qa-sel='Comment-text-area']" contains the text "This is a Consultation comment"
		Then I expect that element "[data-qa-sel='submit-button']" contains the text "Saved"
		Then I click on the button "body [data-qa-sel='delete-comment-button']"

Scenario: User makes a comment at document level
		And I pause for 1000ms
		When I click on the button "[data-qa-sel='comment-on-consultation-document']"
		And I pause for 1000ms
		Then I wait on element "body [data-qa-sel='comment-box-title']" for 10000ms to exist
    Then I expect that element "[data-qa-sel='comment-box-title']" contains the text "document"
		When I add "This is a Document comment" to the inputfield "[data-qa-sel='Comment-text-area']"
		And I click on the button "[data-qa-sel='submit-button']"
		And I pause for 1000ms
		Then I expect that element "[data-qa-sel='Comment-text-area']" contains the text "This is a Document comment"
		Then I expect that element "[data-qa-sel='submit-button']" contains the text "Saved"
		Then I click on the button "body [data-qa-sel='delete-comment-button']"

Scenario: User makes a comment at chapter level
		And I pause for 1000ms
		When I select the 1st option for element "body [data-qa-sel='in-text-comment-button']"
		And I pause for 1000ms
		Then I wait on element "body [data-qa-sel='comment-box-title']" for 10000ms to exist
    Then I expect that element "[data-qa-sel='comment-box-title']" contains the text "chapter"
		When I add "This is a Chapter comment" to the inputfield "[data-qa-sel='Comment-text-area']"
		And I click on the button "[data-qa-sel='submit-button']"
		And I pause for 1000ms
		Then I expect that element "[data-qa-sel='Comment-text-area']" contains the text "This is a Chapter comment"
		Then I expect that element "[data-qa-sel='submit-button']" contains the text "Saved"
		Then I click on the button "body [data-qa-sel='delete-comment-button']"
