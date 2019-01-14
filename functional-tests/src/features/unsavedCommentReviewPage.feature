Feature: User unable to submit when they have unsaved comments on Review page
   	As a user of consultations
  	I want to be able to login to make a comment
 		I want to be able to comment at the Document level
		I want to be warned if I attempt to navigate away while having unsaved Comments

 Background:
    Given I open the url "234/1/recommendations"
		When I log into accounts with username "ACCOUNTS_EMAIL2" and password "ACCOUNTS_PASSWORD"

 Scenario: User is unable to Submit when there are unsaved comments
 		And I pause for 1000ms
		And I wait on element "body [data-qa-sel='comment-on-consultation-document']" for 10000ms to be visible
 		When I click on the button "[data-qa-sel='comment-on-consultation-document']"
 		And I pause for 1000ms
 		Then I wait on element "body [data-qa-sel='comment-box-title']" for 10000ms to be visible
    Then I expect that element "[data-qa-sel='comment-box-title']" contains the text "document"
		When I add "This comment" to the inputfield "[data-qa-sel='Comment-text-area']"
		And I click on the button "[data-qa-sel='submit-button']"
		And I pause for 1000ms
		When I click on the button "[data-qa-sel='review-all-comments']"
		And I pause for 1000ms
		And I add ". must be saved" to the inputfield "[data-qa-sel='Comment-text-area']"
		And I pause for 1000ms
		And I wait on element "body [data-qa-sel='respond-no-responding-as-org']" to be visible
		And I pause for 1000ms
		And I click on the element "[data-qa-sel='respond-no-responding-as-org']"
		And I wait on element "body [data-qa-sel='respond-no-has-tobac-links']" to be visible
		And I pause for 1000ms
		And I click on the element "[data-qa-sel='respond-no-has-tobac-links']"
		Then I expect that element "[data-qa-sel='submit-comment-button']" is not enabled




