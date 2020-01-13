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

toastr.options = {
    "positionClass": "toast-top-center",
    "timeOut": "3000"
};

$(".add-to-cart").on("click", function () {
    var id = $(this).attr("data-catalog-item-id");
    $.post("/cart/addToCart", { itemId: id })
        .done(function (data) {
            toastr.success('Book added to cart');
            var cart = $(".basket-status");
            cart.removeClass("is-disabled");
            var badge = cart.find(".basket-status-badge");
            badge.text(+badge.text() + 1);
        })
        .fail(function () {
            toastr.error("Error happened");
        });
});

$('.simple-pagination').each(function () {
    var totalPages = $(this).attr("data-total-pages");
    var currentPage = $(this).attr("data-current-page");
    $(this).pagination({
        pages: totalPages,
        currentPage: currentPage,
        cssStyle: 'light-theme',
        ellipsePageSet: false,
        hrefTextPrefix: '?page='
    });
});