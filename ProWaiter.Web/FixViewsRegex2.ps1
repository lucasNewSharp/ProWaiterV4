$dir = 'N:\src\particular\ProWaiterV4\ProWaiter.Web\Views'
$files = Get-ChildItem -Path $dir -Recurse -Filter '*.cshtml'

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw

    # Scripts.Render replacement
    $content = [regex]::Replace($content, '@Scripts\.Render\("([^"]+)"\)', '<script src="$1"></script>')

    # Request.IsAuthenticated to User.Identity.IsAuthenticated
    $content = $content -replace 'Request\.IsAuthenticated', 'User.Identity.IsAuthenticated'
    
    # User.Identity.GetUserName() to User.Identity.Name
    $content = $content -replace 'User\.Identity\.GetUserName\(\)', 'User.Identity.Name'

    # Startup.Version or whatever to a hardcoded string or remove it
    $content = $content -replace 'Startup\.', 'ProWaiter.Web.'

    # Fix Html.BeginForm missing antiforgery arg
    $content = [regex]::Replace($content, 'Html\.BeginForm\("([^"]+)", "([^"]+)", new \{ returnUrl = ViewBag\.ReturnUrl \}, FormMethod\.Post, htmlAttributes: new \{ @id\s*=\s*"([^"]+)" \}\)', 'Html.BeginForm("$1", "$2", new { returnUrl = ViewBag.ReturnUrl }, FormMethod.Post, true, new { id = "$3" })')

    # Fix ViewDataDictionary
    $content = $content -replace 'new ViewDataDictionary\(\)', 'new ViewDataDictionary(ViewData)'

    Set-Content -Path $file.FullName -Value $content -Encoding UTF8
}
