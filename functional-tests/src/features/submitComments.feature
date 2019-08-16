Feature: Submit Comments on a Consultation
  As a user of consultations
  We want to be able to login to make a comment
	We want to be able to submit my comments for review by NICE

Background:
    Given I open the url "1/1/introduction"
		When I log into accounts with username "ACCOUNTS_EMAIL3" and password "ACCOUNTS_PASSWORD"

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
		Given I delete submissions for userid "38bb6df2-9ab8-4248-bb63-251b5424711a"
		And I delete all comments on the page

