document.addEventListener("click", async function (e) {

    if (!e.target.classList.contains("add-to-cart")) return;

    const button = e.target;
    const id = button.dataset.id;

    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const formData = new FormData();
    formData.append("id", id);
    formData.append("quantity", 1);

    const res = await fetch("/Cart/AddToCart", {
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
    let badge = document.getElementById("cart-count");

    if (!badge) {
        const cartBtn = document.getElementById("cart-btn");

        badge = document.createElement("span");
        badge.id = "cart-count";
        badge.className = "cart-badge";

        cartBtn.appendChild(badge);
    }

    badge.textContent = count;

    badge.style.display = "inline-flex";

    badge.classList.add("animate-bounce");
    setTimeout(() => badge.classList.remove("animate-bounce"), 300);
}

document.addEventListener("DOMContentLoaded", async () => {
    const res = await fetch("/Cart/GetCount");
    const data = await res.json();

    if (data.success) {
        document.getElementById("cart-count").textContent = data.count;
    }
});

const drawer = document.getElementById("cart-drawer");
const overlay = document.getElementById("cart-overlay");
const cartBtn = document.getElementById("cart-btn");
const closeBtn = document.getElementById("close-cart");

// OPEN
if (cartBtn && drawer && overlay) {
    cartBtn.addEventListener("click", () => {
        drawer.classList.add("open");
        overlay.classList.add("show");
        loadCartDrawer();
    });
}

// CLOSE
if (closeBtn) closeBtn.addEventListener("click", closeCart);
if (overlay) overlay.addEventListener("click", closeCart);

function closeCart() {
    drawer?.classList.remove("open");
    overlay?.classList.remove("show");
}

// ESC support
document.addEventListener("keydown", (e) => {
    if (e.key === "Escape") closeCart();
});

// LOAD CART
async function loadCartDrawer() {
    try {
        const res = await fetch("/Cart/MiniCart");
        const data = await res.json();

        const container = document.getElementById("cart-items");
        const totalEl = document.getElementById("cart-total");

        container.innerHTML = "";

        if (!data.success || data.items.length === 0) {
            container.innerHTML = `<p class="text-muted">Your cart is empty</p>`;
            totalEl.textContent = "$0.00";
            return;
        }

        data.items.forEach(item => {
            container.innerHTML += `
            <div class="cart-item d-flex align-items-center justify-content-between">

                <!-- LEFT: IMAGE -->
                <img src="${item.image}" class="cart-img"/>

                <!-- MIDDLE: NAME + CONTROLS -->
                <div class="cart-info flex-grow-1">

                    <div class="cart-name">${item.name}</div>

                    <div class="cart-controls d-flex align-items-center justify-content-between">

                        <!-- QUANTITY GROUP -->
                        <div class="qty-group">
                            <button class="qty-btn" data-id="${item.id}" data-action="decrease">−</button>
                            <span class="qty-value">${item.quantity}</span>
                            <button class="qty-btn" data-id="${item.id}" data-action="increase">+</button>
                        </div>

                        <!-- PRICE -->
                        <span class="item-price">£${item.price}</span>

                        <!-- RIGHT: REMOVE -->
                        <button class="remove-btn" data-id="${item.id}">
                            <i class="fa fa-trash"></i>
                        </button>
                    </div>
                </div>

                

            </div>
            `;
        });

        totalEl.textContent = data.total;

    } catch (err) {
        console.error("Cart load failed:", err);
    }
}

document.addEventListener("click", async (e) => {

    const btn = e.target.closest(".qty-btn");
    if (!btn) return;

    const id = btn.dataset.id;
    const action = btn.dataset.action;

    const itemEl = btn.closest(".cart-item");

    const qtyEl = itemEl.querySelector(".qty-value");
    const itemTotalEl = itemEl.querySelector(".item-price");

    let currentQty = parseInt(qtyEl.textContent);

    // Prevent invalid values
    let newQty = action === "increase"
        ? currentQty + 1
        : Math.max(0, currentQty - 1);

    // Disable buttons ( to prevent spam clicks)
    btn.disabled = true;

    try {
        const res = await fetch("/Cart/UpdateQuantity", {
            method: "POST",
            body: new URLSearchParams({ id, quantity: newQty })
        });

        const data = await res.json();

        if (!data.success) return;

        // remove item
        if (data.quantity === 0) {
            itemEl.style.opacity = "0";
            setTimeout(() => itemEl.remove(), 200);
        } else {
            // Update item
            qtyEl.textContent = data.quantity;
            itemTotalEl.classList.add("fade");
            setTimeout(() => {
                itemTotalEl.textContent = `£${data.itemTotal}`;
                itemTotalEl.classList.remove("fade");
            }, 100);
        }

        // Update totals
        document.getElementById("cart-total").textContent =
            `£${data.cartTotal.toFixed(2)}`;

        console.log("Cart count:", data.count);

        updateCartCount(data.count);

        // If cart is empty
        if (data.count === 0) {
            document.getElementById("cart-items").innerHTML =
                `<p class="text-muted text-center mt-4">Your cart is empty</p>`;
        }

    } catch (err) {
        console.error(err);
    } finally {
        btn.disabled = false;
    }
});

document.addEventListener("click", async (e) => {
    const btn = e.target.closest(".remove-btn");
    if (!btn) return;

    const id = btn.dataset.id;

    const itemEl = btn.closest(".cart-item");
    const totalEl = document.getElementById("cart-total");
    var removeItem = document.getElementsByClassName("remove-btn");

    try {
        const res = await fetch("/Cart/RemoveFromCart", {
            method: "POST",
            body: new URLSearchParams({ id })
        });

        const data = await res.json();
        console.log(data);

        if (!data.success) return;

        itemEl.style.opacity = "0";
        setTimeout(() => itemEl.remove(), 200);

        updateCartCount(data.count);

        totalEl.textContent = data.total;

        if (data.count === 0) {
            document.getElementById("cart-items").innerHTML =
                `<p class="text-muted text-center mt-4">Your cart is empty</p>`;
        }

    } catch (err) {
        console.error("Remove failed:", err);
    }
});