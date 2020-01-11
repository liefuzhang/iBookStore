// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(".book-rating").each(function () {
    var rating = $(this).attr("data-rating");
    $(this).starRating({
        readOnly: true,
        initialRating: rating,
        starSize: 18
    });
});

//$(".add-to-cart").on("click", )