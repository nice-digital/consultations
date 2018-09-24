 Feature: Comment on a Chapter
   	As a user of consultations
  	I want to be able to login to make a comment
 		I want to be able to comment at the Chapter level

 Background:
    Given I open the url "158/3/introduction"
    And I refresh
 		When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL2" and password "ACCOUNTS_PASSWORD"
 		When I wait on element ".page-header" to exist
 		And I pause for 1000ms
		Given I delete all comments on the page

Scenario: User makes a comment at chapter level
 		And I pause for 1000ms
		When I wait on element ".document-comment-container" to be visible
 		And I pause for 1000ms
 		When I click on the button ".chapter > .title [data-qa-sel='in-text-comment-button']"
 		And I pause for 1000ms
 		Then I wait on element "body [data-qa-sel='comment-box-title']" to be visible
    Then I expect that element "[data-qa-sel='comment-box-title']" contains the text "chapter"
 		When I add "This is a Chapter comment" to the inputfield "[data-qa-sel='Comment-text-area']"
 		And I click on the button "[data-qa-sel='submit-button']"
 		And I pause for 1000ms
 		Then I expect that element "[data-qa-sel='Comment-text-area']" contains the text "This is a Chapter comment"
		And I pause for 1000ms
 		Then I expect that element ".CommentBox:first-child .CommentBox__savedIndicator" contains the text "Saved"
 		Then I click on the button "body [data-qa-sel='delete-comment-button']"
