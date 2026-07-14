$files = Get-ChildItem -Path "N:\src\particular\ProWaiterV4\ProWaiter.Web\Controllers", "N:\src\particular\ProWaiterV4\ProWaiter.Web\APIs" -Filter "*.cs" -Recurse
foreach ($file in $files) {
    $content = [System.IO.File]::ReadAllText($file.FullName)
    
    # MVC Namespaces
    $content = $content -replace 'using System\.Web\.Mvc;', "using Microsoft.AspNetCore.Mvc;`r`nusing Microsoft.AspNetCore.Mvc.Rendering;`r`nusing Microsoft.AspNetCore.Authorization;"
    $content = $content -replace 'using System\.Web\.Http;', "using Microsoft.AspNetCore.Mvc;`r`nusing Microsoft.AspNetCore.Authorization;"
    
    # Identity Namespaces
    $content = $content -replace 'using Microsoft\.AspNet\.Identity;', "using Microsoft.AspNetCore.Identity;"
    $content = $content -replace 'using Microsoft\.AspNet\.Identity\.Owin;', "using Microsoft.AspNetCore.Identity;"
    
    # Controllers
    $content = $content -replace '(\s+)ApiController', '$1ControllerBase'
    
    # ActionResult responses
    $content = $content -replace 'new HttpStatusCodeResult\(HttpStatusCode\.BadRequest\)', 'BadRequest()'
    $content = $content -replace 'HttpNotFound\(\)', 'NotFound()'
    
    # [Bind(Include="...")] -> [Bind("...")]
    $content = $content -replace '\[Bind\(Include\s*=\s*(.+?)\)\]', '[Bind($1)]'
    
    # Replace AllowGet in JsonResult
    $content = $content -replace 'JsonRequestBehavior\.AllowGet', ''
    $content = $content -replace ',\s*JsonRequestBehavior\.AllowGet', ''
    
    [System.IO.File]::WriteAllText($file.FullName, $content)
}

# Fix Models/IdentityModels.cs references if any
$filesModels = Get-ChildItem -Path "N:\src\particular\ProWaiterV4\ProWaiter.Web\Models" -Filter "*.cs" -Recurse
foreach ($file in $filesModels) {
    $content = [System.IO.File]::ReadAllText($file.FullName)
    $content = $content -replace 'using System\.Web\.Mvc;', "using Microsoft.AspNetCore.Mvc;`r`nusing Microsoft.AspNetCore.Mvc.Rendering;"
    $content = $content -replace 'using Microsoft\.AspNet\.Identity;', "using Microsoft.AspNetCore.Identity;"
    [System.IO.File]::WriteAllText($file.FullName, $content)
}

