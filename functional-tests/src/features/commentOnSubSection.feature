 Feature: Comment on a Sub-Section
   	As a user of consultations
  	I want to be able to login to make a comment
 		I want to be able to comment at the Sub-section level

 Background:
    Given I open the url "158/3/guidance"
 		When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL2" and password "ACCOUNTS_PASSWORD"
 		When I wait on element ".page-header" to exist
 		And I pause for 1000ms
		Given I delete all comments on the page

	Scenario: User makes a comment at sub-section level
 		# And I pause for 1000ms
 		# When I click on the button "[data-qa-sel='nav-list-item']:nth-of-type(4)"
 		And I pause for 10000ms
		Then I wait on element ".section:first-of-type > [data-qa-sel='in-text-comment-button']" to exist
 		When I click on the button ".section:first-of-type > [data-qa-sel='in-text-comment-button']"
 		And I pause for 1000ms
 		Then I wait on element "body [data-qa-sel='comment-box-title']" for 1000ms to exist
		And I pause for 1000ms
    Then I expect that element "[data-qa-sel='comment-box-title']" contains the text "subsection"
 		When I add "This is a Sub-section comment" to the inputfield "[data-qa-sel='Comment-text-area']"
 		And I click on the button "[data-qa-sel='submit-button']"
 		And I pause for 1000ms
 		Then I expect that element "[data-qa-sel='Comment-text-area']" contains the text "This is a Sub-section comment"
		And I pause for 1000ms
 		Then I expect that element "[data-qa-sel='submit-button']" contains the text "Saved"
 		Then I click on the button "body [data-qa-sel='delete-comment-button']"