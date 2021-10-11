import os

from FlaskApi import ZerodhaTraderStatic
from FlaskApi.App import flask_app
from FlaskApi.GlobalHelper import GlobalHelper
from IniConfiguration import IniConfiguration

if __name__ == '__main__':
    IniConfiguration.read_ini()
    ZerodhaTraderStatic.ZerodhaTraderStatic.instruments_df = GlobalHelper.read_instruments_csv(force=False)
    if not os.path.exists(IniConfiguration.get_value("system", "data_dir_path")):
        os.mkdir(path=IniConfiguration.get_value("system", "data_dir_path"))
    flask_app.run(host='0.0.0.0', port='4000', debug=True)
