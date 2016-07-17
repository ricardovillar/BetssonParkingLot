var HomeController = function ($scope, $http) {
    var home = this;

    $scope.init = function (config) {
        home.urls = {
            api: config.ApiUrl,
            getData: decodeURI(config.GetDataUrl),
            getPartialData: decodeURI(config.GetPartialDataUrl),
            getFullData: config.GetFullDataUrl,
        };
    };

    home.data = {
        maxOccupationRate: "N/A",
        maxOccupationDate: "N/A"
    };

    home.getData = function () {
        var url = home.urls.getData.replace('(apiUrl)', home.urls.api);
        $http.get(url).then(handleData.bind(home), handleError);
    }

    function handleData(req) {
        this.data.maxOccupationRate = req.data.MaxOccupationRate;
        this.data.maxOccupationDate = toJsDate(req.data.MaxOccupationDate).toLocaleString();
        buildChart(req.data.Data);
    }

    function buildChart(chartData) {
        Chart.defaults.global.defaultFontSize = defineFontSize();
        var data = chartData.map(function (chartPoint) {
            return chartPoint.Cars;
        });
        var labels = chartData.map(function (chartPoint) {
            return toJsDate(chartPoint.Date).toLocaleString();
        });
        var ctx = document.getElementById("parking-chart");
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

    function handleError() {
        debugger;
    }

}


HomeController.$inject = ['$scope', '$http'];