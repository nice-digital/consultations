Feature: Comment on a Document
	As a user of consultations
	We want to be able to login to make a comment
	We want to be able to comment at the Document level

	Background:
		Given I open the url "158/3/introduction"
		And I refresh
		When I log into accounts with username "IDAM_EMAIL2" and password "IDAM_PASSWORD"
		Given I open the url "158/3/introduction"
		Given I delete all comments on the page

	Scenario: User makes a comment at document level
		Given I comment on a Document
		When I add the comment "This is a Document comment" and submit
		Then I expect the comment box contains "This is a Document comment"
		Then I expect the comment save button displays "Saved"
		Then I click delete comment
