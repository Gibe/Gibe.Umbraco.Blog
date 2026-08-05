#!/bin/bash
# Demo Site Setup Script
# Creates a local Umbraco site referencing this repo's Gibe.Umbraco.Blog project, installs
# uSync, and imports a set of demo blog doctypes/content so the package can be exercised
# without building any of that by hand.

set -e

# Determine repository root (parent of scripts folder)
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &>/dev/null && pwd )"
REPO_ROOT="$( cd "$SCRIPT_DIR/.." &>/dev/null && pwd )"

# Change to repository root to ensure consistent behavior
cd "$REPO_ROOT" || exit 1

# Parse arguments
SKIP_TEMPLATE_INSTALL=false
FORCE=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --skip-template-install|-s)
            SKIP_TEMPLATE_INSTALL=true
            shift
            ;;
        --force|-f)
            FORCE=true
            shift
            ;;
        --help|-h)
            echo "Usage: $0 [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  -s, --skip-template-install  Skip reinstalling Umbraco.Templates"
            echo "  -f, --force                  Recreate demo if it already exists"
            echo "  -h, --help                   Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

echo "========================================="
echo "Gibe.Umbraco.Blog Demo Site Setup"
echo "========================================="
echo "Working directory: $REPO_ROOT"
echo ""

# Read the Umbraco template version straight out of the library's csproj so this stays in
# lockstep with whatever Umbraco major this package currently targets.
CSPROJ_PATH="$REPO_ROOT/Gibe.Umbraco.Blog/Gibe.Umbraco.Blog.csproj"
if [ ! -f "$CSPROJ_PATH" ]; then
    echo "ERROR: Could not find $CSPROJ_PATH" >&2
    exit 1
fi
TEMPLATE_VERSION=$(grep -oE 'Umbraco\.Cms\.Web\.Website"\s+Version="[^"]+"' "$CSPROJ_PATH" | grep -oE '[0-9]+\.[0-9]+\.[0-9]+(-[A-Za-z0-9.]+)?')
if [ -z "$TEMPLATE_VERSION" ]; then
    echo "ERROR: Could not find the Umbraco.Cms.Web.Website version in $CSPROJ_PATH" >&2
    exit 1
fi
VERSION_MAJOR=$(echo "$TEMPLATE_VERSION" | cut -d. -f1)
IS_TEMPLATE_PRERELEASE=false
if echo "$TEMPLATE_VERSION" | grep -q '-'; then
    IS_TEMPLATE_PRERELEASE=true
fi
echo "Target Umbraco.Cms template version: $TEMPLATE_VERSION (v$VERSION_MAJOR)"
echo ""

DEMO_DIR="demo"
DEMO_SITE_NAME="Gibe.Umbraco.Blog.DemoSite"
DEMO_SITE_DIR="${DEMO_DIR}/${DEMO_SITE_NAME}"
SOLUTION_NAME="Gibe.Umbraco.Blog.local"
LIBRARY_PROJECT="Gibe.Umbraco.Blog/Gibe.Umbraco.Blog.csproj"
USYNC_SEED_DIR="scripts/uSync-seed"
USYNC_VERSION_FOLDER="v${VERSION_MAJOR}"

# Check if demo already exists
if [ -d "$DEMO_DIR" ] && [ "$FORCE" = false ]; then
    echo "Demo folder '$DEMO_DIR' already exists. Use --force to recreate."
    echo "Or open the existing ${SOLUTION_NAME}.slnx"
    exit 0
fi

# Clean up existing demo if Force
if [ "$FORCE" = true ] && [ -d "$DEMO_DIR" ]; then
    echo "Removing existing demo folder '$DEMO_DIR'..."
    rm -rf "$DEMO_DIR"
fi

if [ "$FORCE" = true ] && [ -f "${SOLUTION_NAME}.slnx" ]; then
    rm -f "${SOLUTION_NAME}.slnx"
fi

