Feature: User is able to view hidden consultation with assigned roles
	As a user of consultations
	We want user with assigned access roles to view hidden consultation

	Background:
		Given I open the url "admin"
		When I log into the admin page with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"
		Given I open the url "admin"

	Scenario: User is able to view hidden consultation with assigned roles
		Given I expect the result list count contains "Showing 1 to 25"
		When I add the indev GID "GID-QS10096" to the filter
		Then I expect the result list count contains "Showing 1 consultation"
		When I click on the cancel filter
		Then I expect the result list count contains "Showing 1 to 25"
		When I add the indev GID "GID-MT530" to the filter
		Then I expect the result list count contains "Showing 1 consultation"
