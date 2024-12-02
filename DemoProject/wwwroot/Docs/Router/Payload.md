# Payload Structure

The `Payload` struct is used to represent the data returned in an HTTP response. It encapsulates three key elements: the content, the content type, and the HTTP status code.

### Payload Properties

- **Content**: A nullable byte array (`byte[]?`) that holds the actual data of the response. This could be any type of content, such as HTML, JSON, or binary data.
  
- **ContentType**: A nullable string (`string?`) that specifies the MIME type of the content (e.g., `text/html`, `application/json`). This informs the client of the type of data being returned.

- **Status**: An integer representing the HTTP status code (e.g., `200` for success, `404` for not found). It defines the outcome of the HTTP request.

### Constructors

The `Payload` struct provides two constructors to initialize a response:

1. A constructor that accepts content, content type, and an optional status code (default is `200`).
2. A constructor that accepts only the status code (default is `200`), useful for empty responses or when no content is needed.

### Methods

- **Empty()**: This method checks if the payload is empty, meaning either the `Content` or `ContentType` is `null`.

### Summary

The `Payload` struct serves as a container for the data sent in an HTTP response. It allows the server to specify the response's content, type, and status in a structured way.
