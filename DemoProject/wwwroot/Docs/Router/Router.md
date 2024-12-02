# Router

## `Get` Method

The `Get` method is used to define a new GET [route](/docs/router/route). It takes in the following parameters:

- **Route**: The URL route, relative to the base URL. For example, the index page would be at `/`.
- **Handler Function**: A function that handles the GET request. This function accepts two parameters:
  - `context`: An instance of `HttpListenerContext`, which provides access to the HTTP request and response.
  - `parameters`: A dictionary mapping dynamic route parameters (if any) to their corresponding values.

## `Post` Method

The `Post` method is similar to `Get`, but it is used to define a new POST route. It operates as follows:

- The server uses a function in the server class called `ReadBody`. This function takes an `HttpListenerRequest` as input and returns the body of the request as a string.
- The server also has a utility function called `BakeJson`, which converts a string or object into a JSON payload. This payload can then be served as a response to the user.


## Example Usage

Below is an example of how to define routes and handle requests using the `Get` and `Post` methods in your server:

```csharp
// Create a new instance of the server
Server server = new();

// Define a GET route for the home page
server.Router.Get("/", (context, parameters) => {
    // Return a simple HTML response
    return server.BakeHtml(
        @"<!DOCTYPE html>
        <html>
        <body>
            <h1>My First Heading </h1>
            <p>My first paragraph.</p>
        </body>
        </html>", 200);
});

// Define a GET route with dynamic parameters for a product page
server.Router.Get("/product/{section}/{item}", (context, parameters) => {
    // Use dynamic route parameters to customize the HTML content
    return server.BakeHtml(
        $@"<!DOCTYPE html>
        <html>
        <body>
            <h1>{parameters["section"]}</h1>
            <p>{parameters["item"]}</p>
        </body>
        </html>", 200);
});

// Define a POST route to respond with JSON
server.Router.Post("/hello", (context, parameters) => {
    // Create an anonymous object to send as JSON
    object json = new {
        hello = "world!"
    };

    // Return the JSON response
    return server.BakeJson(json, 200);
});

// Start the server
server.Start();
```
