// site.js

$(document).ready(function () {
    loadProductCards();
});

// load Product data 
function loadProductCards() {
    $.ajax({
        url: '/admin/product/getall',
        method: 'GET',
        success: function (data) {
            populateProductCards(data.data);
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
                            <a href="/admin/Product/Delete/${product.id}" class="btn btn-outline-danger btn-sm mb-0">
                                <i class="bi bi-trash"></i> Delete
                            </a>
                        </div>
                    </div>
                </div>
            </div>`;

        productContainer.append(productCard);
    });
}
