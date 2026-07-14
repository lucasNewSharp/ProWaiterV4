$files = Get-ChildItem -Path "N:\src\particular\ProWaiterV4\ProWaiter.Web\Models\Mapeamento" -Filter "*.cs"
foreach ($file in $files) {
    $content = [System.IO.File]::ReadAllText($file.FullName)
    
    $content = $content -replace 'using System\.Data\.Entity\.ModelConfiguration;', "using Microsoft.EntityFrameworkCore;`r`nusing Microsoft.EntityFrameworkCore.Metadata.Builders;"
    $content = $content -replace 'EntityTypeConfiguration<(.+?)>', 'IEntityTypeConfiguration<$1>'
    
    $className = $file.BaseName
    if ($content -match "IEntityTypeConfiguration<(.+?)>") {
        $typeName = $matches[1]
        $content = $content -replace "public $className\s*\(\)", "public void Configure(EntityTypeBuilder<$typeName> builder)"
        
        $content = $content -replace '(?m)^([ \t]*)(HasKey|Property|HasRequired|HasOptional|HasMany|ToTable|Ignore)\(', '$1builder.$2('
        
        $content = $content -replace 'HasRequired\(', 'HasOne('
        $content = $content -replace 'HasOptional\(', 'HasOne('
        $content = $content -replace 'WithRequiredPrincipal\(', 'WithOne('
        $content = $content -replace 'WithRequiredDependent\(', 'WithOne('
        $content = $content -replace 'WithOptionalDependent\(', 'WithOne('
        $content = $content -replace 'WillCascadeOnDelete\(false\)', 'OnDelete(DeleteBehavior.Restrict)'
        $content = $content -replace 'WillCascadeOnDelete\(true\)', 'OnDelete(DeleteBehavior.Cascade)'
        $content = $content -replace 'HasDatabaseGeneratedOption\(.+?Identity\)', 'ValueGeneratedOnAdd()'
        $content = $content -replace 'HasDatabaseGeneratedOption\(.+?None\)', 'ValueGeneratedNever()'
        $content = $content -replace 'HasDatabaseGeneratedOption\(.+?Computed\)', 'ValueGeneratedOnAddOrUpdate()'
        
        [System.IO.File]::WriteAllText($file.FullName, $content)
    }
}