# Step 1: Install Umbraco templates
if [ "$SKIP_TEMPLATE_INSTALL" = false ]; then
    echo "Installing Umbraco templates ($TEMPLATE_VERSION)..."
    # Uninstall any existing version to avoid conflicts
    echo "Removing any existing Umbraco.Templates installations..."
    if dotnet new uninstall 2>&1 | grep -q "Umbraco\.Templates"; then
        dotnet new uninstall Umbraco.Templates 2>/dev/null || true
    fi
    if [ "$IS_TEMPLATE_PRERELEASE" = true ]; then
        # Prerelease templates require the umbracoprereleases MyGet feed to be configured.
        # If not yet configured: dotnet nuget add source https://www.myget.org/F/umbracoprereleases/api/v3/index.json --name UmbracoPreReleases
        echo "NOTE: Prerelease template ($TEMPLATE_VERSION) requires the umbracoprereleases MyGet source."
    fi
    dotnet new install "Umbraco.Templates::${TEMPLATE_VERSION}" --force
fi

# Step 2: Create the Umbraco demo site
echo "Creating demo folder '$DEMO_DIR'..."
mkdir -p "$DEMO_DIR"

echo "Creating Umbraco demo site..."
pushd "$DEMO_DIR" > /dev/null
dotnet new umbraco --force -n "$DEMO_SITE_NAME" --friendly-name "Administrator" --email "admin@example.com" --password "password1234" --development-database-type SQLite
popd > /dev/null

DEMO_PROJECT="${DEMO_SITE_DIR}/${DEMO_SITE_NAME}.csproj"

# Step 3: Add project reference to Gibe.Umbraco.Blog
echo "Adding project reference to Gibe.Umbraco.Blog..."
dotnet add "$DEMO_PROJECT" reference "$LIBRARY_PROJECT"

# Step 4: Install uSync so the demo content below can be imported/re-exported as disk files
echo "Installing uSync..."
USYNC_VERSION=$(curl -sf --max-time 10 "https://api.nuget.org/v3-flatcontainer/usync/index.json" 2>/dev/null | grep -oE '"[0-9]+\.[0-9]+\.[0-9]+(\.[0-9]+)?"' | tr -d '"' | grep -E "^${VERSION_MAJOR}\." | tail -1)
if [ -n "$USYNC_VERSION" ]; then
    dotnet add "$DEMO_PROJECT" package uSync --version "$USYNC_VERSION"
else
    echo "NOTE: Could not query nuget.org for the latest uSync version. Falling back to a floating version range."
    dotnet add "$DEMO_PROJECT" package uSync --version "${VERSION_MAJOR}.*"
fi

# Step 5: Add a concrete IBlogPostModel implementation and wire up AddGibeBlog<T>
echo "Adding a concrete BlogPost model..."
MODELS_DIR="${DEMO_SITE_DIR}/Models"
mkdir -p "$MODELS_DIR"
cat > "$MODELS_DIR/BlogPost.cs" <<'EOF'
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
EOF

echo "Wiring AddGibeBlog<BlogPost>() into Program.cs..."
PROGRAM_PATH="${DEMO_SITE_DIR}/Program.cs"
PROGRAM_CONTENT=$(cat "$PROGRAM_PATH")
# NOTE: ">(" is process-substitution syntax to bash's parser and gets misinterpreted even inside
# double quotes if it appears literally in the replacement text, so the ">" is kept in its own
# variable to avoid "AddGibeBlog<BlogPost>();" ever appearing as a literal ">(" in the script.
GT=">"
PROGRAM_CONTENT="${PROGRAM_CONTENT/WebApplicationBuilder builder = WebApplication.CreateBuilder(args);/using Gibe.Umbraco.Blog;
using Gibe.Umbraco.Blog.DemoSite.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddGibeBlog<BlogPost${GT}();}"
printf '%s\n' "$PROGRAM_CONTENT" > "$PROGRAM_PATH"

