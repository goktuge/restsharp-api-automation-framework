Feature: SIM Activation

  Scenario: Activate a SIM successfully
    Given I have a valid SIM activation request
    When I send the activation request
    Then the response status code should be 202
    And the activation status should be "Accepted"

  Scenario Outline: Reject invalid SIM activation requests
    Given I have an activation request without "<missingField>"
    When I send the activation request
    Then the response status code should be 400
    And the error message should be "<errorMessage>"

    Examples:
      | missingField | errorMessage           |
      | iccid        | ICCID is required      |
      | customerId   | CustomerId is required |
      | planCode     | PlanCode is required   |