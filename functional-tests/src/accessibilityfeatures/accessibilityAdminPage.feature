Feature: Accessibility testing on admin page
	As a user of consultations
  We can check accessibility on admin page

  Background:
	Given I open the url "admin"
	When I log into the admin page with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"

  Scenario: Accessibility testing on admin page
    Then the page should have no accessibility issues
