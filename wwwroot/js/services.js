document.addEventListener("click", async function (e) {

    if (!e.target.classList.contains("add-to-cart")) return;

    const button = e.target;
    const id = button.dataset.id;

    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const formData = new FormData();
    formData.append("id", id);
    formData.append("quantity", 1);

    const res = await fetch("/Items/AddToCart", {
        method: "POST",
        headers: {
            "RequestVerificationToken": token
        },
        body: formData
    });

    const data = await res.json();
    console.log("Cart count:", data.count);

    if (data.success) {
        updateCartCount(data.count);

        // UX feedback
        button.textContent = "Added!";
        button.classList.remove("btn-primary-custom");
        button.classList.add("btn-success");

        setTimeout(() => {
            button.textContent = "Add to Cart";
            button.classList.remove("btn-success");
            button.classList.add("btn-primary-custom");
        }, 1500);
    }
});


function updateCartCount(count) {
    const badge = document.getElementById("cart-count");
    if (badge) {
        badge.textContent = count;

        // animation
        badge.classList.add("animate-bounce");

        setTimeout(() => {
            badge.classList.remove("animate-bounce");
        }, 300);
    }
}


document.addEventListener("DOMContentLoaded", async () => {
    const res = await fetch("/Items/GetCount");
    const data = await res.json();

    if (data.success) {
        document.getElementById("cart-count").textContent = data.count;
    }
});