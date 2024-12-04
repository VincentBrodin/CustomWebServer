# Stratus Documentation

Welcome to the official documentation for **Stratus**, an ultra-lightweight .NET backend framework designed for flexibility, performance, and ease of use.

Stratus provides a simple and customizable structure that allows developers to create their own routers and rendering engines through intuitive interfaces, rather than being forced to work within rigid structures. It’s built to give you full control over your application, ensuring you can design it exactly how you want.

## Installation

### **Package Manager (PM)**
```bash
NuGet\Install-Package Stratus -Version 1.0.0
```

### **dotnet CLI**
```bash
dotnet add package Stratus --version 1.0.0
```

### **PackageReference**
```XML
<PackageReference Include="Stratus" Version="1.0.0" />
```

## Setup

```csharp
using Stratus;
public static class Program {
	public async static void Main(string[] args) {
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
		await server.Start();
	}

}
```

## Tips for Managing `wwwroot` Folder in .NET Projects

If you want to automatically copy the contents of your `wwwroot` folder to the build output directory during the build process, you can add the following snippet to your `.csproj` file. This will ensure that all files in the `wwwroot` directory are included in the build output.

**Note:** This approach will not delete any files from the output's `wwwroot` folder, so if you need to remove any files manually, you will have to do so separately.

### Add the following to your `.csproj` file:

```xml
<ItemGroup>
  <None Update="wwwroot\**">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