# Step 6: Copy the demo Controllers/Views/CSS (plain MVC routes for /, /blog and /blog/{id} -
# these render via IBlogService<T> directly rather than Umbraco content templates).
echo "Copying demo templates and CSS..."
DEMO_TEMPLATES_DIR="scripts/demo-templates"
mkdir -p "${DEMO_SITE_DIR}/Controllers"
cp -r "$DEMO_TEMPLATES_DIR"/Controllers/* "${DEMO_SITE_DIR}/Controllers"/
mkdir -p "${DEMO_SITE_DIR}/Views/Demo"
cp -r "$DEMO_TEMPLATES_DIR"/Views/Demo/* "${DEMO_SITE_DIR}/Views/Demo"/
mkdir -p "${DEMO_SITE_DIR}/wwwroot/css"
cp -r "$DEMO_TEMPLATES_DIR"/wwwroot/css/* "${DEMO_SITE_DIR}/wwwroot/css"/

# Step 7: Copy the checked-in uSync seed content (doctypes + demo blog posts) into the site,
# and ask uSync to import it on startup so the demo site boots with content already in place.
echo "Copying uSync seed content..."
SEED_SOURCE="${USYNC_SEED_DIR}/${USYNC_VERSION_FOLDER}"
if [ -d "$SEED_SOURCE" ]; then
    SEED_TARGET="${DEMO_SITE_DIR}/uSync/${USYNC_VERSION_FOLDER}"
    mkdir -p "$SEED_TARGET"
    cp -r "$SEED_SOURCE"/* "$SEED_TARGET"/
else
    echo "NOTE: No uSync seed folder found at $SEED_SOURCE for $USYNC_VERSION_FOLDER - skipping content seed."
fi

echo "Enabling uSync import at startup..."
DEV_SETTINGS_PATH="${DEMO_SITE_DIR}/appsettings.Development.json"
if command -v jq >/dev/null 2>&1; then
    jq '.uSync = { "Settings": { "ImportAtStartup": "All" } }' "$DEV_SETTINGS_PATH" > "${DEV_SETTINGS_PATH}.tmp"
    mv "${DEV_SETTINGS_PATH}.tmp" "$DEV_SETTINGS_PATH"
else
    # jq isn't guaranteed to be installed; fall back to a plain text insert. This relies on the
    # scaffolded appsettings.Development.json being a normal single top-level JSON object, which is
    # what `dotnet new umbraco` produces.
    echo "  (jq not found, falling back to a plain text insert)"
    CONTENT=$(cat "$DEV_SETTINGS_PATH")
    TRIMMED="${CONTENT%\}}"
    TRIMMED="${TRIMMED%"${TRIMMED##*[![:space:]]}"}"
    {
        printf '%s' "$TRIMMED"
        cat <<'JSON_EOF'
,
  "uSync": {
    "Settings": {
      "ImportAtStartup": "All"
    }
  }
}
JSON_EOF
    } > "${DEV_SETTINGS_PATH}.tmp"
    mv "${DEV_SETTINGS_PATH}.tmp" "$DEV_SETTINGS_PATH"
fi

# Step 8: Create unified solution
echo "Creating unified solution..."
dotnet new sln -n "$SOLUTION_NAME" --force
dotnet sln "${SOLUTION_NAME}.slnx" add "$LIBRARY_PROJECT" --solution-folder "Library"
dotnet sln "${SOLUTION_NAME}.slnx" add "$DEMO_PROJECT" --solution-folder "Demo"

echo ""
echo "========================================="
echo "Setup Complete!"
echo "========================================="
echo ""
echo "Solution: ${SOLUTION_NAME}.slnx"
echo "Demo site: $DEMO_SITE_DIR"
echo ""
echo "Credentials:"
echo "  Email: admin@example.com"
echo "  Password: password1234"
echo ""
echo "Next steps:"
echo "  1. Open ${SOLUTION_NAME}.slnx in your IDE, build, and run the $DEMO_SITE_NAME project."
echo "  2. uSync is set to import at startup, but if the doctypes/content don't appear,"
echo "     log into /umbraco and run the import manually from the uSync dashboard."
echo ""
