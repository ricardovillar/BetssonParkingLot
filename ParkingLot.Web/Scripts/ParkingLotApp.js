var ParkingLotApp = angular.module('ParkingLotApp', ['ui.bootstrap.datetimepicker']);

ParkingLotApp.constant("moment", moment);

ParkingLotApp.directive('date', function (dateFilter) {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {

            var dateFormat = attrs['date'] || 'yyyy-MM-dd';

            ctrl.$formatters.push(function (modelValue) {
                return dateFilter(modelValue, dateFormat);
            });
        }
    };
});

ParkingLotApp.controller('HomeController', HomeController);