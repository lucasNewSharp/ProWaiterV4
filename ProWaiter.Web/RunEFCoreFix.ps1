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
        
        # HasRequired(x).WithMany(y) -> HasOne(x).WithMany(y).IsRequired()
        $content = $content -replace 'HasRequired\((.*?)\)\.WithMany\((.*?)\)', 'HasOne($1).WithMany($2).IsRequired()'
        $content = $content -replace 'HasRequired\((.*?)\)\.WithMany\(\)', 'HasOne($1).WithMany().IsRequired()'
        
        # HasRequired(x).WithOptional(y) -> HasOne(x).WithOne(y).IsRequired()
        $content = $content -replace 'HasRequired\((.*?)\)\.WithOptional\((.*?)\)', 'HasOne($1).WithOne($2).IsRequired()'
        $content = $content -replace 'HasRequired\((.*?)\)\.WithOptional\(\)', 'HasOne($1).WithOne().IsRequired()'
        
        # HasOptional(x).WithMany(y) -> HasOne(x).WithMany(y)
        $content = $content -replace 'HasOptional\((.*?)\)\.WithMany\((.*?)\)', 'HasOne($1).WithMany($2)'
        $content = $content -replace 'HasOptional\((.*?)\)\.WithMany\(\)', 'HasOne($1).WithMany()'
        
        # HasMany(x).WithRequired(y) -> HasMany(x).WithOne(y).IsRequired()
        $content = $content -replace 'HasMany\((.*?)\)\.WithRequired\((.*?)\)', 'HasMany($1).WithOne($2).IsRequired()'
        $content = $content -replace 'HasMany\((.*?)\)\.WithRequired\(\)', 'HasMany($1).WithOne().IsRequired()'
        
        # HasMany(x).WithOptional(y) -> HasMany(x).WithOne(y)
        $content = $content -replace 'HasMany\((.*?)\)\.WithOptional\((.*?)\)', 'HasMany($1).WithOne($2)'
        $content = $content -replace 'HasMany\((.*?)\)\.WithOptional\(\)', 'HasMany($1).WithOne()'

        # Standalone HasRequired(...) that has no With...
        $content = $content -replace 'HasRequired\((.*?)\)\.HasForeignKey', 'HasOne($1).WithMany().IsRequired().HasForeignKey'
        $content = $content -replace 'HasOptional\((.*?)\)\.HasForeignKey', 'HasOne($1).WithMany().HasForeignKey'
        
        # Leftover HasRequired/HasOptional without WithMany/WithOne
        $content = $content -replace 'HasRequired\((.*?)\)', 'HasOne($1).WithMany().IsRequired()'
        $content = $content -replace 'HasOptional\((.*?)\)', 'HasOne($1)'

        $content = $content -replace 'WillCascadeOnDelete\(false\)', 'OnDelete(DeleteBehavior.Restrict)'
        $content = $content -replace 'WillCascadeOnDelete\(true\)', 'OnDelete(DeleteBehavior.Cascade)'
        $content = $content -replace 'HasDatabaseGeneratedOption\(.+?Identity\)', 'ValueGeneratedOnAdd()'
        $content = $content -replace 'HasDatabaseGeneratedOption\(.+?None\)', 'ValueGeneratedNever()'
        $content = $content -replace 'HasDatabaseGeneratedOption\(.+?Computed\)', 'ValueGeneratedOnAddOrUpdate()'
        
        [System.IO.File]::WriteAllText($file.FullName, $content)
    }
}