// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.setTimeout(function () {

    // seleciona todos os elementos com class ".alert" e inicia uma animacao de desvanecimento
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        // apos a animacao, remove o elemento do dom
        $(this).remove();
    });

}, 3000)

//$('.date').mask('00/00/0000');
//$('.time').mask('00:00:00');
//$('.date_time').mask('00/00/0000 00:00:00');
//$('.cep').mask('00000-000');


//$(document).ready(function () { 
//    $('#Celular').mask('(00)00000-0000');
    //$('.phone_with_ddd').mask('(00) 0000-0000');
    //$('.phone_us').mask('(000) 000-0000');
    //$('.mixed').mask('AAA 000-S0S');
    //$('.cpf').mask('000.000.000-00', { reverse: true });
    //$('.cnpj').mask('00.000.000/0000-00', { reverse: true });
    //$('.money').mask('000.000.000.000.000,00', { reverse: true });
//});


