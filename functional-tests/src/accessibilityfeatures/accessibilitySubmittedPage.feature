Feature: Accessibility testing on Submitted page
	As a user of consultations
    We can check accessibility on Submitted page

  Background:
	Given I open the url "409/1/recommendations"
	When I log into accounts with username "IDAM_EMAIL3" and password "IDAM_PASSWORD"

  Scenario: Accessibility testing on Submitted page
  	Given I comment on a Document
	Then I expect the comment box title contains "document"
	When I add the comment "This comment" and submit
	When I navigate to the Review Page
	And I submit my response
    Then the page should have no accessibility issues
