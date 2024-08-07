let currPage = 1;
let companyPageSize = 3;
let TotalPages = 0;
let TotalCompanies = 0;

$(document).ready(function () {
    loadCompanyCards(currPage);

    $('#searchCompany').on('input', function () {
        currPage = 1;
        loadCompanyCards(currPage);
    });
});

function loadCompanyCards(page) {
    const searchCompany = $('#searchCompany').val();
    console.log("Searching for:", searchCompany); // Debugging step
    $.ajax({
        url: `/admin/company/getall?page=${page}&pageSize=${companyPageSize}&query=${encodeURIComponent(searchCompany)}`,
        method: "GET",
        success: function (response) {
            console.log("Response data:", response); // Debugging step
            populateCompanyCards(response.data);
            TotalCompanies = response.totalCompanies;
            TotalPages = response.totalPages;
            updatePaginationCompanyControls(page);
            updateCompanyPageInfo(page);
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", status, error); // Debugging step
        }
    });
}

function populateCompanyCards(data) {
    var companyContainer = $('#companyContainer');
    companyContainer.empty();

    data.forEach(function (company) {
        var companyCard = `
                    <div class="col-md-4 mb-3">
                        <div class="card card-content border-secondary shadow-sm">
                            <div class="card-header">Company</div>
                            <div class="card-body mb-2">
                                <h4 class="card-title">${company.name}</h4>
                                <p class="card-text"><b>Street Address: </b>${company.streetAddress}</p>
                                <p class="card-text"><b>State: </b>${company.state}</p>
                                <p class="card-text"><b>City: </b>${company.city}</p>
                                <p class="card-text"><b>Postal Code: </b>${company.postalCode}</p>
                                <p class="card-text"><b>Phone Number: </b>${company.phoneNumber}</p>
                                <div class="d-flex justify-content-between">
                                    <a href="/admin/company/upsert/${company.id}" class="btn bg-gradient btn-primary"><i class="bi bi-pencil-square"></i> Edit</a>
                                    <button onClick="DeleteCompany('/admin/company/delete/${company.id}')" class="btn bg-gradient btn-outline-danger">
                                        <i class="bi bi-trash-circle"></i> Delete
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>`;

        companyContainer.append(companyCard);
    });
}

function updatePaginationCompanyControls(page) {
    const paginationList = $('#paginationCompanyNumbers');
    paginationList.empty();

    // Add Previous button
    paginationList.append(createPageButton('&laquo;', page - 1, page <= 1));

    // Add page number buttons
    for (let i = 1; i <= TotalPages; i++) {
        const pageButton = $(`<li class="page-item${i === page ? ' active' : ''}"><a class="page-link" href="#">${i}</a></li>`);
        pageButton.on('click', function () {
            if (i !== page) {
                currPage = i;
                loadCompanyCards(currPage);
            }
        });
        paginationList.append(pageButton);
    }

    // Add Next button
    paginationList.append(createPageButton('&raquo;', page + 1, page >= TotalPages));
}

function createPageButton(text, targetPage, isDisabled) {
    const button = $(`<li class="page-item${isDisabled ? ' disabled' : ''}"><a class="page-link" href="#">${text}</a></li>`);
    button.on('click', function () {
        if (!isDisabled) {
            currPage = targetPage;
            loadCompanyCards(currPage);
        }
    });
    return button;
}

function updateCompanyPageInfo(page) {
    if (TotalCompanies > 0) {
        const startIndex = (page - 1) * companyPageSize + 1;
        const endIndex = Math.min(page * companyPageSize, TotalCompanies);

        $('#startIndex').text(startIndex);
        $('#endIndex').text(endIndex);
        $('#totalCompanies').text(TotalCompanies);
    } else {
        $('#startIndex').text(0);
        $('#endIndex').text(0);
        $('#totalCompanies').text(0);
    }
}

function DeleteCompany(url) {
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
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Deleted!",
                            text: response.message,
                            icon: "success"
                        });
                        loadCompanyCards(currPage);
                    } else {
                        Swal.fire({
                            title: "Error!",
                            text: response.message,
                            icon: "error"
                        });
                    }
                }
            });
        }
    });
}