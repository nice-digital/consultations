 Feature: Comment on a Document
   	As a user of consultations
  	I want to be able to login to make a comment
 		I want to be able to comment at the Document level

 Background:
    Given I open the url "158/3/introduction"
    And I refresh
 		When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL2" and password "ACCOUNTS_PASSWORD"
 		When I wait on element ".page-header" to exist
 		And I pause for 1000ms
		Given I delete all comments on the page

 Scenario: User makes a comment at document level
 		Given I comment on a Document
		When I add the comment "This is a Document comment" and submit
		Then I expect the comment box contains "This is a Document comment"
		And I pause for 1000ms
		Then I expect the comment save button displays "Saved"
 		Then I click on the button "body [data-qa-sel='delete-comment-button']"
