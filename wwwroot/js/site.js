$(window).on("scroll", function () {
    $(".fade-in").each(function () {
        if ($(this).offset().top < $(window).scrollTop() + window.innerHeight - 50) {
            $(this).addClass("show");
        }
    });
});


document.addEventListener("click", function (e) {
    const btn = e.target.closest(".filter-btn");
    if (!btn) return;

    var category = btn.dataset.category;

    // Find currently active button
    const currentActive = document.querySelector(".filter-btn.active");

    // Remove active class from previous button
    if (currentActive) {
        currentActive.classList.remove("active");
    }

    // Add active class to clicked button
    btn.classList.add("active");

    // Remove focus from clicked button
    btn.blur();

    $.ajax({
        url: '/Services/Filter',
        type: 'GET',
        data: { category: category },
        success: function (result) {
            $("#content").html(result);
            $("#content").css("opacity", "1");

            // Smooth scroll to catalogue
            $('html, body').animate({
                scrollTop: $("#service-container").offset().top - 100
            }, 300);
        }
    });
})