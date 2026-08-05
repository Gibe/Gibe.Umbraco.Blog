# Demo Site Setup Script
# Creates a local Umbraco site referencing this repo's Gibe.Umbraco.Blog project, installs
# uSync, and imports a set of demo blog doctypes/content so the package can be exercised
# without building any of that by hand.

param(
    [switch]$SkipTemplateInstall,
    [switch]$Force
)

$ErrorActionPreference = "Stop"

# Determine repository root (parent of scripts folder)
$ScriptDir = $PSScriptRoot
$RepoRoot = (Resolve-Path (Split-Path -Parent $ScriptDir)).Path

# Change to repository root to ensure consistent behavior
Push-Location $RepoRoot

Write-Host "=== Gibe.Umbraco.Blog Demo Site Setup ===" -ForegroundColor Cyan
Write-Host "Working directory: $RepoRoot" -ForegroundColor Gray
Write-Host ""

# Read the Umbraco template version straight out of the library's csproj so this stays in
# lockstep with whatever Umbraco major this package currently targets.
$csprojPath = Join-Path $RepoRoot "Gibe.Umbraco.Blog\Gibe.Umbraco.Blog.csproj"
if (-not (Test-Path $csprojPath)) {
    Write-Host "ERROR: Could not find $csprojPath" -ForegroundColor Red
    exit 1
}
$csprojContent = Get-Content $csprojPath -Raw
if ($csprojContent -match 'Umbraco\.Cms\.Web\.Website"\s+Version="([^"]+)"') {
    $TemplateVersion = $matches[1]
} else {
    Write-Host "ERROR: Could not find the Umbraco.Cms.Web.Website version in $csprojPath" -ForegroundColor Red
    exit 1
}
$VersionMajor = [int]($TemplateVersion -split '\.')[0]
$IsTemplatePrerelease = $TemplateVersion -match '-'
Write-Host "Target Umbraco.Cms template version: $TemplateVersion (v$VersionMajor)" -ForegroundColor Gray
Write-Host ""

$DemoDir = "demo"
$DemoSiteName = "Gibe.Umbraco.Blog.DemoSite"
$DemoSiteDir = "$DemoDir\$DemoSiteName"
$SolutionName = "Gibe.Umbraco.Blog.local"
$LibraryProject = "Gibe.Umbraco.Blog\Gibe.Umbraco.Blog.csproj"
$USyncSeedDir = "scripts\uSync-seed"
$USyncVersionFolder = "v$VersionMajor"

# Check if demo already exists
if ((Test-Path $DemoDir) -and -not $Force) {
    Write-Host "Demo folder '$DemoDir' already exists. Use -Force to recreate." -ForegroundColor Yellow
    Write-Host "Or open the existing $SolutionName.slnx" -ForegroundColor Yellow
    Pop-Location
    return
}

# Clean up existing demo if Force
if ($Force -and (Test-Path $DemoDir)) {
    Write-Host "Removing existing demo folder '$DemoDir'..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force $DemoDir
}

if ($Force -and (Test-Path "$SolutionName.slnx")) {
    Remove-Item -Force "$SolutionName.slnx"
}

# Step 1: Install Umbraco templates
if (-not $SkipTemplateInstall) {
    Write-Host "Installing Umbraco templates ($TemplateVersion)..." -ForegroundColor Green

    # Uninstall any existing version to avoid conflicts
    Write-Host "Removing any existing Umbraco.Templates installations..." -ForegroundColor Gray
    $installedTemplates = dotnet new uninstall 2>&1 | Out-String
    if ($installedTemplates -match "Umbraco\.Templates") {
        try {
            dotnet new uninstall Umbraco.Templates 2>&1 | Out-Null
        } catch {
            # Ignore errors during uninstall
        }
    }

    if ($IsTemplatePrerelease) {
        # Prerelease templates require the umbracoprereleases MyGet feed to be configured.
        # If not yet configured: dotnet nuget add source https://www.myget.org/F/umbracoprereleases/api/v3/index.json --name UmbracoPreReleases
        Write-Host "NOTE: Prerelease template ($TemplateVersion) requires the umbracoprereleases MyGet source." -ForegroundColor Yellow
    }
    dotnet new install "Umbraco.Templates::$TemplateVersion" --force
}

# Step 2: Create the Umbraco demo site
Write-Host "Creating demo folder '$DemoDir'..." -ForegroundColor Green
New-Item -ItemType Directory -Path $DemoDir -Force | Out-Null

Write-Host "Creating Umbraco demo site..." -ForegroundColor Green
Push-Location $DemoDir
dotnet new umbraco --force -n $DemoSiteName --friendly-name "Administrator" --email "admin@example.com" --password "password1234" --development-database-type SQLite
Pop-Location

$demoProject = "$DemoSiteDir\$DemoSiteName.csproj"

# Step 3: Add project reference to Gibe.Umbraco.Blog
Write-Host "Adding project reference to Gibe.Umbraco.Blog..." -ForegroundColor Green
dotnet add $demoProject reference $LibraryProject

