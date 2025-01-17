## Gedetailleerde omschrijving

Als een student het proces voor een inschrijving aan het doorlopen is moet hij zich kunnen inschrijvingen voor opleidingsonderdelen binnen de richting waarvoor hij nog geen credits heeft verworven.

## Aanpassingen aan het domeinmodel

Het concept opleidingsonderdeelInschrijving moet toegevoegd worden. Deze is gekoppeld aan een inschrijving van een student en aan een opleidingsonderdeel.

## Relevante wireframes

Link naar de wireframes

## Acceptatiecriteria

```
  Background:
    Given Students
    | name | email      |  id|
    | Jan  | jan@kdg.be |  1 |
    | Piet | piet@kdg.be|  2 |

    Given Courses
    | name          | id  | credits |
    | Programmeren  | 1   | 6       |
    | Databanken    | 2   | 3       |


    Given CourseSubscriptions
    | studentId | courseId | creditReceived   |
    | 1         | 1        | TRUE             |
    | 1         | 2        | FALSE            |

    Given subscription
    | studentId | year | id |
    | 1         | 2023 | 1  |

    Scenario: Student subscribes to a course
      Given student with id 1 edits subscription with id 1
      When student adds course with id 2
      Then subscription with id 1 has a courseSubscriptions with courseId 2

    Scenario: Student subscribes to a course he already has
        Given student with id 1 edits subscription with id 1
        When student adds course with id 1
        Then subscription with id 1 has a courseSubscriptions with courseId 1
```

## Definition of ready checklist

\[Vink aan welk detail is toegevoegd. Als je eerlijk alles hebt afgevinkt mag je de user story als "refined" beschouwen\]

* [x] Genoeg detail toegevoegd, de toegewezen ontwikkelaar heeft voldoende detail om de user story zonder bijkomende vragen uit te werken.
* [x] 1 positief en 1 negatief scenario
* [ ] Nuttige wireframes gekoppeld
* [x] Gelinkte en blokkerende user stories geïdentificeerd (Linked Items)
* [ ] Ouder epic vastgelegd (indien van toepassing) (Child Items)
* [ ] Gewicht vastgelegd
* [ ] Prioriteit vastgelegd
* [ ] Alle instructies uit de template werden verwijderd