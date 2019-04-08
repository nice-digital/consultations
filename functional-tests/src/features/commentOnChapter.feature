 Feature: Comment on a Chapter
   	As a user of consultations
  	We want to be able to login to make a comment
 		We want to be able to comment at the Chapter level

 Background:
    Given I open the url "158/3/introduction"
		When I log into accounts with username "ACCOUNTS_EMAIL2" and password "ACCOUNTS_PASSWORD"
		Given I delete all comments on the page

Scenario: User makes a comment at chapter level
		Given I comment on a Chapter
		Then I expect the comment box title contains "chapter"
		When I add the comment "This is a Chapter comment" and submit
		Then I expect the comment box contains "This is a Chapter comment"
		Then I expect the comment save button displays "Saved"
 		Then I click delete comment
