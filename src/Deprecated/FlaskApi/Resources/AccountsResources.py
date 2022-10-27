from flask import Response, json, request, jsonify
from flask_restx import Namespace, Resource, fields
from FlaskApi.GlobalHelper import GlobalHelper

from FlaskApi.Models.GlobalModels import KiteHelperCustomResponse
from FlaskApi.ZerodhaTraderStatic import ZerodhaTraderStatic as Zts
import jsonpickle

account_resources_api = Namespace('AccountsResources', description='Accounts Related API')


class LoginResponseModel(KiteHelperCustomResponse):
    def __init__(self) -> None:
        super().__init__()


@account_resources_api.route('/kite/login')
class KiteLogin(Resource):
    kite_login_request_model = account_resources_api.model('kite_login_request_model', {
        'username': fields.String(description='Username', required=True),
        'password': fields.String(description='Password', required=True),
        'pin': fields.Integer(description='two factor pin', required=True)
    })

    @account_resources_api.expect(kite_login_request_model)
    def post(self):
        login_response_model: LoginResponseModel = LoginResponseModel()

        try:
            is_logged_in = Zts.zerodha_trader.kite_login(
                user_id=request.json['username'],
                password=request.json['password'],
                twofa=request.json['pin'])

            if is_logged_in:
                login_response_model.result = Zts.zerodha_trader.kite.profile()
                login_response_model.error_code = 200

        except Exception as e:
            login_response_model.error_messages = [e.__str__()]

        flask_response = jsonify(login_response_model.__dict__)
        flask_response.status_code = login_response_model.error_code
        return flask_response


@account_resources_api.route('/kite/profile')
class KiteProfile(Resource):

    @account_resources_api.doc("Provides Profile Information of Logged in User")
    def get(self):
        p = Zts.zerodha_trader.kite.profile()
        return Response(response=json.dumps(p), status=200, mimetype="application/json")

@account_resources_api.route('/kite/update_instruments_csv')
class KiteProfile(Resource):

    @account_resources_api.doc("Updates Instruments CSV in Local System")
    def get(self):
        Zts.instruments_df = GlobalHelper.read_instruments_csv(force=True)
        return Response(response=json.dumps({"success":True}), status=200, mimetype="application/json")
