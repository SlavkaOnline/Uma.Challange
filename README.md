# Scenario: `test load`

- Duration: `00:05:00`
- RPS: `64`
- Concurrent Copies: `1000`

| __step__                 | __details__                                                             |
|--------------------------|-------------------------------------------------------------------------|
| name                     | `request`                                                               |
| request count            | all = `20480`, OK = `20480`, failed = `0`                               |
| response time            | RPS = `68`, min = `0`, mean = `162`, max = `2139`                       |
| response time percentile | 50% = `2`, 75% = `3`, 95% = `2077`, StdDev = `614`                      |
| data transfer            | min = `0.100 Kb`, mean = `0.110 Kb`, max = `0.110 Kb`, all = `2.200 MB` |
| name                     | `pause`                                                                 |
| request count            | all = `19480`, OK = `19480`, failed = `0`                               |
| response time            | RPS = `64`, min = `103`, mean = `14690`, max = `30012`                  |
| response time percentile | 50% = `14519`, 75% = `22127`, 95% = `28351`, StdDev = `8619`            |
| data transfer            | min = - , mean = - , max = - , all = -                                  |