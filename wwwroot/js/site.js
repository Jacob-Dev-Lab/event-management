$(window).on("scroll", function () {
    $(".fade-in").each(function () {
        if ($(this).offset().top < $(window).scrollTop() + window.innerHeight - 50) {
            $(this).addClass("show");
        }
    });
});
