
$(document).ready(function () {

    //Sobrescrtia dos metodos de validação de Range e formato de número
    $.validator.methods.range = function (value, element, param) {
        var globalizedValue = value.replace(".", "");
        globalizedValue = globalizedValue.replace(",", ".");
        return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
    };

    $.validator.addMethod("number", function (value, element) {
        return this.optional(element) || /^(?:-?\d+|-?\d{1,3}(?:.\d{3})+)?(?:\,\d+)?$/.test(value);
    });
});