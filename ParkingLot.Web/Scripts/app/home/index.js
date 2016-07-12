(function ($) {
    $(function () {
        $('input.timepicker').timepicker({
            interval: 30
        });
    });


    var buildChart = function (req) {

        var data = req.Data.map(function (chartPoint) {
            return { x: chartPoint.Date, y: chartPoint.Cars };
        });

        var ctx = $("#occupation-chart");
        var occupationChart = new Chart(ctx, {
            type: 'line',
            responsive: false,
            data: {
                datasets: [{
                    label: "My First dataset",
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(75,192,192,0.4)",
                    borderColor: "rgba(75,192,192,1)",
                    borderCapStyle: 'butt',
                    borderDash: [],
                    borderDashOffset: 0.0,
                    borderJoinStyle: 'miter',
                    pointBorderColor: "rgba(75,192,192,1)",
                    pointBackgroundColor: "#fff",
                    pointBorderWidth: 1,
                    pointHoverRadius: 5,
                    pointHoverBackgroundColor: "rgba(75,192,192,1)",
                    pointHoverBorderColor: "rgba(220,220,220,1)",
                    pointHoverBorderWidth: 2,
                    pointRadius: 1,
                    pointHitRadius: 10,
                    data: data
                }]
            },
            options: {
                scales: {
                    xAxes: [{
                        type: 'linear',
                        position: 'bottom'
                    }]
                }
            }
        });

    }

    $.ajax({
        url: '/Home/GetData',
        success: buildChart
    });

})(jQuery);



