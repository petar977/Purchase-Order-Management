function createOrder(url, data) {
    return $.ajax({
        method: "post",
        url: url,
        data: data
    }).then(function (data) {
        if (data.success == true) {
            toastr.success(data.message);
        } else {
            toastr.error(data.message)
        }
        $("#poId").val(data.id)
    }).fail(function (data) {
        toastr.error('error')
    })
}
function saveOrder(url,data) {
    $.ajax({
        method: "post",
        url: url,
        data:data
    }).done(function (data) {
        if (data.success == true) {
            toastr.success(data.message);
        } else {
            toastr.error(data.message)
        }    
    }).fail(function (data) {
        toastr.error('error')
    })
}
function orderDataTable() {
    var dataTable = $('#orderTable').DataTable({
        "serverSide": true,
        "order": [[3, 'desc']],
        "ajax": {
            url: '/PurchaseOrder/GetAll',
            type: "post",
            data: {
                selectedByDays: selectedByDate,
                selectedByStatus: selectedByStatus,
                selectedByCompany: selectedByCompany
            }
        },
        "columns": [
            { data: 'poNumber'},
            { data: 'vendorName' },
            { data: 'paymentType' },
            {
                data: 'date',
                render: function (data, type, row) {
                    return moment(data).format('DD.MM.YYYY, h:mm A')
                }
            },
            { data: 'status' },
            {
                data: 'approvedDate', render: function (data, type, row) {
                    if (row.approvedDate == null) {
                        return ""
                    } else {
                        return moment(row.approvedDate).format('DD.MM.YYYY, h:mm A')
                    }
                }
            },
            { data: 'approvedBy' },
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
                    <button type="button" onclick="viewItemsTable('${data}'); getInfo('${data}');" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#myModal" title="View Order Items"><i class="bi bi-eye-fill"></i></button>
                    <a href='/PurchaseOrder/Edit/${data}' class="btn btn-success mx-1" title="Edit Order"><i class="bi bi-pencil-square"></i></a>
                    <button type="button" id="btnOpenDocument" onclick="window.open('/PDF/GeneratePdf/${data}', '_blank');" class="btn btn-primary" title="Pdf"><i class="bi bi-printer"></i></button>
                    </div>`
                },
                orderable: false
            },
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
    });
    $("#filterByDays").on("change", function () {
        dataTable.draw();
    })
    $("#filterByStatus").on("change", function () {
        dataTable.draw();
    }) 
    $("#filterByCompany").on("change", function () {
        dataTable.draw();
    })
}
function selectedByDate() {
    return $("select#filterByDays option:selected").val();
}
function selectedByStatus() {
    return $("select#filterByStatus option:selected").val();
}
function selectedByCompany() {
    return $("select#filterByCompany option:selected").val();
}
function changeStatus(url) {
    $(".onChangeDisable").addClass("disabled");
    $.ajax({
        url: url,
    }).done(function (data) {
        if (data.success == true) {           
            window.location.reload();
        } else {
            toastr.error(data.message);
        }       
    }).fail(function (data) {
        toastr.error('error')
    })
}
function isReadOnly() {
    if ($("#readOnly").is(":checked") || $("#status").val() == "Approved" || $("#status").val() == "Denied"
        || $("#status").val() == "Canceled" || $("#status").val() == "Pending")
    {
        $("input").prop("disabled", true);
        $("input[name='__RequestVerificationToken']").prop("disabled", false);
        $("textarea").prop("disabled", true);
        $("#editForm button").addClass("disabled");
        $("#editForm #back").removeClass("disabled");
        $("button").prop("disabled", true);
        $('#btnOpenDocument').prop('disabled', false);
        $("#logout").prop("disabled", false);
    }
}
function newPartial() {
    $.ajax({
        url: "/PurchaseOrder/TestPartial"
    }).done(function (result) {
        $("#partial").html(result);
    })
}
function viewItemsTable(id) {
    var dataTable = $('#viewTable').DataTable({
        "serverSide": false,
        "ajax": {
            url: '/PurchaseOrderItems/GetItems',
            data: { id: id },
        },
        language: {
            lengthMenu: "Showing _MENU_ records per page",
            infoFiltered: '',
            search: ""
        },
        searching: false,
        "columns": [
            { data: 'itemsName', "width": 'auto' },
            { data: 'qty', "width": "40px" },
            { data: 'unitPrice', "width": "auto", render: function (data) { return Math.round(data * 100) / 100 } },
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
            { data: 'total', "width": "auto", render: function (data) { return Math.round(data * 100) / 100 } },
        ], columnDefs: [
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
                .column(4)
                .data()
                .reduce((a, b) => intVal(a) + intVal(b), 0);

            // Total over this page
            pageTotal = api
                .column(4, { page: 'current' })
                .data()
                .reduce((a, b) => intVal(a) + intVal(b), 0);

            // Update footer
            api.column(4).footer().innerHTML =
                '$' + Math.round(pageTotal*100)/100 + ' ($' + Math.round(total*100)/100 + ' total)';
        }
    });
}
function getInfo(id) {
    $.ajax({
        url: "/PurchaseOrder/GetInfo",
        data: { id: id },
    }).done(function (result) {
        $("#info").text(result.data)
    })
}
function selectCounter() {
    $.ajax({
        type: "get",
        url: "/PurchaseOrder/AddPages",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
    }).done(function (data) {
        var counter = $("#order-select");
        var companies = selectedComp();
        for (let i = 0; i < data.data.counter.length; i++) {
            if (companies == data.data.counter[i].companyId) {
                counter.append(new Option(data.data.counter[i].fullNameLetter, data.data.counter[i].id));
            }        
        }
    })
}
function selectedComp() {
    return $("select#company-select option:selected").val();
}


        






