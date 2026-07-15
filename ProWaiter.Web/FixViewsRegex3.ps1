$dir = 'N:\src\particular\ProWaiterV4\ProWaiter.Web\Views'
$files = Get-ChildItem -Path $dir -Recurse -Filter '*.cshtml'

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw

    $content = $content -replace '<script src="~/bundles/([^"]+)"></script>', '<script src="~/js/$1.js"></script>'
    $content = $content -replace '<link rel="stylesheet" href="~/Content/([^"]+)" />', '<link rel="stylesheet" href="~/css/$1.css" />'
    
    # MsgResultadoValidacao namespace fix
    $content = $content -replace 'ProWaiter\.Web\.MsgResultadoValidacao', 'ProWaiter.Validador.MsgResultadoValidacao'

    # _botoesAcaoDetalhesPedido needs ViewDataDictionary
    $content = $content -replace 'new ViewDataDictionary \{ \{ "id", Model\.Codigo \}', 'new ViewDataDictionary(ViewData) { { "id", Model.Codigo }'
    $content = $content -replace 'new ViewDataDictionary \{ \{ "id", Model\.Codigo \}, \{ "imprimirPedidoExterno", imprimirLanchesPedidoExterno \} \}', 'new ViewDataDictionary(ViewData) { { "id", Model.Codigo }, { "imprimirPedidoExterno", imprimirLanchesPedidoExterno } }'

    Set-Content -Path $file.FullName -Value $content -Encoding UTF8
}
