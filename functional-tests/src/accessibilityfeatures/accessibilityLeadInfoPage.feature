Feature: Accessibility testing on Lead Info page
	As a user of consultations
  We can check accessibility on Lead Info page

  Background:
	Given I open the url "admin"
	When I log into the admin page with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"

  Scenario: Accessibility testing on Lead Info page
	Given I click on the request commenting lead permision link
    Then the page should have no accessibility issues
