let currentCategoryPage = 1;
const CategoryPageSize = 6;
let CategoryTotalPages = 1;

$(document).ready(function () {
    loadCategoryCards(currentCategoryPage);
});

function loadCategoryCards(page) {
    console.log("Loading page:", page); // Debugging line
    $.ajax({
        url: `/admin/Category/GetAll?page=${page}&pageSize=${CategoryPageSize}`,
        method: "GET",
        success: function (response) {
            console.log("Response received:", response); // Debugging line
            if (response && response.data && response.totalPages) {
                populateCategoryCards(response.data);
                CategoryTotalPages = response.totalPages;
                updatePaginationCategoryControls(page);
            } else {
                console.error("Invalid response format:", response);
            }
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

function updatePaginationCategoryControls(page) {
    const paginationCategoryList = $('#paginationCategoryNumbers');
    paginationCategoryList.empty();

    // Add Previous button
    paginationCategoryList.append(createCategoryPageButton('&laquo;', page - 1, page <= 1));

    // Add page number buttons
    for (let i = 1; i <= CategoryTotalPages; i++) {
        const pageButton = $(`<li class="page-item${i === page ? ' active' : ''}"><a class="page-link" href="#">${i}</a></li>`);
        pageButton.on('click', function () {
            if (i !== page) {
                currentCategoryPage = i;
                loadCategoryCards(currentCategoryPage);
            }
        });
        paginationCategoryList.append(pageButton);
    }

    // Add Next button
    paginationCategoryList.append(createCategoryPageButton('&raquo;', page + 1, page >= CategoryTotalPages));
}

function createCategoryPageButton(text, targetPage, isDisabled) {
    const button = $(`<li class="page-item${isDisabled ? ' disabled' : ''}"><a class="page-link" href="#">${text}</a></li>`);
    button.on('click', function () {
        if (!isDisabled) {
            currentCategoryPage = targetPage;
            loadCategoryCards(currentCategoryPage);
        }
    });
    return button;
}
