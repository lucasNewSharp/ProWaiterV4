jQuery.expr[":"].containsCI = jQuery.expr.createPseudo(function (arg) {
    return function (elem) {
        return jQuery(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0;
    };
});

function PesquisarNaTabela(idTabela, filtro) {
    var linhas = $('#' + idTabela + " tr td").parent("tr");
    linhas.each(function () {
        var linhaComFiltro = $(this).children("td:containsCI('" + filtro + "')");
        if (linhaComFiltro.length == 0) {
            $(this).hide();
        }
        else {
            $(this).show();
        }
    });
}

function PesquisarNaSelectList(idLista, filtro) {
    
    var lista = $('select#' + idLista + ' option')
    filtro = filtro.toUpperCase();
    lista.each(function (i) {        
        var exibir = $(this).text().toUpperCase().indexOf(filtro) >= 0;
        if (exibir) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });    
}


/*
Antes da tabela:
@Html.Partial("_pesquisarnaTabelaClient")

Chamada no cshtml
<script src="~/Scripts/PesquisarNaTabela.js"></script>
 <script type="text/javascript">
    $(document).ready(function () {
        var txt = $("#txtFiltro");
        txt.focus();
        txt.keyup(function () { PesquisarNaTabela('tabela', $(this).val()) });
    });
</script>
*/