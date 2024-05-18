function loadCounterTable() {
    dataTable = $('#counterTbl').DataTable({
        serverSide: false,
        order: [[0, 'asc']],
        ajax: {
            url: "/OrderCount/GetAllCounters",
            type: "post",
            data: {

            }
        },
        columns: [
            { data: "companyName" },
            { data: "year" },
            { data: "firstLetter" },
            { data: "fullNameLetter" },
            { data: "count" },
            {
                data: "id",
                render: function (data) {
                    return `<div class="btn-group">
                    <button type="button" onclick="getEditData(${data})" data-bs-toggle="modal" data-bs-target="#editCounterModal" class="btn btn-primary" title="Edit"><i class="bi bi-pencil-square"></i></button>
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
}
function getEditData(id) {
    $.ajax({
        type: "get",
        url: "/OrderCount/GetEditData",
        data: { id : id }
    }).done(function (result) {
        $('#edit-firstLetter').val(result.firstLetter);
        $('#edit-type').val(result.fullNameLetter);
        $('#edit-counter').val(result.count);
        $('#edit-id').val(result.id)
    })
}
function editCounter() {
    $.ajax({
        url: '/OrderCount/editData',
        type: 'post',
        data: {
            id : $("#edit-id").val(),
            type: $('#edit-type').val(),
            firstLetter: $('#edit-firstLetter').val(),
            counter: $('#edit-counter').val()
        }
    }).done(function (result) {
        if (result.success == true) {
            toastr.success(result.message)
            $('#counterTbl').DataTable().ajax.reload();

        } else {
            toastr.error(result.message)
        }
    })
}
