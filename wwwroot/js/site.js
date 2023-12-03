//Pokemon Index
$(document).ready(function () {
   /* $("#filterForm").submit(function () { //se ejecuta al hacer submit

        var selectedValue = $("#Tipos").val(); // el id "Tipos" se genera automáticamente por el helper DropDownListFor a partir de la expresión (m => m.Tipos)
        var baseAction = $(this).attr("action"); //obtiene la url actual de acción del formulario (el atributo "asp-action" del formulario)

        // Se actualiza la url actual de acción del formulario (si no, se concatena con cada envío en la url). Si lo hay, se sustituye el número del final de la url de la anterior búsqueda con una cadena vacía y luego concatena el value de la opción seleccionada, que equivale al id del tipo para así hacer correctamente la petición al método FiltrarPokemonPorTipo.
        $(this).attr("action", baseAction.replace(/\/\d+$/, "") + "/" + selectedValue);

        return true; //al devolver true el formulario continúa con el envío
    });

    //Se guarda la opcion seleccionada en la session storage
    document.getElementById("filterForm").addEventListener("submit", function () {
        var valorSeleccionado = document.getElementById("Tipos").value;
        sessionStorage.setItem("valorSeleccionado", valorSeleccionado);
    });

    //Se recupera el valor almacenado para que se convierta en la opción seleccionada
    window.addEventListener("load", function () {
        var valorSeleccionado = sessionStorage.getItem("valorSeleccionado");
        if (valorSeleccionado) {
            document.getElementById("Tipos").value = valorSeleccionado;
        }
    });*/

    //timeout para mensaje de error
   /* document.getElementById("peso").addEventListener("input", function (event) {
        this.value = this.value.replace(',', '.');
    });

    document.getElementById("altura").addEventListener("input", function (event) {
        this.value = this.value.replace(',', '.');
    });*/
    setTimeout(function () {
        $(".alert").fadeOut("slow");
    }, 3000);


});

//
