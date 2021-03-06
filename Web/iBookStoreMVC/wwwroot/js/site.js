﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
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
        hrefTextPrefix: '#',
        selectOnClick: false,
        ellipsePageSet: false,
        onPageClick: function (pageNumber, event) {
            $('.spinner-container').removeClass('d-none');
            var search = window.location.search;
            var queryParams = new URLSearchParams(search);
            queryParams.set('page', pageNumber);
            search = queryParams.toString();
            var url = location.origin + '?' + search;
            window.location.href = url;
        }
    });
});

$('#currencySelect').each(function () {
    var currency = $(this).attr("data-currency");
    $(this).val(currency);
});

$('.newsletter button').on("click", () => {
    var email = $('.newsletter input').val();
    if (!email)
        return;

    $.post("/userManagement/signUpNewsletter", { email: email })
        .done(function (data) {
            toastr.success('Email added to newsletter list');
        })
        .fail(function (error) {
            toastr.error(error.responseText);
        });
});