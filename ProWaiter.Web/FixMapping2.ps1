$files = Get-ChildItem -Path "N:\src\particular\ProWaiterV4\ProWaiter.Web\Models\Mapeamento" -Filter "*.cs"
foreach ($f in $files) {
    $c = [System.IO.File]::ReadAllText($f.FullName)
    
    $c = $c -replace 'builder\.builder\.', 'builder.'
    $c = $c -replace '\.IsOptional\(\)', '.IsRequired(false)'
    $c = $c -replace '\.WithRequired\(\)', '.WithOne().IsRequired()'
    $c = $c -replace '\.WithRequired\((.*?)\)', '.WithOne($1).IsRequired()'
    $c = $c -replace '\.WithOptional\(\)', '.WithOne()'
    $c = $c -replace '\.WithOptional\((.*?)\)', '.WithOne($1)'
    
    # Fix Ignore being called on KeyBuilder
    $c = $c -replace '\.Ignore\((.*?)\)', ';' + "`r`n            " + 'builder.Ignore($1)'
    
    [System.IO.File]::WriteAllText($f.FullName, $c)
}

# Fix ApplicationUser.Roles issue
$auc = "N:\src\particular\ProWaiterV4\ProWaiter.Web\Models\Mapeamento\ApplicationUserConfiguration.cs"
if (Test-Path $auc) {
    $aucContent = [System.IO.File]::ReadAllText($auc)
    $aucContent = $aucContent -replace '(?s)builder\.HasMany\(u => u\.Roles\).*?;', '// builder.HasMany(u => u.Roles)...'
    [System.IO.File]::WriteAllText($auc, $aucContent)
}

# Fix IdentityRole.Users issue
$irc = "N:\src\particular\ProWaiterV4\ProWaiter.Web\Models\Mapeamento\IdentityRoleConfiguration.cs"
if (Test-Path $irc) {
    $ircContent = [System.IO.File]::ReadAllText($irc)
    $ircContent = $ircContent -replace '(?s)builder\.HasMany\(r => r\.Users\).*?;', '// builder.HasMany(r => r.Users)...'
    [System.IO.File]::WriteAllText($irc, $ircContent)
}

# Fix RestauranteController
$rc = "N:\src\particular\ProWaiterV4\ProWaiter.Web\Controllers\RestauranteController.cs"
if (Test-Path $rc) {
    $rcContent = [System.IO.File]::ReadAllText($rc)
    $rcContent = $rcContent -replace 'Startup\.', '// Startup.'
    [System.IO.File]::WriteAllText($rc, $rcContent)
}