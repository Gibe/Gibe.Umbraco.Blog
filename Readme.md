# Gibe.Umbraco.Blog

## Installation

Install using Nuget

``` install-package Gibe.Umbraco.Blog ```

## Composition

The types used to represent blog posts are controlled from the calling site, thus you are responsible for composing the blog service (`IBlogService<T>`). Type T must implement `IBlogPostModel`. 

## Config

In most cases you do not need to configure anything. However, if necessary you may override certain details. 

| Key                             | Default                   | Purpose                                                                                              |
|---------------------------------|---------------------------|------------------------------------------------------------------------------------------------------|
| `IndexName`                     | `ExternalIndex`           | Sets the index in which we'll look for blog posts.                                                   |
| `BlogPostDocumentTypeAlias`     | `blogPost`                | Defines which type of documents we'll look for.                                                      |
| `UserPickerPropertyEditorAlias` | `Umbraco.UserPicker`      | *Only used during initialisation.* The property editor type to use for blog post author user picker. |
| `UserPickerName`                | `User Picker - All Users` | *Only used during initialisation.* The name of the the blog post author user picker data type.       |

## Doctype

Upon running the site, a document type will be created which will be named 'Blog Post Composition'. This document type comtains the all properties necessary for the module to function. As the name suggests, it's intended to be used as a composition which will make up part of the actual blog post document type. 

Creation of the wider 'blog post' document type is up to you. The alias of said document type must match `BlogPostDocumentTypeAlias`.

## Testing locally

`scripts/install-demo-site.ps1` (or `scripts/install-demo-site.sh`) scaffolds a local Umbraco site referencing this repo's `Gibe.Umbraco.Blog` project, installs uSync, and imports a set of demo doctypes and blog posts (`scripts/uSync-seed`) so the package can be exercised without building any of that by hand.

```
scripts/install-demo-site.ps1
```

This creates `demo/Gibe.Umbraco.Blog.DemoSite` plus a `Gibe.Umbraco.Blog.local.slnx` solution referencing both the library and the demo site. Open the solution, build, and run - uSync is configured to import the seed content at startup. If it doesn't appear, log into `/umbraco` and run the import manually from the uSync dashboard. Login is `admin@example.com` / `password1234`.