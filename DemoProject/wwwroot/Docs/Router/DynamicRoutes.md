# Dynamic Routes

Dynamic routes allow you to define URL paths that can capture parameters directly from the URL.
These parameters can be accessed and used within the route handler.

### Defining Dynamic Routes

- Use curly braces (`{}`) in the path to define dynamic parameters, e.g., `/product/{category}/{item}`.
- Parameters are passed to the handler in a dictionary, where the key is the parameter name and the value is the captured data.

### Example: Defining a Dynamic Route

```csharp
// Define a GET route with dynamic parameters
server.Router.Get("/product/{category}/{item}", (context, parameters) => {
    return server.BakeHtml(
        $@"<h1>{parameters["category"]}</h1><p>{parameters["item"]}</p>", 200);
});
