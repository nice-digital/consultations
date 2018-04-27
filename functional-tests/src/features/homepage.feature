Feature: Consultations homepage feature
  As a user of consultations
  I want to be able to use navigation

  #Background:
    #Given I open the page ""

  #Scenario: Load home page 
    #When I click on text "Document View" in ".stacked-nav"
    #Then I expect that element "h1" matches the text "For consultation comments"

  Background:
    Given I open the url "1/1/introduction"
    When I set a cookie "__nrpa_2.2" with the content "PqE2orjsjYMsNAEtKWLzoyibMbunIZHYqA9UwmGksTLXtvNKrYg9OBck8duFex/ET7Ms+4A4vSunuxuWNtmdTA=="
    And I refresh

  Scenario: Load home page 
    #When I click on the link "Patient-centred care"
    When I wait on element ".page-header" to exist
    Given I debug
    Then I expect that element "h1" matches the text "Unstable angina and NSTEMI: early management"
    And I expect that element ".page-header h2" matches the text "Unstable angina and NSTEMI"  

  Scenario: Comment Panel can be rolled down
    When I wait on element ".page-header" to exist
    Given the element "#root > div.Drawer.Drawer--open" is visible
    When I click on the element "#root > div.Drawer.Drawer--open > div.Drawer__controls > button"
    And I debug
    Then I expect that element "root > div.Drawer.Drawer--open" is not visible

   Scenario: Get Cookie
    When I wait on element ".page-header" to exist
    #When I set a cookie "__nrpa_2.2" with the content "SJQxrQET32KYHUYHY6eOSLldyy87hHkQpuxX9Sh/Z3k+V2UENlYwe4T5i5CDTf4Ltn2I35Yen+unTSHs08TXcg=="
    Then I debug
    Then I expect that element "#root > div.Drawer.Drawer--open > div.Drawer__main > ul > li > form > textarea" contains the text "authenticated comment insert test" 

