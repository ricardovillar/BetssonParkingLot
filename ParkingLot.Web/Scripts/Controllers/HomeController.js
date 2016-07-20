var HomeController = function ($scope, $http, moment, dateFilter, toaster) {
    var RequestDateFormat = 'YYYY-MM-DDTHH:mm';
    var ViewDateFormat = 'DD/MM/YYYY HH:mm';
    var chartCtx = document.getElementById("parking-chart");

    var home = this;

    $scope.init = function (config) {
        home.urls = {
            api: config.ApiUrl,
            getData: decodeURI(config.GetDataUrl),
            getPartialData: decodeURI(config.GetPartialDataUrl),
            getFullData: config.GetFullDataUrl,
        };
    };

    $scope.beforeRenderRangeDate = function ($view, $dates, $leftDate, $upDate, $rightDate) {
        if (home.data.startRange && home.data.endRange) {
            var startDate = moment(home.data.startRange);
            var endDate = moment(home.data.endRange);
            for (var i = 0; i < $dates.length; i++) {
                var localDate = $dates[i].localDateValue();
                if (localDate < startDate.valueOf() || localDate > endDate.valueOf()) {
                    $dates[i].selectable = false;
                }
            }
        }
    }

    home.data = {
        maxOccupationRate: 'N/A',
        maxOccupationDate: 'N/A',
        startRange: undefined,
        endRange: undefined,
        filterStartRange: undefined,
        filterEndRange: undefined
    };

    home.getData = function () {
        var url = home.urls.getData.replace('(apiUrl)', encodeURIComponent(home.urls.api));
        requestData.call(this, url, true);
    }

    home.getPartialData = function () {
        if (home.data.filterStartRange && home.data.filterEndRange) {
            if (home.data.filterStartRange >= home.data.filterEndRange) {
                showErrorMessage('Invalid Range', 'Start Date must be before than End Date.');
                return;
            }
            var start = moment(home.data.filterStartRange).format(RequestDateFormat);
            var end = moment(home.data.filterEndRange).format(RequestDateFormat);
            var url = home.urls.getPartialData.replace('(start)', start).replace('(end)', end);
            requestData.call(this, url, false);
        }
    }

    home.restoreFullData = function () {
        var url = home.urls.getFullData;
        requestData.call(this, url, true);
    }

    home.selectParkingLot = function () {
        showInfoMessage('Ooops ...', 'This feature is not avaible yet, but it is coming soon!');
    }

    function requestData(url, updateRange) {
        $http.get(url).then(handleData.bind(home, updateRange), handleError);
    }

    function handleData(updateRange, req) {
        this.data.maxOccupationRate = req.data.MaxOccupationRate;
        this.data.maxOccupationDate = moment(req.data.MaxOccupationDate).format(ViewDateFormat);
        if (updateRange) {
            this.data.startRange = moment(req.data.StartDate).toDate();
            this.data.endRange = moment(req.data.EndDate).toDate();
            this.data.filterStartRange = this.data.startRange;
            this.data.filterEndRange = this.data.endRange;
        }
        buildChart(req.data.Data);
    }

    function buildChart(chartData) {
        Chart.defaults.global.defaultFontSize = defineFontSize();
        var data = chartData.map(function (chartPoint) {
            return chartPoint.Cars;
        });
        var labels = chartData.map(function (chartPoint) {
            return moment(chartPoint.Date).format(ViewDateFormat);
        });

        if (home.occupationChart) {
            home.occupationChart.destroy();
        }

        home.occupationChart = new Chart(chartCtx, {
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

    function defineFontSize() {
        var mq = window.matchMedia("(min-width: 48em)");
        if (mq.matches) {
            return 12;
        } else {
            return 6;
        }
    }

    function handleError(response) {
        showErrorMessage(response.statusText, response.data);
    }

    function showErrorMessage(title, message, timeout) {
        showMessage('error', title, message, timeout);
    }

    function showInfoMessage(title, message, timeout) {
        showMessage('info', title, message, timeout);
    }

    function showMessage(type, title, message, timeout) {
        toaster.pop({
            type: type,
            title: title,
            body: message,
            timeout: timeout
        });
    }

}


HomeController.$inject = ['$scope', '$http', 'moment', 'dateFilter', 'toaster'];