# Installing

Install using Nuget

``` install-package Gibe.Umbraco.Blog ```

## Setting up the Examine indexes

Add this to ExamineSettings.config 

In ExamineIndexProviders > Providers
```xml
<add name="BlogIndexer" type="UmbracoExamine.UmbracoContentIndexer, UmbracoExamine"/>
```

In ExamineSearchProviders > Providers
```xml
<add name="BlogSearcher" type="UmbracoExamine.UmbracoExamineSearcher, UmbracoExamine" />
```

Add this to ExamineIndex.config

```xml 
<IndexSet SetName="BlogIndexSet" IndexPath="~/App_Data/TEMP/ExamineIndexes/{machinename}/Blog/">
	<IndexAttributeFields>
		<add Name="id" />
		<add Name="nodeName" />
		<add Name="updateDate" />
		<add Name="writerName" />
		<add Name="path" />
		<add Name="nodeTypeAlias" />
		<add Name="parentID" />
	</IndexAttributeFields>
	<IndexUserFields>
		<add Name="postDate" Type="DateTime" EnableSorting="true" />
		<add Name="settingsNewsTags" />
		<add Name="postAuthor" />
		<!-- Optionally, your fields here -->
	</IndexUserFields>
	<IncludeNodeTypes>
		<add Name="blogPost"/>
	</IncludeNodeTypes>
</IndexSet>

```
## Doctype

Add a new DocType with an alias blogPost with the following properties along with any custom properties:

settingsNewsTags - Tag Picker

postDate - Date Picker

postAuthor - User Picker

# Usage


