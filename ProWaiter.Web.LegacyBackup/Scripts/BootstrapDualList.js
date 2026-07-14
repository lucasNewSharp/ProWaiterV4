/* http://bootsnipp.com/index.php/snippets/featured/bootstrap-dual-list */

$(function () {

    $('body').on('click', '.list-group .list-group-item', function () {
        $(this).toggleClass('active');
    });
    $('.list-arrows button').click(function () {
        var $button = $(this), actives = '';
        if ($button.hasClass('move-left')) {
            actives = $('.list-right ul li.active');
            actives.clone().appendTo('.list-left ul');
            actives.remove();
        } else if ($button.hasClass('move-right')) {
            actives = $('.list-left ul li.active');
            actives.clone().appendTo('.list-right ul');
            actives.remove();
        }
    });
    $('.dual-list .selector').click(function () {
        var $checkBox = $(this);
        if (!$checkBox.hasClass('selected')) {
            $checkBox.addClass('selected').closest('.well').find('ul li:not(.active)').addClass('active');
            $checkBox.children('i').removeClass('glyphicon-unchecked').addClass('glyphicon-check');
        } else {
            $checkBox.removeClass('selected').closest('.well').find('ul li.active').removeClass('active');
            $checkBox.children('i').removeClass('glyphicon-check').addClass('glyphicon-unchecked');
        }
    });
    $('[name="SearchDualList"]').keyup(function (e) {
        var code = e.keyCode || e.which;
        if (code == '9') return;
        if (code == '27') $(this).val(null);
        FiltrarLista($(this));
    });

    function FiltrarLista(obj) {
        var $rows = obj.closest('.dual-list').find('.list-group li');
        var val = $.trim(obj.val()).replace(/ +/g, ' ').toLowerCase();
        $rows.show().filter(function () {
            var text = $(this).text().replace(/\s+/g, ' ').toLowerCase();
            return !~text.indexOf(val);
        }).hide();
    }

    $('[name="SearchDualList"]').focus(function (e) {
        $(this).val(null);
        FiltrarLista($(this));
    });
    $(document).submit(function (event) {
        var lista = $('.list-right ul li');
        var itensSelecionados = $('[name="ItensSelecionados"]');
        lista.each(function (i, li) {
            var id = $(this).find("div");
            itensSelecionados.val(itensSelecionados.val() + ',' + id.text());
        });

        var valor = itensSelecionados.val();
        if (valor.length > 0) itensSelecionados.val(valor.substr(1, valor.length));
    });
});

function AdicionarNovoItemAListaDaDireita(chave, valor) {
    var listaDireita = $('.list-right ul');
    listaDireita.append("<li class='list-group-item active'>" + valor + "<div class='hidden'>" + chave + "</div></li >");
}
