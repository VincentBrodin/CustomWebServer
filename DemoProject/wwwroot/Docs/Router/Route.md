# Route

A route is defined by three components: a **path**, a **handler**, and a **method**.

- **Path**: The path represents the specific URL segment after the root. For example, on `localhost:8080`, the root URL is `/`, and `/about` would represent the About page.
  
- **Handler**: The handler is responsible for processing the request and determining how to respond. It returns a [payload](/docs/router/payload), which defines the content type to be served.

- **Method**: The method is an enumeration with two possible values as of now: `GET` and `POST`. These refer to the HTTP methods used to send and retrieve data.
