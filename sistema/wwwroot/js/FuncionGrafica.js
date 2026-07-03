


function retornarVentas(){

    var fecha = document.getElementById('reservationtime').value;
    console.log(fecha);

    var datos = { "fecha": fecha }

    $.ajax({
        method: "POST",
        data: JSON.stringify(datos),
        "dataType": "json",
        "contentType": "application/json",
        url: '/Venta/GraficaVentas?fecha='+fecha,
        traditional: true,
        success: function (data, textStatus) {
            // si tiene exito, reiniciemos la pag
            // pero marquemos que tuvo exito
           // alert(data);
           // location.reload();

           console.log(data);
        },
        error: function (data) {
            alert(data.error);
        }
    });


}



var ctx = document.getElementById('MiGrafica2');
var myChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: [
            
          'Vendedor1','vendedor2'
            
            
            
        ],
        datasets: [{
            label: 'Numero de Transacciones',
            data: [
                
                 55,22
                
                 ]
                   
                
               ,
            backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)',
                'rgba(255, 206, 86, 0.2)',
                'rgba(75, 192, 192, 0.2)',
                'rgba(153, 102, 255, 0.2)',
                'rgba(255, 159, 64, 0.2)'
            ],
            borderColor: [
                'rgba(255, 99, 132, 1)',
                'rgba(54, 162, 235, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(75, 192, 192, 1)',
                'rgba(153, 102, 255, 1)',
                'rgba(255, 159, 64, 1)'
            ],
            borderWidth: 1
        }]
    },
    options: {
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true
                }
            }]
        }
    }
});