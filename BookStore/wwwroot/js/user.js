var DataTbl; 

$(document).ready(function () {
    loadDataTableUsers();
});

function loadDataTableUsers() {
    DataTbl = $('#tblData').DataTable({
        "ajax": {
            "url": '/admin/user/getall',
            "type": "GET",
            "dataSrc": "data" // Specify the data source if the data is nested
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company", "width": "15%" },
            { "data": "role", "width": "15%" },

            {
                "data": { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    console.log(data);
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        return `
                            <div class="d-flex justify-content-center">
                                <a href="#" class="btn btn-danger text-white rounded-3 me-2" style="width: 100px;" onclick=LockUnlock('${data.id}')>
                                    <i class="bi bi-lock-fill"></i> Lock 
                                </a>
                                <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white rounded-3" style="width: 150px;">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                        `
                    }
                    else {
                        return `
                            <div class="d-flex justify-content-center">
                                <a href="#" class="btn btn-success text-white rounded-3 me-2" style="width: 100px;" onclick=LockUnlock('${data.id}')>
                                    <i class="bi bi-unlock-fill"></i> Unlock 
                                </a>
                                <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white rounded-3" style="width: 150px;">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                        `
                    }
                    
                },
                "width": "25%"
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id), // Send the ID in an object
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message, "Success!", { positionClass: 'toast-bottom-right' });
                DataTbl.ajax.reload(); // Reload the DataTable
            } else {
                toastr.error(data.message, "Error!", { positionClass: 'toast-bottom-right' }); // Display error message
            }
        },
        error: function (xhr, status, error) {
            toastr.error("An error occurred: " + error, "Error!", { positionClass: 'toast-bottom-right' }); // Handle any errors
        }
    });
}
