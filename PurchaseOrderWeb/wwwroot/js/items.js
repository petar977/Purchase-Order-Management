function loadItemsTable() {
    dataTable = $('#itemsTbl').DataTable({
        serverSide: true,
        order: [[0,'asc']],
        ajax: {
            url: "/Items/GetAllItems",
            type: "post",
            data: {
                selectedByCompany: selectedByCompany
            }
        },
        columns: [
            { data: "name" },
            { data: "unitPrice" },
            { data: "companyName" },
            {
                data: "id",
                render: function (data) {
                    return `<div class="btn-group">
                    <button type="button" onclick="getItemsEdit(${data})" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#edit-item-modal" title="Edit"><i class="bi bi-pencil-square"></i></button>
                    <button type='button' onclick="Delete('/Items/DeleteItem', ${data})" class="btn btn-danger mx-1" title="Delete"><i class="bi bi-trash-fill"></i></button>
                    </div>`
                },
                orderable: false,
            }
        ],
        language: {
            lengthMenu: "Showing _MENU_ records per page",
            infoFiltered: ''
        },
        columnDefs: [
            {
                "className": "align-middle", "targets": "_all",
            }
        ]
    })
    $("#filter-company").on("change", function () {
        dataTable.draw();
    })
}
function selectedByCompany() { return $('#filter-company').find(':selected').val()}
function getItemsEdit(id){
    $.ajax({
        url: '/Items/GetItemData',
        type: 'get',
        data: { id: id }
    }).done(function (result) {
        //if(result.)
        $('#edit-name').val(result.name);
        $('#edit-price').val(result.unitPrice);
        $('#edit-id').val(result.id);
    })
}
function editItem() {
    $.ajax({
        url: '/Items/PostItemData',
        type: 'post',
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        data: JSON.stringify({
            Id: $('#edit-id').val(),
            Name: $('#edit-name').val(),
            UnitPrice: $('#edit-price').val()
        })
    }).done(function (result) {
        if (result.success == true) {
            toastr.success(result.message)
            $('#itemsTbl').DataTable().ajax.reload();
            
        } else {
            toastr.error(result.message)
        }      
    })
}
function Delete(url, data) {
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
                data: {                   
                    id: data
                }
            }).done(function (data) {                   
                if (data.success == true) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                } else if (data.success == false) {
                    toastr.error(data.message);
                }
            }).fail(function (data) {
                toastr.error(data.message);
            })
        }
    })
}
function addItem(url, name, price, companyId) {
    $.ajax({
        url: url,
        type: 'post',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify({
            Name: name,
            UnitPrice: price,
            CompanyId: companyId
        })
    }).done(function (result) {
        if (result.success == true) {
            toastr.success(result.message)
            dataTable.ajax.reload();
           
        } else {
            toastr.error(result.message)
        }
    })
}