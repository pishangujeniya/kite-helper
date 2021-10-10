import os

import pandas as pd
import requests

from IniConfiguration import IniConfiguration


class GlobalHelper:
    @staticmethod
    def read_instruments_csv() -> pd.DataFrame:
        instruments_csv_path = os.path.join(IniConfiguration.get_value("system", "data_dir_path"), "instruments.csv")
        if os.path.exists(instruments_csv_path) is False:
            r = requests.get("https://api.kite.trade/instruments", allow_redirects=True)
            open(instruments_csv_path, 'wb').write(r.content)
        return pd.read_csv(instruments_csv_path)
