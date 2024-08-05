let currentPage = 1;
const pageSize = 3;
let totalPages = 1;

$(document).ready(function () {
    loadProductCards(currentPage);
});

function loadProductCards(page) {
    $.ajax({
        url: `/admin/product/getall?page=${page}&pageSize=${pageSize}`,
        method: 'GET',
        success: function (data) {
            populateProductCards(data.data);
            totalPages = data.totalPages;
            updatePaginationControls(page);
        }
    });
}

function populateProductCards(products) {
    var productContainer = $('#productContainer');
    productContainer.empty();

    products.forEach(function (product) {
        var productCard = `
            <div class="col-md-4 mb-3">
                <div class="card bg-light">
                    <div class="card-header text-dark">Product</div>
                    <div class="card-body">
                        <h5 class="card-title text-dark">${product.title}</h5>
                        <p class="card-text">ISBN: ${product.isbn}</p>
                        <p class="card-text">Category: ${product.category.name}</p>
                        <p class="card-text">Author: ${product.author}</p>
                        <p class="card-text">Price: $${product.price}</p>
                        <div class="d-flex justify-content-between">
                            <a href="/admin/Product/Upsert/${product.id}" class="btn text-white btn-outline-primary btn-sm shadow-sm mb-0">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a href="/admin/Product/Delete/${product.id}" class="btn  btn-outline-danger mb-0"">
                                <i class="bi bi-trash"></i> Delete
                            </a>
                        </div>
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
