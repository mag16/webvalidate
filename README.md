# webvalidate - A web request validation testing tool

![License](https://img.shields.io/badge/license-MIT-green.svg)

Run the Validation Test

```bash

# run the tests in the container
docker run -it --rm retaildevcrew/webvalidate --host https://www.microsoft.com --files msft.json

```

## Test Validation

Validation files are located in the src/TestFiles directory and are json files that control the validation tests.

- Url (required)
  - path to resource (do not include http or dns name)
  - valid: must begin with /
- Verb
  - default: GET
  - valid: HTTP verbs
- Validation (optional)
  - if not specified in test file, no validation checks will run
  - Code (required)
    - http status code
    - default: 200
    - valid: 100-599
  - ContentType (required)
    - MIME type
    - default: application/json
    - valid: non-empty string
  - MinLength
    - minimum content length
    - valid: >= 0
  - MaxLength
    - maximum content length
    - valid: >= MinLength
  - MaxMilliSeconds
    - maximum duration in ms
    - valid: > 0
  - ExactMatch
    - Body exactly matches value
    - valid: non-empty string
  - Contains[]
    - IsCaseSensitive
      - default: true
    - Value
      - valid: non-empty string
  - JsonArray
    - valid: parses into json array
    - Count
      - exact number of items
      - Valid: > 0
    - MinCount
      - minimum number of items
      - valid: >= 0
        - can be comined with MaxCount
    - MaxCount
      - maximum number of items
      - valid: > MinCount
        - can be comined with MinCount
    - CountIsZero
      - Array length == 0
      - valid: bool
  - JsonObject[]
    - valid: parses into json object
    - Field
      - name of field
      - valid: non-empty string
    - Value
      - valid: null or non-empty string
        - null checks for the presence of the field only
- PerfTarget (optional)
  - Category
    - used to group requests into categories for reporting
    - valid: non-empty string
  - Targets[3]
    - maximum quartile value in ascending order

## Sample json

```json

{
    "Url":"/version",
    "Validation":{
        "Code":200,
        "ContentType":"text/plain"
    }
}

{
    "Url":"/healthz",
    "PerfTarget":{
        "Category":"healthz"
    },
    "Validation":{
        "ContentType":"text/plain",
        "ExactMatch":{
            "Value":"pass"
        }
    }
}

{
    "Url":"/healthz/ietf",
    "PerfTarget":{
        "Category":"healthz"
    },
    "Validation":{
        "ContentType":"application/health+json",
        "JsonObject":[{
            "Field":"status","Value":"pass"
        }]
    }
}

{
    "Url":"/api/actors",
    "PerfTarget":{
        "Category":"PagedRead"
    },
    "Validation":{
        "JsonArray":{
            "Count":100
        }
    }
}

```

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit [Microsoft Contributor License Agreement](https://cla.opensource.microsoft.com).

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
