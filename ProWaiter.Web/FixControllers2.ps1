$files = Get-ChildItem -Path "N:\src\particular\ProWaiterV4\ProWaiter.Web\APIs", "N:\src\particular\ProWaiterV4\ProWaiter.Web\Controllers" -Filter "*.cs" -Recurse
foreach ($f in $files) {
    $c = [System.IO.File]::ReadAllText($f.FullName)
    $modified = $false
    
    # Remove System.Data.Entity and add Microsoft.EntityFrameworkCore
    if ($c -match "using System\.Data\.Entity;") {
        $c = $c -replace "using System\.Data\.Entity;", "using Microsoft.EntityFrameworkCore;"
        $modified = $true
    }
    if ($c -match "using System\.Data\.Entity\.Infrastructure;") {
        $c = $c -replace "using System\.Data\.Entity\.Infrastructure;", ""
        $modified = $true
    }
    
    # Fix HttpStatusCode usages
    if ($c -match "HttpStatusCode\.") {
        $c = $c -replace "HttpStatusCode\.OK", "200"
        $c = $c -replace "HttpStatusCode\.Created", "201"
        $c = $c -replace "HttpStatusCode\.NoContent", "204"
        $c = $c -replace "HttpStatusCode\.BadRequest", "400"
        $c = $c -replace "HttpStatusCode\.NotFound", "404"
        $c = $c -replace "HttpStatusCode\.InternalServerError", "500"
        $modified = $true
    }
    
    # Remove old Dispose methods to avoid syntax errors
    if ($c -match "protected void Dispose\(bool disposing\)") {
        # regex to remove the method. It's tricky to balance braces, so let's just comment out the method signature and contents
        $c = $c -replace 'protected void Dispose\(bool disposing\)', '// protected void Dispose(bool disposing)'
        $c = $c -replace 'db\.Dispose\(\);', '// db.Dispose();'
        $modified = $true
    }
    
    # Also fix some Route/API specific issues
    if ($c -match "\[ResponseType\(typeof") {
        $c = $c -replace '\[ResponseType\((typeof\([^\)]+\))\)\]', '[ProducesResponseType($1, 200)]'
        $modified = $true
    }
    
    if ($modified) {
        [System.IO.File]::WriteAllText($f.FullName, $c)
    }
}
