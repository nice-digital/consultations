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
    And I refresh

  Scenario: Load home page 
    #When I click on the link "Patient-centred care"
    When I wait on element ".page-header" to exist
    Then I expect that element "#root > div > div > div > main > div.page-header > h1" matches the text "Unstable angina and NSTEMI: early management"
    And I expect that element ".page-header h2" matches the text "Unstable angina and NSTEMI"  

  Scenario: Comment Panel can be rolled down
    When I wait on element ".page-header" to exist
    Given I expect that element "root > div.Drawer.Drawer--open" is not visible
    When I click on the element "#js-drawer-toggleopen"
    Then I expect that element "root > div.Drawer.Drawer--open" is visible

   Scenario: Get Cookie
    When I wait on element ".page-header" to exist
    When I click on the element "#js-drawer-toggleopen"
    #When I set a cookie "__nrpa_2.2" with the content "SJQxrQET32KYHUYHY6eOSLldyy87hHkQpuxX9Sh/Z3k+V2UENlYwe4T5i5CDTf4Ltn2I35Yen+unTSHs08TXcg=="
    Then I expect that element "#sidebar-panel" contains the text "No comments" 

