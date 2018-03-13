Feature: Consultations homepage feature
  As a user of consultations
  I want to be able to use navigation

  Scenario: Load home page 
    Given I am on the homepage 
    When I click on Fetch Data in the nav
    Then I should see weather forecast