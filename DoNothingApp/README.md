# DoNothingAPI

Demonstrates an asynchronous function.

## GET /api/DoNothingAPI?name=<name>

* Calls GET /api/DoNothing2API?name=<name> and returns response body from that call.

## POST /api/DoNothingAPI

* Calls GET /api/DoNothing2API?name=<name> and returns response body from that call.

# DoNothing2API

Demonstrates a synchronous function.

## GET /api/DoNothing2API?name=<name>

* If name is specified, the API responds with `Hello, {name}. This HTTP triggered function executed successfully.`
* Otherwise, the API responds with `This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.`

## POST /api/DoNothing2API

* If request body is specified, the API responds with `Hello, {name}. This HTTP triggered function executed successfully.`
* Otherwise, the API responds with `This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.`