# Step 4: Install uSync so the demo content below can be imported/re-exported as disk files
Write-Host "Installing uSync..." -ForegroundColor Green
$USyncVersion = $null
try {
    $USyncVersions = (Invoke-RestMethod -Uri "https://api.nuget.org/v3-flatcontainer/usync/index.json" -TimeoutSec 10).versions
    $USyncVersion = $USyncVersions | Where-Object { $_ -match "^$VersionMajor\." -and $_ -notmatch '-' } | Select-Object -Last 1
} catch {
    Write-Host "NOTE: Could not query nuget.org for the latest uSync version ($($_.Exception.Message))." -ForegroundColor Yellow
}
if ($USyncVersion) {
    dotnet add $demoProject package uSync --version $USyncVersion
} else {
    Write-Host "Falling back to a floating version range for uSync." -ForegroundColor Yellow
    dotnet add $demoProject package uSync --version "$VersionMajor.*"
}

# Step 5: Add a concrete IBlogPostModel implementation and wire up AddGibeBlog<T>
Write-Host "Adding a concrete BlogPost model..." -ForegroundColor Green
$modelsDir = "$DemoSiteDir\Models"
New-Item -ItemType Directory -Path $modelsDir -Force | Out-Null
@"
using Gibe.Umbraco.Blog.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Gibe.Umbraco.Blog.DemoSite.Models
{
	public class BlogPost : BlogPostBase
	{
		public BlogPost(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback) { }

		public string BodyText => this.Value<string>("bodyText") ?? string.Empty;
	}
}
"@ | Out-File -FilePath "$modelsDir\BlogPost.cs" -Encoding utf8 -Force

Write-Host "Wiring AddGibeBlog<BlogPost>() into Program.cs..." -ForegroundColor Green
$programPath = "$DemoSiteDir\Program.cs"
$programContent = Get-Content $programPath -Raw
$programContent = $programContent -replace
    '(WebApplicationBuilder builder = WebApplication\.CreateBuilder\(args\);)',
    "using Gibe.Umbraco.Blog;`r`nusing Gibe.Umbraco.Blog.DemoSite.Models;`r`n`r`n`$1`r`n`r`nbuilder.Services.AddGibeBlog<BlogPost>();"
$programContent | Out-File -FilePath $programPath -Encoding utf8 -Force

# Step 6: Copy the demo Controllers/Views/CSS (plain MVC routes for /, /blog and /blog/{id} -
# these render via IBlogService<T> directly rather than Umbraco content templates).
Write-Host "Copying demo templates and CSS..." -ForegroundColor Green
$demoTemplatesDir = "scripts\demo-templates"
New-Item -ItemType Directory -Path "$DemoSiteDir\Controllers" -Force | Out-Null
Copy-Item -Path "$demoTemplatesDir\Controllers\*" -Destination "$DemoSiteDir\Controllers" -Recurse -Force
New-Item -ItemType Directory -Path "$DemoSiteDir\Views\Demo" -Force | Out-Null
Copy-Item -Path "$demoTemplatesDir\Views\Demo\*" -Destination "$DemoSiteDir\Views\Demo" -Recurse -Force
New-Item -ItemType Directory -Path "$DemoSiteDir\wwwroot\css" -Force | Out-Null
Copy-Item -Path "$demoTemplatesDir\wwwroot\css\*" -Destination "$DemoSiteDir\wwwroot\css" -Recurse -Force

# Step 7: Copy the checked-in uSync seed content (doctypes + demo blog posts) into the site,
# and ask uSync to import it on startup so the demo site boots with content already in place.
Write-Host "Copying uSync seed content..." -ForegroundColor Green
$seedSource = Join-Path $USyncSeedDir $USyncVersionFolder
if (Test-Path $seedSource) {
    $seedTarget = "$DemoSiteDir\uSync\$USyncVersionFolder"
    New-Item -ItemType Directory -Path $seedTarget -Force | Out-Null
    Copy-Item -Path "$seedSource\*" -Destination $seedTarget -Recurse -Force
} else {
    Write-Host "NOTE: No uSync seed folder found at $seedSource for $USyncVersionFolder - skipping content seed." -ForegroundColor Yellow
}

Write-Host "Enabling uSync import at startup..." -ForegroundColor Green
$devSettingsPath = "$DemoSiteDir\appsettings.Development.json"
$devSettings = Get-Content $devSettingsPath -Raw | ConvertFrom-Json
$uSyncSettings = [PSCustomObject]@{
    Settings = [PSCustomObject]@{
        ImportAtStartup = "All"
    }
}
$devSettings | Add-Member -NotePropertyName "uSync" -NotePropertyValue $uSyncSettings -Force
$devSettings | ConvertTo-Json -Depth 10 | Out-File -FilePath $devSettingsPath -Encoding utf8 -Force

# Step 8: Create unified solution
Write-Host "Creating unified solution..." -ForegroundColor Green
dotnet new sln -n $SolutionName --force
dotnet sln "$SolutionName.slnx" add $LibraryProject --solution-folder "Library"
dotnet sln "$SolutionName.slnx" add $demoProject --solution-folder "Demo"

Write-Host ""
Write-Host "=== Setup Complete! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Solution: $SolutionName.slnx" -ForegroundColor Cyan
Write-Host "Demo site: $DemoSiteDir" -ForegroundColor Cyan
Write-Host ""
Write-Host "Credentials:" -ForegroundColor Yellow
Write-Host "  Email: admin@example.com"
Write-Host "  Password: password1234"
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Open $SolutionName.slnx in your IDE, build, and run the $DemoSiteName project."
Write-Host "  2. uSync is set to import at startup, but if the doctypes/content don't appear,"
Write-Host "     log into /umbraco and run the import manually from the uSync dashboard."
Write-Host ""

# Restore original directory
Pop-Location
