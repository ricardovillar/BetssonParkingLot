(function ($) {
    $(function () {
        $('input.timepicker').timepicker({
            interval: 30
        });
    });

    $('#getData').on('click', getData);
    $('#plotAllTimeData').on('click', plotAllTimeData);

    function getData() {
        var apiUrl = $("#apiUrl").val();
        $.ajax({
            url: '/betsson/Home/GetData?apiUrl=' + apiUrl,
            success: buildChart
        });
    }

    function plotAllTimeData() {
        $.ajax({
            url: '/Home/GetAllTimeData',
            success: buildChart
        });
        return true;
    }

    function buildChart(req) {
        Chart.defaults.global.defaultFontSize = defineFontSize();
        var data = req.Data.map(function (chartPoint) {
            return chartPoint.Cars;
        });
        var labels = req.Data.map(function (chartPoint) {
            return toJsDate(chartPoint.Date).toLocaleString();
        });
        //var fontSize = defineFontSize();
        var ctx = $("#parking-chart");
        var occupationChart = new Chart(ctx, {
            type: 'line',
            responsive: false,
            data: {
                labels: labels,
                datasets: [{
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
                legend: {
                    display: false
                }
            }
        });
    }

    function toJsDate(date) {
        var d = /\/Date\((\d*)\)\//.exec(date);
        return new Date(+d[1]);
    }

    function defineFontSize() {
        var mq = window.matchMedia("(min-width: 48em)");
        if (mq.matches) {
            return 12;
        } else {
            return 6;
        }
    }

})(jQuery);




