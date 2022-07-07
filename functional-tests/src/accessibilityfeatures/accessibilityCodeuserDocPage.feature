Feature: Accessibility testing on Codeuser Document Page
	As a code user of consultations
	We want to be able to use generated organisation code
    We can check accessibility on Codeuser Document Page

  Background:
	Given I open the url "admin"
	When I log into the admin page with username "IDAM_EMAIL6" and password "IDAM_PASSWORD"
	Given I open the url "admin"

  Scenario: Accessibility testing on Codeuser Document Page
	When I add the indev GID "GID-NG10186" to the filter
	Then I click on consultation to generate and copy the organisation code
	Given I open the url "254/1/overview"
	When I log into consultation with copied organisation code
    Then the page should have no accessibility issues
