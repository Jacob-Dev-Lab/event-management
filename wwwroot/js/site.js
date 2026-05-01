$(window).on("scroll", function () {
    $(".fade-in").each(function () {
        if ($(this).offset().top < $(window).scrollTop() + window.innerHeight - 50) {
            $(this).addClass("show");
        }
    });
});

/** Filter services by category **/
$(document).on("click", ".filter-btn", function () {

    var category = $(this).data("category");

    // Active state toggle
    $(".filter-btn").removeClass("active");
    $(this).addClass("active");
    $("#service-container").css("opacity", "0.5");

    $.ajax({
        url: '/Services/Filter',
        type: 'GET',
        data: { category: category },
        success: function (result) {
            $("#service-container").html(result);
            $("#service-container").css("opacity", "1");

            // Smooth scroll to catalogue
            $('html, body').animate({
                scrollTop: $("#service-container").offset().top - 100
            }, 300);
        }
    });
});