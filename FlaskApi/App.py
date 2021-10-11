from flask import Flask
from flask_restx import Api

from FlaskApi.Resources.AccountsResources import account_resources_api
from FlaskApi.Resources.InstrumentsResources import instruments_resources_api
from flask_cors import CORS

flask_app = Flask(__name__)
CORS(flask_app)
api = Api(app=flask_app,
          version="1.0",
          title="Kite Helper",
          description="Kite related API")

api.add_namespace(ns=account_resources_api, path="/accounts")
api.add_namespace(ns=instruments_resources_api, path="/instruments")
