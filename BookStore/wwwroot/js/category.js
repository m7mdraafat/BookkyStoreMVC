$(document).ready(function () {
    loadCategoryCards();
});

function loadCategoryCards() {
    $.ajax({
        url: "/admin/Category/GetAll",
        method: "GET",
        success: function (response) {
            populateCategoryCards(response.data);
        },
        error: function (xhr, status, error) {
            console.error("Error loading categories:", error);
        }
    });
}

function populateCategoryCards(categories) {
    var categoryContainer = $('#categoryContainer');
    categoryContainer.empty();

    categories.forEach(function (category) {
        var categoryCard = `
            <div class="col-md-4 mb-3">
                <div class="card bg-light">
                    <div class="card-header">Category</div>
                    <div class="card-body">
                        <h5 class="card-title text-dark">${category.name}</h5>
                        <p class="card-text">Display Order: ${category.displayOrder}</p>
                        <div class="d-flex justify-content-between">
                            <a href="/admin/Category/Edit/${category.id}" class="btn text-white btn-outline-primary btn-sm shadow-sm mb-0">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a href="/admin/Category/Delete/${category.id}" class="btn btn-outline-danger btn-sm mb-0">
                                <i class="bi bi-trash"></i> Delete
                            </a>
                        </div>
                    </div>
                </div>
            </div>`;

        categoryContainer.append(categoryCard);
    });
}
