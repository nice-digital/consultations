Feature: External User can access download page and only the consultations commented appear by default
	As an external user of consultations
	We want to be able to login to make a comment
	We want to be able to submit my comment
	We want to be able to see the consultations I have commented on on the download page

	Background:
		Given I open the url "472/1/quality-statements"
		And I refresh
		When I log into accounts with username "IDAM_EMAIL5" and password "IDAM_PASSWORD"
		Given I open the url "472/1/quality-statements"
		And I refresh

	Scenario: User makes a comment and submits it to NICE and when they view the download page it shows they have commented
		Given I comment on a Document
		When I add the comment "This is a comment" to the first in the list and submit
		Then I expect the comment save button displays "Saved"
		When I navigate to the Review Page
		And I submit my response
		When I open the url "admin"
		Then I expect the result list count contains "Showing 1 consultation"
		Then I expect the your responses filter to be selected by default

