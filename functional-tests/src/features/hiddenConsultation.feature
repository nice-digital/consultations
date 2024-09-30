Feature: External User unable to view hidden consultation.
	As a user of consultations
	We want to hide consultation from external user if roles are not assigned


	Background:
		Given I open the url "admin"
		When I log into the admin page with username "IDAM_EMAIL7" and password "IDAM_PASSWORD"
		Given I open the url "admin"

	Scenario: External user unable to view hidden consultation
		Given I expect the result list count contains "Showing 1 to 25"
		When I add the indev GID "GID-QS10096" to the filter
		Then I expect the result list count contains "Showing 0 consultations"
		When I click on the cancel filter
		Then I expect the result list count contains "Showing 1 to 25"
		When I add the indev GID "GID-MT530" to the filter
		Then I expect the result list count contains "Showing 0 consultations"
		When I click on the cancel filter
		Then I expect the result list count contains "Showing 1 to 25"
		When I add the indev GID "GID-DG10076" to the filter
		Then I expect the result list count contains "Showing 1 consultation"


