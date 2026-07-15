$dir = 'N:\src\particular\ProWaiterV4\ProWaiter.Web\Views'
$files = Get-ChildItem -Path $dir -Recurse -Filter '*.cshtml'

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw

    $content = $content -replace 'new ViewDataDictionary\(\)', 'new ViewDataDictionary(ViewData)'
    $content = $content -replace '@Url\.Encode\(Request\.RawUrl\)', '@System.Net.WebUtility.UrlEncode(Context.Request.Path + Context.Request.QueryString.ToString())'
    $content = $content -replace 'Request\.RawUrl', '(Context.Request.Path + Context.Request.QueryString.ToString())'
    
    $content = [regex]::Replace($content, '@Html\.Label\("([^"]+)", "([^"]+)", new \{ @class = "([^"]+)" \}\)', '<label class="$3">$2</label>')
    $content = [regex]::Replace($content, '@Html\.Label\("([^"]+)", htmlAttributes: new \{ @class = "([^"]+)" \}\)', '<label class="$2">$1</label>')

    $content = [regex]::Replace($content, '@Styles\.Render\("([^"]+)"\)', '<link rel="stylesheet" href="$1" />')

    $content = [regex]::Replace($content, 'Html\.BeginForm\("([^"]+)", "([^"]+)", null, FormMethod\.Post, htmlAttributes: new \{ @Id = "([^"]+)" \}', 'Html.BeginForm("$1", "$2", null, FormMethod.Post, true, new { id = "$3" }')
    $content = [regex]::Replace($content, 'Html\.BeginForm\(null, null, FormMethod\.Post, new \{ id = "([^"]+)" \}', 'Html.BeginForm(null, null, null, FormMethod.Post, true, new { id = "$1" }')

    Set-Content -Path $file.FullName -Value $content -Encoding UTF8
}
