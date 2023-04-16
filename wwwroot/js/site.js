// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function FillStreets(lstCityCtrl, lstSteetId) {
    var lstStreets = $("#" + lstSteetId);
    lstStreets.empty();

    var selectedCity = lstCityCtrl.options[lstCityCtrl.selectedIndex].value;

    if (selectedCity != null && selectedCity != '') {
        $.getJSON("/Address/GetStreetsByCity", { cityId: selectedCity }, function (streets) {
            if (streets != null && !jQuery.isEmptyObject(streets)) {
                $.each(streets, function (index, street) {
                    lstStreets.append($('<option/>', {
                        value: street.value,
                        text: street.text
                    }))
                });
            };
        })
    }

    return;
}