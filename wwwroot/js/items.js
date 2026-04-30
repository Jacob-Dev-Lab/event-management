document.addEventListener("DOMContentLoaded", () => {

	const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

	// ---------------- CREATE ----------------
	document.getElementById("createItemForm")?.addEventListener("submit", async e => {
		e.preventDefault();

		const form = e.target;
		const res = await fetch("/Items/Create", {
			method: "POST",
			headers: { "RequestVerificationToken": token },
			body: new FormData(form)
		});

		const data = await res.json();
		clearErrors();

		if (!data.success) return showErrors(data.errors);

		addRow(data.data);
		form.reset();
		bootstrap.Modal.getInstance(document.getElementById("createItemModal")).hide();
	});

	// ---------------- EDIT CLICK ----------------
	document.addEventListener("click", async e => {
		if (!e.target.classList.contains("edit-btn")) return;

		const id = e.target.dataset.id;

		const res = await fetch(`/Items/Get/${id}`);
		const data = await res.json();

		if (!data.success) return;

		fillEditForm(data.data);
		new bootstrap.Modal(document.getElementById("editItemModal")).show();
	});

	// ---------------- UPDATE ----------------
	document.getElementById("editItemForm")?.addEventListener("submit", async e => {
		e.preventDefault();

		const form = e.target;
		const id = document.getElementById("EditId").value;

		const res = await fetch(`/Items/Update/${id}`, {
			method: "POST",
			headers: { "RequestVerificationToken": token },
			body: new FormData(form)
		});

		const data = await res.json();
		clearErrors();

		if (!data.success) return showErrors(data.errors);

		updateRow(data.data);
		bootstrap.Modal.getInstance(document.getElementById("editItemModal")).hide();
	});

	// ---------------- DELETE ----------------
	document.addEventListener("click", async e => {
		if (!e.target.classList.contains("delete-btn")) return;

		const id = e.target.dataset.id;

		if (!confirm("Delete this item?")) return;

		const res = await fetch(`/Items/Delete/${id}`, {
			method: "POST",
			headers: { "RequestVerificationToken": token }
		});

		const data = await res.json();

		if (data.success) {
			document.querySelector(`tr[data-id="${id}"]`).remove();
		}
	});

	// ---------------- HELPERS ----------------

	function addRow(item) {
		const row = buildRow(item);
		document.getElementById("itemsTableBody").insertAdjacentHTML("beforeend", row);
	}

	function updateRow(item) {
		const row = document.querySelector(`tr[data-id="${item.id}"]`);
		row.outerHTML = buildRow(item);
	}

	function buildRow(item) {
		return `
		<tr data-id="${item.id}">
			<td>${item.imagePath ? `<img src="${item.imagePath}" width="40" heigth="25"/>` : ''}</td>
			<td>${item.category}</td>
			<td>${item.name}</td>
			<td>${item.description}</td>
			<td>${item.formattedPrice}</td>
			<td>
				<button class="btn btn-sm btn-primary edit-btn" data-id="${item.id}">Edit</button>
				<button class="btn btn-sm btn-danger delete-btn" data-id="${item.id}">Delete</button>
			</td>
		</tr>`;
	}

	function fillEditForm(item) {
		document.getElementById("EditId").value = item.id;
		document.getElementById("EditImageFile").value = item.imagePath;
		document.getElementById("EditName").value = item.name;
		document.getElementById("EditCategory").value = item.category;
		document.getElementById("EditDescription").value = item.description;
		document.getElementById("EditPrice").value = item.price;
	}

	function showErrors(errors) {
		for (let key in errors) {
			const el = document.querySelector(`[data-valmsg-for="${key}"]`);
			if (el) el.textContent = errors[key][0];
		}
	}

	function clearErrors() {
		document.querySelectorAll("[data-valmsg-for]").forEach(e => e.textContent = "");
	}
});