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
