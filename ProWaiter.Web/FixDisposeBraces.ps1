$files = Get-ChildItem -Path "N:\src\particular\ProWaiterV4\ProWaiter.Web\APIs", "N:\src\particular\ProWaiterV4\ProWaiter.Web\Controllers" -Filter "*.cs" -Recurse
foreach ($f in $files) {
    $lines = [System.IO.File]::ReadAllLines($f.FullName)
    $modified = $false
    $inDispose = $false
    $braceCount = 0
    
    for ($i = 0; $i -lt $lines.Length; $i++) {
        if ($lines[$i] -match "// protected void Dispose\(bool disposing\)" -or $lines[$i] -match "// protected override void Dispose\(bool disposing\)") {
            $inDispose = $true
            $braceCount = 0
            $modified = $true
        }
        
        if ($inDispose) {
            if ($lines[$i] -match '\{') { $braceCount++ }
            if ($lines[$i] -match '\}') { $braceCount-- }
            
            if (-not ($lines[$i] -match "^//")) {
                $lines[$i] = "// " + $lines[$i]
            }
            
            if ($braceCount -eq 0 -and $lines[$i] -match '\}') {
                $inDispose = $false
            }
        }
    }
    
    if ($modified) {
        [System.IO.File]::WriteAllLines($f.FullName, $lines)
    }
}
