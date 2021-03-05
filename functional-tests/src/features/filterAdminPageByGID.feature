Feature: Admin users can search by using the InDev GID ID
	As a user of consultations
	We want to be able to filter the consultations list by Indev GID

	Background:
		Given I open the url "admin"
		And I refresh
		When I log into the admin page with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"

	Scenario: User can search for a consultation by using its InDev GID ID
		Given I expect the result list count contains "Showing 1 to 25"
		When I add the indev GID "GID-TA10269" to the filter
		Then I expect the result list count contains "Showing 1 consultation"
		When I click on the cancel filter
		Then I expect the result list count contains "Showing 1 to 25"
