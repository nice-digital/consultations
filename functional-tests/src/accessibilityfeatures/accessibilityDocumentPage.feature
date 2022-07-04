Feature: Accessibility testing on Document page
	As a user of consultations
    We can check accessibility on Document page

  Background:
	Given I open the url "233/1/recommendations"
	When I log into accounts with username "IDAM_EMAIL2" and password "IDAM_PASSWORD"

  Scenario: Accessibility testing on Document page
    Then the page should have no accessibility issues
