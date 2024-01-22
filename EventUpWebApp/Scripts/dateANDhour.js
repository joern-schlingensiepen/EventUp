$(function () {
    // Obtener la fecha actual en formato UTC
    var currentDate = new Date();
    var minStartDate = formatDate(currentDate);

    // Establecer la fecha mínima para el campo de fecha de inicio
    $("#Start_DateTime").attr("min", minStartDate);

    // Manejar el cambio en la fecha de inicio
    $("#Start_DateTime").change(function () {
        // Obtener el valor de la fecha de inicio
        var startDate = new Date($(this).val());

        // Establecer la fecha mínima para el campo de fecha de finalización
        $("#End_DateTime").attr("min", formatDate(startDate));
    });

    // Función para formatear la fecha como se requiere para el atributo "min"
    function formatDate(date) {
        var year = date.getUTCFullYear();
        var month = ('0' + (date.getUTCMonth() + 1)).slice(-2);
        var day = ('0' + date.getUTCDate()).slice(-2);
        var hours = ('0' + date.getUTCHours()).slice(-2);
        var minutes = ('0' + date.getUTCMinutes()).slice(-2);

        return year + '-' + month + '-' + day + 'T' + hours + ':' + minutes;
    }
});
