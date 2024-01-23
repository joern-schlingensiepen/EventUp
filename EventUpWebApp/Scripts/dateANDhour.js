$(function () {
    // current date
    var currentDate = new Date();
    var minStartDate = formatDate(currentDate);

    $("#Start_DateTime").attr("min", minStartDate);

    
    $("#Start_DateTime").change(function () {
        
        var startDate = new Date($(this).val());

        
        if (startDate < currentDate) {
            $(this).val(minStartDate);
        }

        // End_Date Time after Start_DateTime
        $("#End_DateTime").attr("min", formatDate(startDate));
    });

    // format the date as required for the attribute "min".
    function formatDate(date) {
        var year = date.getUTCFullYear();
        var month = ('0' + (date.getUTCMonth() + 1)).slice(-2);
        var day = ('0' + date.getUTCDate()).slice(-2);
        var hours = ('0' + date.getUTCHours()).slice(-2);
        var minutes = ('0' + date.getUTCMinutes()).slice(-2);

        return year + '-' + month + '-' + day + 'T' + hours + ':' + minutes;
    }
});

