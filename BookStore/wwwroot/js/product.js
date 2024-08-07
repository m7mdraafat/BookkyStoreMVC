let currentPage = 1;
const pageSize = 3;
let totalPages = 1;
let totalProducts = 0;

$(document).ready(function () {
    loadProductCards(currentPage);

    $('#searchInput').on('input', function () {
        currentPage = 1; // Reset to the first page on new search
        loadProductCards(currentPage);
    });

});

function loadProductCards(page) {
    const searchQuery = $('#searchInput').val(); // Get the search query
    $.ajax({
        url: `/admin/product/getall?page=${page}&pageSize=${pageSize}&search=${searchQuery}`,
        method: 'GET',
        success: function (data) {
            populateProductCards(data.data);
            totalPages = data.totalPages;
            totalProducts = data.totalProducts;
            updateProductPageInfo(currentPage);
            updatePaginationControls(page);
        },
        error: function (xhr, status, error) {
            console.error("Error loading products:", error);
        }
    });
}

function populateProductCards(products) {
    var productContainer = $('#productContainer');
    productContainer.empty();

    products.forEach(function (product) {
        var productCard = `
            <div class="col-md-4 mb-3">
                <div class="card card-content border-secondary shadow-sm">
                  <div class="card-header">Product</div>
                      <div class="card-body mb-2">
                        <h4 class="card-title">${product.title}</h4>
                        <p class="card-text"><b>ISBN: </b>${product.isbn}</p>
                        <p class="card-text"><b>Author: </b>${product.author}</p>
                        <p class="card-text"><b>Price: </b>$${product.price}</p>
                        <p class="card-text"><b>Category:</b> ${product.category.name}</p>
                        <div class = "d-flex justify-content-between">
                            <a href="/admin/product/upsert/${product.id}" class="btn bg-gradient btn-primary"> <i class="bi bi-pencil-square"></i> Edit</a>
                            <button onClick="Delete('/admin/product/delete/${product.id}')" class="btn bg-gradient btn-outline-danger">
                                            <i class="bi bi-trash-circle"></i> Delete
                            </button>
                      </div>
                </div>
            </div>`;

        productContainer.append(productCard);
    });
}

function updatePaginationControls(page) {
    const paginationList = $('#paginationNumbers');
    paginationList.empty();

    // Add Previous button
    paginationList.append(createPageButton('&laquo;', page - 1, page <= 1));

    // Add page number buttons
    for (let i = 1; i <= totalPages; i++) {
        const pageButton = $(`<li class="page-item${i === page ? ' active' : ''}"><a class="page-link" href="#">${i}</a></li>`);
        pageButton.on('click', function () {
            if (i !== page) {
                currentPage = i;
                loadProductCards(currentPage);
            }
        });
        paginationList.append(pageButton);
    }

    // Add Next button
    paginationList.append(createPageButton('&raquo;', page + 1, page >= totalPages));
}

function createPageButton(text, targetPage, isDisabled) {
    const button = $(`<li class="page-item${isDisabled ? ' disabled' : ''}"><a class="page-link" href="#">${text}</a></li>`);
    button.on('click', function () {
        if (!isDisabled) {
            currentPage = targetPage;
            loadProductCards(currentPage);
        }
    });
    return button;
}

function updateProductPageInfo(page) {
    if (totalProducts > 0) {
        const startIndex = (page - 1) * pageSize + 1;
        const endIndex = Math.min(page * pageSize, totalProducts);

        $('#currentStart').text(startIndex);
        $('#currentEnd').text(endIndex);
        $('#totalProducts').text(totalProducts);
    } else {
        $('#currentStart').text(0);
        $('#currentEnd').text(0);
        $('#totalProducts').text(0);
    }
}


function Delete(url) {
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
                        text: "Your product has been deleted.",
                        icon: "success"
                    });
                    loadProductCards(currentPage);
                } 
            });
        }
    });
}