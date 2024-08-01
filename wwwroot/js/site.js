$(document).ready(function () {
    var buttonClicked = false;
    $("#InProcess").click(function () {
        buttonClicked = true;
        LoadListing("InProcess");
    });
    $("#Pending").click(function () {
        buttonClicked = true;
        LoadListing("Pending");
    });
    $("#Completed").click(function () {
        buttonClicked = true;
        LoadListing("Completed");
    });
    $("#Cancel").click(function () {
        buttonClicked = true;
        LoadListing("Cancel");
    });
    $("#All").click(function () {
        buttonClicked = true;
        LoadListing(null);
    });
    if (!buttonClicked) {
        LoadListing(null);
    }
});
function LoadListing(status) {
    var orderData = [];

    if (status != null) {
        Url = "/Home/GetAllOrder?status=" + status;
    } else {
        Url = "/Home/GetAllOrder";
    }
    $.ajax({
        type: "GET",
        url: Url,
        async: false,
        success: function (data) {
            $.each(data, function (key, value) {
                var OrderDetailsMorebtn = "<td><a href='/Home/OrderDetailsMore/" + value.id + "'><span class='bb-btnEdit w-100 text-center'><i class='bi bi-pencil-square'></i></span></a></td >";
                orderData.push([value.id, value.name, value.phone, value.email, value.paymentStatus, value.amount, OrderDetailsMorebtn])
            });
            $('#tbllist').DataTable().destroy();
            $('#tbllist').DataTable({
                data: orderData,
                language: {
                    emptyTable: "No data available",
                    lengthMenu:
                        'Show <select>' +
                        '<option value="5">5</option>' +
                        '<option value="10">10</option>' +
                        '<option value="15">15</option>' +
                        '<option value="30">30</option>' +
                        '<option value="50">50</option>' +
                        '<option value="-1">All</option>' +
                        '</select> records'
                }
            });
        },
        error: function (err) {
            console.log(err);
        }
    });

}