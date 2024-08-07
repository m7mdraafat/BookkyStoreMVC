let currentCategoryPage = 1;
const CategoryPageSize = 6;
let CategoryTotalPages = 1;
let totalCategories = 0; // Store the total number of categories

$(document).ready(function () {
    loadCategoryCards(currentCategoryPage);

    // Handle search input
    $('#searchInput').on('input', function () {
        currentCategoryPage = 1; // Reset to the first page on new search
        loadCategoryCards(currentCategoryPage);
    });
});

function loadCategoryCards(page) {
    const searchQuery = $('#searchInput').val();
    console.log("Loading page:", page); // Debugging line
    $.ajax({
        url: `/admin/Category/GetAll?page=${page}&pageSize=${CategoryPageSize}&search=${searchQuery}`,
        method: "GET",
        success: function (response) {
            console.log("Response received:", response); // Debugging line
            if (response && response.data && response.totalPages && response.totalCategories !== undefined) {
                populateCategoryCards(response.data);
                CategoryTotalPages = response.totalPages;
                totalCategories = response.totalCategories; // Update totalCategories
                updatePaginationCategoryControls(page);
                updatePageInfo(page);
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
                <div class="card card-content shadow-sm border-secondary">
                    <div class="card-header">Category</div>
                    <div class="card-body">
                        <h5 class="card-title text-dark">${category.name}</h5>
                        <p class="card-text">Display Order: ${category.displayOrder}</p>
                        <div class="d-flex justify-content-between">
                            <a href="/admin/Category/Edit/${category.id}" class="btn bg-gradient text-white btn-primary ">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <button onClick="deleteCategory('/admin/Category/Delete/${category.id}')" class="btn  btn-outline-danger">
                                <i class="bi bi-trash-circle"></i> Delete
                            </button>
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

function updatePageInfo(page) {
    // Check if totalCategories is valid
    if (totalCategories > 0) {
        const startIndex = (page - 1) * CategoryPageSize + 1;
        const endIndex = Math.min(page * CategoryPageSize, totalCategories);

        $('#currentCategoryStart').text(startIndex);
        $('#currentCategoryEnd').text(endIndex);
        $('#totalCategories').text(totalCategories);
    } else {
        // If no categories, set the default message
        $('#currentStart').text(0);
        $('#currentEnd').text(0);
        $('#totalCategories').text(0);
    }
}


function deleteCategory(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    Swal.fire({
                        title: "Deleted!",
                        text: "Your category has been deleted.",
                        icon: "success"
                    });
                    loadCategoryCards(currentCategoryPage);
                }
            });
        }
    });
}