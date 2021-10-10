

var myapp = angular.module("myapp", []);

myapp.filter('prettify', function () {

    function syntaxHighlight(json) {
        json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
        return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
            var cls = 'number';
            if (/^"/.test(match)) {
                if (/:$/.test(match)) {
                    cls = 'key';
                } else {
                    cls = 'string';
                }
            } else if (/true|false/.test(match)) {
                cls = 'boolean';
            } else if (/null/.test(match)) {
                cls = 'null';
            }
            return '<span class="' + cls + '">' + match + '</span>';
        });
    }

    return syntaxHighlight;
});

myapp.controller("IndexController", function ($scope, $http) {

    //#region Configuration 

    $scope.kiteHelper = {};
    $scope.kiteHelper.version = "1.0"
    $scope.kiteHelper.config = {
        accounts_resources: {
            login: "/accounts/kite/login",
            profile: "/accounts/kite/profile"
        },
        instruments_resources: {
            get_all_instruments: "/instruments/get_all_instruments",
            get_all_instruments_1: "/instruments/get_all_instruments_1",
            get_historical_data: "/instruments/get_historical_data",
            get_instrument_trading_symbol: "/instruments/get_instrument_trading_symbol"
        }
    }

    $scope.configurationInputs = {};
    $scope.configurationInputs.configurationAPIPathInput = "http://192.168.0.109:4000";

    //#endregion Configuration

    //#region Kite Login
    $scope.kiteInputs = {};
    $scope.kiteInputs.kiteUsernameInput = "";
    $scope.kiteInputs.kitePasswordInput = "";
    $scope.kiteInputs.kitePinInput = "";
    $scope.kiteInputs.kiteLoginResponseMessage = "Not Logged In";
    $scope.kiteInputs.kiteLoginSuccess = false;

    $scope.kiteLoginOnClick = function () {
        $http.post($scope.configurationInputs.configurationAPIPathInput + $scope.kiteHelper.config.accounts_resources.login, {
            username: $scope.kiteInputs.kiteUsernameInput,
            password: $scope.kiteInputs.kitePasswordInput,
            pin: $scope.kiteInputs.kitePinInput
        }).then(function (response) {
            console.log(response);
        });
    };

    //#endregion Kite Login

    //#region Select Instrument
    $scope.selectInstruments = {};
    $scope.selectInstruments.possibleExchanges = ['NSE', 'BSE', 'NFO', 'MCX', 'BCD', 'CDS'];
    $scope.selectInstruments.selectedExchange = undefined;

    $scope.selectInstrumentOnClick = function () {
        console.log($scope.selectInstruments.selectedExchange);
        console.log($scope.selectDateRangeInputs.selectedEndDateString);
    }

    //#endregion Select Instrument

    //#region Select Date Range

    $scope.selectDateRangeInputs = {};
    $scope.selectDateRangeInputs.possibleIntervals = ['minute', '5minute', '10minute', '15minute', 'day'];
    $scope.selectDateRangeInputs.selectedInterval = "";
    $scope.selectDateRangeInputs.selectedStartDateString = "";
    $scope.selectDateRangeInputs.selectedEndDateString = "";

    $scope.selectIntervalRadioOnChange = function () {

        $scope.selectDateRangeInputs.selectedStartDateString = "";
        $scope.selectDateRangeInputs.selectedEndDateString = "";

        var dateTimePickerConfiguration = {
            opens: 'right',
            showDropdowns: true,
            timePicker: true,
            startDate: moment().startOf('hour'),
            endDate: moment().startOf('hour').add(32, 'hour'),
            locale: {
                format: 'DD/MM/YYYY hh:mm A'
            }
        };
        if ($scope.selectDateRangeInputs.selectedInterval == $scope.selectDateRangeInputs.possibleIntervals[0]) {
            dateTimePickerConfiguration.maxSpan = {
                days: 59
            };
        }

        $('input[name="datetimes"]').daterangepicker(dateTimePickerConfiguration);

        $('input[name="datetimes"]').on('apply.daterangepicker', function (ev, picker) {
            $scope.selectDateRangeInputs.selectedStartDateString = picker.startDate.format('YYYY-MM-DD hh:mm:ss').toString();
            $scope.selectDateRangeInputs.selectedEndDateString = picker.endDate.format('YYYY-MM-DD hh:mm:ss').toString();
            $scope.$apply();
        });

    }





    //#endregion Select Date Range

});