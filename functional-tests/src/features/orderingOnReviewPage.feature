Feature: Comment Ordering on the Review Page
  As a user of consultations
  I want to be able to login to make a comment
	I want to be able to comment at multiple levels
	I want them to be ordered and displayed on the review page

Background:
    Given I open the url "1/1/introduction"
    And I refresh
		When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL" and password "ACCOUNTS_PASSWORD"
		When I wait on element ".page-header" to exist
		And I pause for 1000ms
		Given I delete all comments on the page

Scenario: User makes multiple comments and views them on the Review page
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
		And I click on the button "[data-qa-sel='comment-on-consultation-document']"
		And I pause for 1000ms
		And I add "3" to the inputfield "#Comment-1"
		And I pause for 1000ms
		And I click on the button "[data-qa-sel='submit-button']"
		And I pause for 1000ms
		When I click on the button "[data-qa-sel='review-all-comments']"
		And I pause for 1000ms
		And I wait on element "[data-qa-sel='Comment-text-area']" to be visible
		Then I expect that element "[data-qa-sel='Comment-text-area']" contains the text "3"
		And I expect that element "[data-qa-sel='Comment-text-area']" contains the text "2"
		And I expect that element "[data-qa-sel='Comment-text-area']" contains the text "1"
		Given I delete all comments on the page


