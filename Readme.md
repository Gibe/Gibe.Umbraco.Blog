# Gibe.Umbraco.Blog

## Installation

Install using Nuget

``` install-package Gibe.Umbraco.Blog ```

## Composition

The types used to represent blog posts are controlled from the calling site, thus you are responseible for composing the blog service (`IBlogService<T>`). Type T must implement `IBlogPostModel`. 

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