Feature: The list of consultations is reduced when user searches by title
	As a user of consultations
	We want to be able to filter the consultations list by Title

	Background:
		Given I open the url "admin"
		And I refresh
		When I log into the admin page with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"
		Given I open the url "admin"

	Scenario: User can search for a consultation by its title
		Given I expect the result list count contains "Showing 1 to 25"
		When I add the indev GID "This is for testing unsaved comments" to the filter
		Then I expect the result list count contains "Showing 2 consultations"
		When I click on the cancel filter
		Then I expect the result list count contains "Showing 1 to 25"
