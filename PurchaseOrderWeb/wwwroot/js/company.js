function companyAdd() {
    $.ajax({
        type: "post",
        url: "/Company/Add",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            Name: $("#company-name").val(),
            Email: $("#company-email").val(),
            CompanyLetter: $("#company-letter").val(),
            Address: $("#company-address").val(),
            City: $("#company-city").val(),
            ZipCode: $("#company-zipCode").val()
        })
    }).done(function (result) {
        window.location.reload();
    })
}
function companyEdit(data) {
    $.ajax({
        type: "post",
        url: "/Company/EditCompany",
        data: data
    }).done(function (result) {
        window.location.reload();
    })
}
function getCompanyEdit(id) {
    $.ajax({
        type: "get",
        url: "/Company/GetCompany",
        data: {
            id: id
        }
    }).done(function (result) {
        $('#edit-Email').val(result.email);
        $('#edit-Name').val(result.name);
        $('#company-Id').val(result.id);
        $('#edit-address').val(result.address);
        $('#edit-zipCode').val(result.zipCode);
        $('#edit-city').val(result.city);
        $('#edit-status').val(result.status);
    })
}
function changeStatus(id) {
    $.ajax({
        type: "post",
        url: "/Company/ChangeStatus",
        data: { id: id}
    }).done(function (result) {
        window.location.reload();
    })
}
