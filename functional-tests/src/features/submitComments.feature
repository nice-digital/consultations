Feature: Submit Comments on a Consultation
	As a user of consultations
	We want to be able to login to make a comment
	We want to be able to submit my comments for review by NICE

	Background:
		Given I open the url "1/1/introduction"
		And I refresh
		When I log into accounts with username "IDAM_EMAIL3" and password "IDAM_PASSWORD"

	Scenario: User makes a multiple comments and submits
		Given I comment on a Document
		When I add the comment "1" to the first in the list and submit
		Then I expect the comment save button displays "Saved"
		Given I comment on a Document
		When I add the comment "2" to the first in the list and submit
		Then I expect the comment save button displays "Saved"
		Given I comment on a Document
		When I add the comment "3" to the first in the list and submit
		Then I expect the comment save button displays "Saved"
		When I navigate to the Review Page
		And I submit my response
		And I review my response
		Then I expect all comment boxes are inactive
		Given I delete submissions for userid "auth0|5e3983aacb0dc60d9317d1d5"
		And I delete all comments on the page

