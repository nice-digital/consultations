Feature: Consultations homepage feature
  As a user of consultations
  I want to be able to use navigation

  Background:
    Given I open the page ""

  Scenario: Load home page 
    When I click on text "Document View" in ".stacked-nav"
    Then I expect that element "h1" matches the text "For consultation comments"