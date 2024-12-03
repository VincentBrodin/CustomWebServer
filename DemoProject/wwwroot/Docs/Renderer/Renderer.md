# Renderer

### Directory Breakdown:
- **`wwwroot/`**: The root directory where static assets and view-related templates are stored.
  - **`views/`**: The directory for all view templates used in rendering content.
    - **`layout/`**: Contains layout templates, such as `main.hbs`, that provide a common structure for the pages.
      - **`main.hbs`**: This is the primary layout file, used as a template wrapper for different pages.
    - **`pages/`**: Includes individual templates for different pages in the application.
    - **`partials/`**: Holds partial templates that can be reused across different pages and layouts, like headers, footers, or sidebars.
  
### Notes:
- **`main.hbs`**: This is typically the main layout file that includes common HTML structure (such as `<head>`, `<body>`, navigation, etc.) and allows individual page templates to inject their content into predefined sections.
- **Partials**: Partial templates are smaller, reusable components that can be embedded into different pages and layouts.

This file structure helps maintain modularity and clarity in managing templates and views within the renderer system.