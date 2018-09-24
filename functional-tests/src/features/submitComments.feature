Feature: Submit Comments on a Consultation
  As a user of consultations
  I want to be able to login to make a comment
	I want to be able to submit my comments for review by NICE

Background:
    Given I open the url "1/1/introduction"
    And I refresh
		When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL3" and password "ACCOUNTS_PASSWORD"
		When I wait on element ".page-header" to exist
		And I pause for 1000ms

Scenario: User makes a multiple comments and submits
		When I wait on element "[data-qa-sel='comment-on-consultation-document']" to be visible
		When I click on the button "[data-qa-sel='comment-on-consultation-document']"
		And I pause for 1000ms
		And I add "1" to the inputfield "#Comment-1"
		And I pause for 1000ms
		And I click on the button "[data-qa-sel='submit-button']"
		And I pause for 1000ms
		And I click on the button "[data-qa-sel='comment-on-consultation-document']"
		And I pause for 1000ms
		And I add "2" to the inputfield "#Comment-1"
		And I pause for 1000ms
		And I click on the button "[data-qa-sel='submit-button']"
		And I pause for 1000ms
		And I click on the button ".chapter > .title [data-qa-sel='in-text-comment-button']"
		And I pause for 1000ms
		And I add "3" to the inputfield "#Comment-1"
		And I click on the button "[data-qa-sel='submit-button']"
		When I click on the button "[data-qa-sel='review-all-comments']"
		And I wait on element "body [data-qa-sel='respond-no-responding-as-org']" to be visible
		And I pause for 1000ms
		And I click on the element "[data-qa-sel='respond-no-responding-as-org']"
		And I wait on element "body [data-qa-sel='respond-no-has-tobac-links']" to be visible
		And I pause for 1000ms
		And I click on the element "[data-qa-sel='respond-no-has-tobac-links']"
		And I click on the button "[data-qa-sel='submit-comment-button']"
		And I pause for 1000ms
		When I wait on element "[data-qa-sel='review-submitted-comments']" to be visible
		Then I expect that element "[data-qa-sel='changeable-page-header']" contains the text "Response submitted"
		When I click on the button "[data-qa-sel='review-submitted-comments']"
		When I wait on element "[data-qa-sel='Comment-text-area']" to be visible
		Then I expect that element "[data-qa-sel='Comment-text-area']" is not enabled
		Given I open the url "admin/DeleteAllSubmissionsFromUser?userId=38bb6df2-9ab8-4248-bb63-251b5424711a"
		Given I open the url "1/review"
		When I wait on element "[data-qa-sel='Comment-text-area']" to be visible
		And I pause for 1000ms
		And I delete all comments on the page

