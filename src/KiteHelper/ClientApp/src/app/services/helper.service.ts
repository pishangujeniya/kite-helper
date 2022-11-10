import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class HelperService {

  constructor() { }

  public escapeRegExp(str) {
    return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, '\\$1');
  }
  public replaceAll(str, find, replace) {
    return str.replace(new RegExp(this.escapeRegExp(find), 'g'), replace);
  }
  public toTitleCase(str) {
    return str.replace(/\w\S*/g, (txt) => txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase());
  }
  public arrayToCSV(dataArray, headers = null, dataKeyNames = null, serialNumberColumn = true) {
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
            } else {
              // Next char is upper
              if (index !== 0) {
                y = y + '_';
              }
              y = y + dataKeyName[index];
            }
          } else {
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
    } else {
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
      } else {
        // Serial Number Column not wanted
      }
      for (let dataKeyNameIndex = 0; dataKeyNameIndex < dataKeyNames.length; dataKeyNameIndex++) {
        const head = dataKeyNames[dataKeyNameIndex];
        if (serialNumberColumn) {
          line += ',';
        } else {
          if (dataKeyNameIndex === 0) {
            // Serial Number Column not wanted
          } else {
            line += ',';
          }
        }
        line += JSON.stringify(array[i][head]);
      }
      str += line + '\r\n';
    }
    return str;
  }

  public exportToCSVFile(dataArray: Array<any>, filenameWithoutExtension = 'csv_data', headers = null, dataKeyNames = null, serialNumberColumn = true) {
    if (dataArray instanceof Array) {
      if (dataArray.length < 1) {
        throw new Error('data length must be > 0');
      }
    } else {
      throw new Error('data must be Array');
    }
    const csvData = this.arrayToCSV(dataArray, headers, dataKeyNames, serialNumberColumn);
    const blob = new Blob(['\ufeff' + csvData], {
      type: 'text/csv;charset=utf-8;',
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
  }

  /**
   *
   * @param inputDate date in typescript date datatype
   */
  public getMySQLDateFormat(inputDate: Date) {

    if (inputDate == null) {
      inputDate = new Date();
    }

    const result = inputDate.getFullYear() + '-' +
      ('00' + (inputDate.getMonth() + 1)).slice(-2) + '-' +
      ('00' + inputDate.getDate()).slice(-2) + ' ' +
      ('00' + inputDate.getHours()).slice(-2) + ':' +
      ('00' + inputDate.getMinutes()).slice(-2) + ':' +
      ('00' + inputDate.getSeconds()).slice(-2);

    // let result = inputDate.toISOString().slice(0, 19).replace('T', ' ');

    console.log('getMySQLDateFormat | input : ' + inputDate + ' | output : ' + result);

    return result;
  }

}
