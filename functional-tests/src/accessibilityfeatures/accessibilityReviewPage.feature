Feature: Accessibility testing on Review response page
	As a user of consultations
    We can check accessibility on Review response page

  Background:
	Given I open the url "413/1/recommendations"
	When I log into accounts with username "IDAM_EMAIL2" and password "IDAM_PASSWORD"

  Scenario: Accessibility testing on Review response page
  	Given I comment on a Document
	Then I expect the comment box title contains "document"
	When I add the comment "This comment" and submit
	When I navigate to the Review Page
    Then the page should have no accessibility issues
