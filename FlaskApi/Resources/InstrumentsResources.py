from typing import List
import pandas as pd
from flask import Response, json, jsonify, request
from flask_restx import Namespace, Resource, fields
from dateutil import parser

from FlaskApi.Models.GlobalModels import KiteHelperCustomResponse
from FlaskApi.ZerodhaTraderStatic import ZerodhaTraderStatic as Zts

instruments_resources_api = Namespace('InstrumentsResources', description='Instruments Data Related API')


def listOfStringsToUpper(lst):
    for i in range(len(lst)):
        lst[i] = lst[i].upper()
    return lst

class GetInstrumentTradingSymbolResponseModel(KiteHelperCustomResponse):
    def __init__(self) -> None:
        super().__init__()


# Unused
class GetAllInstrumentsResponseModel:
    exchange: str
    exchange_token: str
    expiry: str
    instrument_token: str
    instrument_type: str
    last_price: float
    lot_size: int
    name: str
    segment: str
    strike: float
    tick_size: float
    tradingsymbol: str

    def __init__(self) -> None:
        super().__init__()

    def set(self, exchange: str, exchange_token: str):
        self.exchange = exchange
        self.exchange_token = exchange_token
        return self


@instruments_resources_api.route('/get_all_instruments')
class GetAllInstruments(Resource):
    get_requirements_request_model = instruments_resources_api.model('get_requirements_request_model', {
        'exchange': fields.String(description='exchange name', required=False),
    })

    @instruments_resources_api.expect(get_requirements_request_model)
    def post(self):
        return Response(response=json.dumps(Zts.zerodha_trader.kite.instruments(exchange=request.json['exchange'])),
                        status=200)


@instruments_resources_api.route('/get_all_instruments_1')
class GetAllInstruments1(Resource):
    get_requirements_request_model = instruments_resources_api.model('get_requirements_request_model', {
        'name': fields.List(description='name', required=False, cls_or_instance=fields.String),
        'exchange': fields.List(description='exchange name', required=False, cls_or_instance=fields.String),
        'segment': fields.List(description='segment name', required=False, cls_or_instance=fields.String),
        'instrument_type': fields.List(description='instrument_type', required=False, cls_or_instance=fields.String),
    })

    @instruments_resources_api.expect(get_requirements_request_model)
    def post(self):
        kh_response: KiteHelperCustomResponse = KiteHelperCustomResponse()
        try:

            required_exchanges: list = request.json['exchange']
            required_names: list = request.json['name']
            required_segments: list = request.json['segment']
            required_instrument_type: list = request.json['instrument_type']

            required_exchanges = listOfStringsToUpper(required_exchanges)
            required_names = listOfStringsToUpper(required_names)
            required_segments = listOfStringsToUpper(required_segments)
            required_instrument_type = listOfStringsToUpper(required_instrument_type)

            instruments = pd.read_csv('https://api.kite.trade/instruments')
            # to handle empty strings
            if len(required_exchanges[0]):
                instruments = instruments[ instruments['exchange'].isin(required_exchanges)]
            if len(required_names[0]):
                instruments = instruments[instruments['name'].isin(required_names)]
            if len(required_segments[0]):
                instruments = instruments[instruments['segment'].isin(required_segments)]
            if len(required_instrument_type[0]):
                instruments = instruments[instruments['instrument_type'].isin(required_instrument_type)]

            instruments.fillna('',inplace=True)
            kh_response.result = instruments.to_dict(orient='records')
            kh_response.error_code = 200

        except Exception as e:
            kh_response.error_messages = [e.__str__()]

        flask_response = jsonify(kh_response.__dict__)
        flask_response.status_code = kh_response.error_code

        return flask_response


@instruments_resources_api.route('/get_historical_data')
class GetHistoricalData(Resource):
    get_historical_data_request_model = instruments_resources_api.model('get_historical_data_request_model', {
        'exchange': fields.String(description='Exchange', required=True),
        'trading_symbol': fields.String(description='trading_symbol for example RELIANCE', required=True),
        'start_dtm': fields.String(description='Start DateTime yyyy-mm-dd hh:mm:ss IST', required=True),
        'end_dtm': fields.String(description='End DateTime yyyy-mm-dd hh:mm:ss IST', required=True),
        'interval': fields.String(description='interval', enum=['minute', '5minute','10minute','15minute','day'])
    })

    @instruments_resources_api.expect(get_historical_data_request_model)
    def post(self):
        instrument_token = \
            Zts.instruments_df[(Zts.instruments_df['tradingsymbol'] == request.json['trading_symbol']) & (Zts.instruments_df['exchange'] == request.json['exchange'])][
                'instrument_token']
        instrument_token = instrument_token.to_list()
        print('-----',instrument_token)
        if not len(instrument_token):
            hd = []
        else:
            instrument_token = instrument_token[0]
            start_dtm = parser.parse(request.json['start_dtm'])
            end_dtm = parser.parse(request.json['end_dtm'])
            interval = request.json['interval']
            hd = Zts.zerodha_trader.kite.historical_data(instrument_token, from_date=start_dtm, to_date=end_dtm,
                                                        interval=interval)
        return Response(response=json.dumps(hd),
                        status=200)


@instruments_resources_api.route('/get_instrument_trading_symbol')
class GetInstrumentTradingSymbol(Resource):
    get_instrument_trading_symbol_request_model = instruments_resources_api.model(
        'get_instrument_trading_symbol_request_model', {
            'symbol': fields.String(description='symbol name eg REL', required=True),
        })

    @instruments_resources_api.expect(get_instrument_trading_symbol_request_model)
    def post(self):
        get_instrument_trading_symbol_response_model: GetInstrumentTradingSymbolResponseModel = GetInstrumentTradingSymbolResponseModel()

        try:
            get_instrument_trading_symbol_response_model.result = list(
                Zts.instruments_df[
                    Zts.instruments_df['tradingsymbol'].str.contains(str(request.json['symbol']).upper(), na=False)][
                    'tradingsymbol'])
            get_instrument_trading_symbol_response_model.error_code = 200

        except Exception as e:
            get_instrument_trading_symbol_response_model.error_messages = [e.__str__()]

        flask_response = jsonify(get_instrument_trading_symbol_response_model.__dict__)
        flask_response.status_code = get_instrument_trading_symbol_response_model.error_code
        return flask_response
