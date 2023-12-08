// dashboard.js
$(function () {
    console.log('Document ready!');
  
    // Aplicar filtro al hacer clic en el botón
    $('#applyFilter').on('click', function () {
        var cityFilter = $('#cityFilter').val().toLowerCase();
        var typeFilter = $('#typeFilter').val().toLowerCase();
        filterTable(cityFilter, typeFilter);
    });

    
    // Función para filtrar la tabla
    function filterTable(cityFilter, typeFilter) {
        $('table tbody tr').hide();

        $('table tbody tr').each(function () {
            var city = $(this).children('td').eq(1).text().toLowerCase();
            var type = $(this).children('td').eq(2).text().toLowerCase();

            if ((cityFilter === '' || city.includes(cityFilter)) && (typeFilter === '' || type.includes(typeFilter))) {
                $(this).show();
            }
        });
    }
});