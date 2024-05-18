function loadDataTable() {
    var dataItemsTable = $('#tblBody').DataTable({
        "serverSide": false,
        language: {
            lengthMenu: "Showing _MENU_ records per page",
            infoFiltered: ''
        },
        "ajax": {
            url: '/PurchaseOrderItems/GetItems',
            data: { id: $("#poId").val() },
        },
        "columns": [
            { data: 'itemsName', "width": "auto" },
            { data: 'qty', "width": "auto" },
            { data: 'unitPrice', "width": "auto", render: function (data) { return Math.round(data * 100) / 100 } },
            { data: 'total', "width": "auto", render: function (data) { return Math.round(data * 100) / 100 } },
            {
                data: 'link', "width": "10%",

                render: function (data) {
                    if (data == "https://") {
                        return ""
                    }
                    else if (data != null) {
                        ustart = data.replace('http://', '').replace('https://', '').substr(0, 26);
                        return `<a type="button" onclick="window.open('${data}', '_blank')" class="btn link-primary">${ustart}...</a>`
                    }
                    return ""
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="btn-group">
                    <button type="button" onclick="getItem(${data});" class="btn btn-outline-primary " data-bs-toggle="modal" data-bs-target="#editItemModal" title="Edit Item"><i class="bi bi-pencil-square"></i></button>
                    <button type="button" id="del" onClick=Delete('/PurchaseOrderItems/Delete/${data}') class="btn btn-outline-danger mx-1"> <i class="bi bi-trash-fill" title="Delete Item"></i></button></div>`
                },
                "width": "auto",
                orderable: false
            }
        ],
        columnDefs: [
            {
                "className": "align-middle", "targets": "_all",
            }
        ],
        footerCallback: function (row, data, start, end, display) {
            let api = this.api();

            // Remove the formatting to get integer data for summation
            let intVal = function (i) {
                return typeof i === 'string'
                    ? i.replace(/[\$,]/g, '') * 1
                    : typeof i === 'number'
                        ? i
                        : 0;
            };

            // Total over all pages
            total = api
                .column(3)
                .data()
                .reduce((a, b) => intVal(a) + intVal(b), 0);

            // Total over this page
            pageTotal = api
                .column(3, { page: 'current' })
                .data()
                .reduce((a, b) => intVal(a) + intVal(b), 0);

            // Update footer
            api.column(5).footer().innerHTML =
                '$' + Math.round(pageTotal * 100) / 100 + ' ($' + Math.round(total * 100) / 100 + ' total)';
        }
    });
    dataItemsTable.on("draw", function () {
        isReadOnly();
    })
}
function getItem(id) {
    $.ajax({
        type: "get",
        url: "/PurchaseOrderItems/GetItem",
        data: { id: id }
    }).done(function (result) {
        $("#edit-name").val(result.itemsName);
        $("#edit-qty").val(result.qty);
        $("#edit-price").val(result.unitPrice);
        $("#edit-id").val(result.id)
    })
}
function editItem() {
    $.ajax({
        type: "post",
        url: "/PurchaseOrderItems/EditItem",
        data: {
            name: $("#edit-name").val(),
            qty: $("#edit-qty").val(),
            price: $("#edit-price").val(),
            itemId: $("#edit-id").val(),
            link: $("#edit-link").val()
        }
    }).done(function (result) {
        $("#tblBody").DataTable().ajax.reload();
        if (result.success == true) {
            toastr.success(result.message);
        } else {
            toastr.error(result.message);
        }
    })
}
function addOrderItem() {
    $.ajax({
        type: "POST",
        url: "/PurchaseOrderItems/Add",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            ItemsName: $('#name').val(),
            Qty: $('#qty').val(),
            UnitPrice: $('#price').val(),
            PurchaseOrderId: $('#poId').val(),
            Link: $('#link').val()
        })
    }).done(function (data) {
        if (data.success == true) {
            toastr.success(data.message);
            $('#tblBody').DataTable().ajax.reload();
            $('#qty').val('');
            $('#price').val('');
            $('#total').val('');
            $('#link').val('');
            $('#name').val('');

        } else {
            toastr.error(data.message)
        }
    }).fail(function (data) {
        toastr.error('error')
    })
}
function cloneDataTable() {
    var dataTable = $('#orderTable').DataTable({
        serverSide: true,
        order: [[3, 'desc']],
        language: {
            lengthMenu: "Showing _MENU_ records per page",
            infoFiltered: ''
        },
        ajax: {
            url: '/PurchaseOrder/GetAll',
            type: "post",
            data: {
                selectedByDays: selectedByDate,
                selectedByStatus: selectedByStatus,
                selectedByCompany: selectedByCompany
            }
        },
        columns: [
            { data: 'vendorName' },
            { data: 'paymentType' },
            {
                data: 'date',
                render: function (data, type, row) {
                    return moment(data).format('DD.MM.YYYY, h:mm A')
                }
            },
            { data: 'status' },
            { data: 'orderedBy' },
            {
                data: 'companyName',
                "render": function (data, type, row) {
                    if (data == null) {
                        return '';
                    } else {
                        return data;
                    }
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="btn-group">
                    <button type="button" onclick="viewItemsTable('${data}'); getInfo('${data}');" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#viewModal" title="View Order Items"><i class="bi bi-eye-fill"></i></button>
                    <button type="button" onClick="cloneIt(${data})" data-bs-dismiss="modal" class="btn btn-outline-primary mx-1"><i class="bi bi-copy"></i> Clone</button>
                    </div`
                },
                /*"width": "10%",*/
                orderable: false
            }
        ],
        columnDefs: [
            {
                "className": "align-middle", "targets": "_all",
            }
        ]
    });
    $("#filterByDays").on("change", function () {
        dataTable.draw();
    });
    $("#filterByStatus").on("change", function () {
        dataTable.draw();
    });
    $("#filterByCompany").on("change", function () {
        dataTable.draw();
    });
}
function getCloneData(user) {
    $.ajax({
        url: "/PurchaseOrderItems/GetCloneData",
        data: { userName: user }
    }).done(function (result) {
        $("#filterByCompany").empty();
        $("#filterByCompany").append('<option value="0">All</option>');
        for (var i = 0; i < result.data.length; i++) {
            $("#filterByCompany").append('<option value="' + result.data[i].id + '">' + result.data[i].name + '</option>');
        }
    })
}
function cloneIt(id) {
    $.ajax({
        url: "/PurchaseOrderItems/Clone",
        type: "post",
        data: {
            cloneId: id,
            currId: $("#poId").val()
        }
    }).done(function (data) {
        if (data.success == true) {
            $('#tblBody').DataTable().ajax.reload();
            toastr.success(data.message);

        } else {
            toastr.error(data.message);
        }
    }).fail(function (data) {

    })
}
function autoComplete() {
    $("#name").autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "get",
                url: "/Items/AutoComplete",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {
                    q: request.term
                },
            }).done(function (data) {
                var s = data.map(x => ({ label: x.name, value: x.name, price: x.unitPrice }));
                response(s);
            })
        },
        autoFocus: true,
        delay: 300,
        minLength: 2,
        select: function (event, ui) {
            var e = ui.item.price;
            $("#price").val(e)
        },
    })
}
function addItems() {
    $.ajax({
        type: "post",
        url: "/Items/Add",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            Name: $('#itemName').val(),
            UnitPrice: $('#itemPrice').val(),
            CompanyId: $("#companyId").val()
        })
    }).done(function (data) {
        if (data.success == true) {
            $("#itemName").val('');
            $("#itemPrice").val('');
            toastr.success(data.message);
        } else {
            toastr.error(data.message);
        }
    }).fail(function (data) {
        toastr.error('error')
    })
};

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
                    orderId: $("#poId").val(),
                    deleteId: data
                }
            }).done(function (data) {
                if (data.message == "User has been deleted.") {
                    window.location.reload();
                }
                if (data.success == true) {
                    $('#tblBody').DataTable().ajax.reload();
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
