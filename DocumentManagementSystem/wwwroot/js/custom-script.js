/* cascading dropdown */
$(document).ready(function () {
    var state = $("#State");
    var lga = $("#lga");
    lga.prop('disabled', true);

    state.change(function () {
        if ($(this).val() == "") {
            lga.append($('<option>', { value: '', text: '---- Select LGA ----' }, '</option>'));
            lga.prop('disabled', true);
            lga.val("");
        }
        else {
            $.ajax({
                url: "https://localhost:44319/admin/GetLga",
                method: "GET",
                data: { state: $(this).val() },
                success: function (data) {
                    /* console.log(data);*/
                    lga.prop('disabled', false);
                    lga.empty();
                    lga.append($('<option>', { value: '0', text: '---- Select LGA ----' }, '</option>'));
                    $(data).each(function (index, item) {
                        lga.append($('<option>', { value: item.value, text: item.text }, '</option>'))
                        //    lga.append($('<option/>', { value: item.Value, text: item.Text }));
                    });
                    /* console.log("finish");*/
                }
            });
        }
    });
});