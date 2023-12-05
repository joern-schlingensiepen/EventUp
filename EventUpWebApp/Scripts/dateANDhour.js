$(function () {
    // Manejar el cambio en la fecha de inicio
    $("#Start_DateTime").change(function () {
        // Obtener el valor de la fecha de inicio
        var startDate = new Date($(this).val());

        // Establecer la fecha mínima para la fecha de finalización
        $("#End_DateTime").attr("min", formatDate(startDate));
    });

    // Función para formatear la fecha como se requiere para el atributo "min"
    function formatDate(date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var hours = date.getHours();
        var minutes = date.getMinutes();

        // Agregar ceros principales según sea necesario
        day = day < 10 ? '0' + day : day;
        month = month < 10 ? '0' + month : month;
        hours = hours < 10 ? '0' + hours : hours;
        minutes = minutes < 10 ? '0' + minutes : minutes;

        return year + '-' + month + '-' + day + 'T' + hours + ':' + minutes;
    }
});
