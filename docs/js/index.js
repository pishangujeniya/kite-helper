function escapeRegExp(str) {
    return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, '\\$1');
}
function replaceAll(str, find, replace) {
    return str.replace(new RegExp(this.escapeRegExp(find), 'g'), replace);
}
function toTitleCase(str) {
    return str.replace(/\w\S*/g, (txt) => {
        return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
    });
}
function arrayToCSV(dataArray, headers = null, dataKeyNames = null, serialNumberColumn = true) {
    const array = typeof dataArray !== 'object' ? JSON.parse(dataArray) : dataArray;
    if (dataKeyNames === null) {
        dataKeyNames = Object.keys(dataArray[0]);
    }
    if (headers === null) {
        headers = new Array();
        for (let i = 0; i < dataKeyNames.length; i++) {
            const dataKeyName = dataKeyNames[i];
            let y = '';
            for (let index = 0; index < dataKeyName.length; index++) {
                if (dataKeyName[index] === dataKeyName[index].toUpperCase()) {
                    if ((index + 1) < length && dataKeyName[index + 1] === dataKeyName[index + 1].toLowerCase()) {
                        // Next char is lower
                        y = y + dataKeyName[index];
                    }
                    else {
                        // Next char is upper
                        if (index !== 0) {
                            y = y + '_';
                        }
                        y = y + dataKeyName[index];
                    }
                }
                else {
                    y = y + dataKeyName[index];
                }
            }
            let x = y;
            x = this.replaceAll(x, '_', ' ');
            x = this.toTitleCase(x);
            x = this.replaceAll(x, '  ', ' ');
            headers.push(x);
        }
    }
    let str = '';
    let row = '';
    if (serialNumberColumn) {
        row = row + 'S.No,';
    }
    else {
        // Serial Number Column not wanted
    }
    for (const index in headers) {
        row += headers[index] + ',';
    }
    row = row.slice(0, -1);
    str += row + '\r\n';
    for (let i = 0; i < array.length; i++) {
        let line = '';
        if (serialNumberColumn) {
            line = line + i + 1 + '';
        }
        else {
            // Serial Number Column not wanted
        }
        for (let dataKeyNameIndex = 0; dataKeyNameIndex < dataKeyNames.length; dataKeyNameIndex++) {
            const head = dataKeyNames[dataKeyNameIndex];
            if (serialNumberColumn) {
                line += ',';
            }
            else {
                if (dataKeyNameIndex === 0) {
                    // Serial Number Column not wanted
                }
                else {
                    line += ',';
                }
            }
            line += JSON.stringify(array[i][head]);
        }
        str += line + '\r\n';
    }
    return str;
}

function exportToCSVFile(dataArray, filenameWithoutExtension = 'csv_data', headers = null, dataKeyNames = null, serialNumberColumn = true) {
    if (dataArray instanceof Array) {
        if (dataArray.length < 1) {
            throw new Error('data length must be > 0');
        }
    }
    else {
        throw new Error('data must be Array');
    }
    const csvData = this.arrayToCSV(dataArray, headers, dataKeyNames, serialNumberColumn);
    const blob = new Blob(['\ufeff' + csvData], {
        type: 'text/csv;charset=utf-8;'
    });
    const dwldLink = document.createElement('a');
    const url = URL.createObjectURL(blob);
    const isSafariBrowser = navigator.userAgent.indexOf('Safari') !== -1 &&
        navigator.userAgent.indexOf('Chrome') === -1;
    if (isSafariBrowser) {
        // if Safari open in new window to save file with random filename.
        dwldLink.setAttribute('target', '_blank');
    }
    dwldLink.setAttribute('href', url);
    dwldLink.setAttribute('download', filenameWithoutExtension + '.csv');
    dwldLink.style.visibility = 'hidden';
    document.body.appendChild(dwldLink);
    dwldLink.click();
    document.body.removeChild(dwldLink);
}

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
            profile: "/accounts/kite/profile",
            update_instruments_csv:"/accounts/kite/update_instruments_csv"
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

    $scope.kiteInstrumentsUpdateClick = function() {
        $http.get($scope.configurationInputs.configurationAPIPathInput + $scope.kiteHelper.config.accounts_resources.update_instruments_csv, {
        }).then(function (response) {
            if (response.status == 200) {
            } else {
            }
            console.log(response);
        });
    }

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
            if (response.status == 200) {
                $scope.kiteInputs.kiteLoginSuccess = true;
                $scope.kiteInputs.kiteLoginResponseMessage = response.data.result.email;
            } else {
                $scope.kiteInputs.kiteLoginSuccess = false;
                $scope.kiteInputs.kiteLoginResponseMessage = JSON.stringify(response.data.error_messages);
            }

            console.log(response);
        });
    };

    //#endregion Kite Login

    //#region Select Instrument
    $scope.selectInstruments = {};
    $scope.selectInstruments.possibleExchanges = ['NSE', 'BSE', 'NFO', 'MCX', 'BCD', 'CDS'];
    $scope.selectInstruments.selectedExchange = "";
    $scope.selectInstruments.searchedTradingSymbolQuery = "";

    $scope.selectInstruments.filteredTradingSymbols = [];

    $scope.searchTradingSymbolQueryOnChange = function () {
        if ($scope.selectInstruments.searchedTradingSymbolQuery.length > 3) {
            $http.post($scope.configurationInputs.configurationAPIPathInput + $scope.kiteHelper.config.instruments_resources.get_instrument_trading_symbol, {
                symbol: $scope.selectInstruments.searchedTradingSymbolQuery,
                exchange: $scope.selectInstruments.selectedExchange
            }).then(function (response) {
                $scope.selectInstruments.filteredTradingSymbols = [];
                for (let index = 0; index < response.data.result.length; index++) {
                    const element = response.data.result[index];
                    $scope.selectInstruments.filteredTradingSymbols.push({ 'name': element });
                }
                console.log($scope.selectInstruments.filteredTradingSymbols);
                $scope.$applyAsync();
            });
        } else {
            $scope.selectInstruments.filteredTradingSymbols = [];
        }
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

    $scope.getHistoricalData = function () {
        $http.post($scope.configurationInputs.configurationAPIPathInput + $scope.kiteHelper.config.instruments_resources.get_historical_data, {
            exchange: $scope.selectInstruments.selectedExchange,
            trading_symbol: $scope.selectInstruments.searchedTradingSymbolQuery,
            start_dtm: $scope.selectDateRangeInputs.selectedStartDateString,
            end_dtm: $scope.selectDateRangeInputs.selectedEndDateString,
            interval: $scope.selectDateRangeInputs.selectedInterval
        }).then(function (response) {
            var localTimeZoneData = [];
            for (let index = 0; index < response.data.result.length; index++) {
                const element = response.data.result[index];
                const gmtTime = moment.utc(moment(new Date(element.date))).toDate();
                element.local_date = moment(gmtTime).format("YYYY-MM-DD HH:mm:ss");
                localTimeZoneData.push(element);
            }

            var fileName = $scope.selectInstruments.searchedTradingSymbolQuery;
            exportToCSVFile(localTimeZoneData, fileName);
        });
    }





    //#endregion Select Date Range

});