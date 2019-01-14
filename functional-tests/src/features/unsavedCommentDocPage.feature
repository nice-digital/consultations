Feature: User is alerted when there are unsaved comments on Document page
   	As a user of consultations
  	I want to be able to login to make a comment
 		I want to be able to comment at the Document level
		I want to be warned if I attempt to navigate away while having unsaved Comments

 Background:
    Given I open the url "233/1/recommendations"
		When I log into accounts with username "ACCOUNTS_EMAIL2" and password "ACCOUNTS_PASSWORD"

 Scenario: User is prompted to Save unsaved Comments
 		And I pause for 1000ms
		And I wait on element "body [data-qa-sel='comment-on-consultation-document']" for 10000ms to be visible
 		When I click on the button "[data-qa-sel='comment-on-consultation-document']"
 		And I pause for 1000ms
 		Then I wait on element "body [data-qa-sel='comment-box-title']" for 10000ms to be visible
    Then I expect that element "[data-qa-sel='comment-box-title']" contains the text "document"
		When I add "This comment must be saved" to the inputfield "[data-qa-sel='Comment-text-area']"
		And I pause for 1000ms
		When I click on the button "[data-qa-sel='review-all-comments']"
		Then I expect that a alertbox contains the text "You have 1 unsaved change. Continue without saving?"
		When I dismiss the confirmbox
		And I pause for 1000ms
		When I click on the button "[data-qa-sel='comment-on-consultation-document']"
 		And I pause for 1000ms
 		Then I wait on element "body [data-qa-sel='comment-box-title']" for 10000ms to be visible
		And I pause for 1000ms
		And I add "This comment must also be saved" to the inputfield "#Comment-2"
		And I pause for 1000ms
		When I click on the button "[data-qa-sel='review-all-comments']"
		Then I expect that a alertbox contains the text "You have 2 unsaved changes. Continue without saving?"




