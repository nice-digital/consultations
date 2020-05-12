Feature: The list of consultations is reduced when filter is completed
	As a user of consultations
	We want to be able to filter the consultations list by Indev GID

	Background:
		Given I open the url "admin"
		When I log into accounts with username "IDAM_EMAIL1" and password "IDAM_PASSWORD"

	Scenario: User is prompted to Save unsaved Comments
		Given I expect the result list count contains "Showing 1 to 25 of 227 consultations"
		When I add the indev GID "GID-TA10269" to the filter
		Then I expect the result list count contains "Showing 1 to 1 of 1 consultation"
