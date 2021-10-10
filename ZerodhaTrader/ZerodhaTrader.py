import os
import pickle

from CustomJugaadTrader.zerodha import Zerodha
from IniConfiguration import IniConfiguration


class ZerodhaTrader:
    def __init__(self) -> None:
        super().__init__()

    kite: Zerodha

    def kite_login(self, user_id: str, password: str, twofa: int):
        self.kite = Zerodha()
        self.kite.user_id = user_id
        self.kite.password = password
        self.kite.twofa = twofa
        j = self.kite.login_step1()
        if j['status'] == 'error':
            raise Exception(j['message'])
        j = self.kite.login_step2(j)
        if j['status'] == 'error':
            raise Exception(j['message'])
        self.kite.enc_token = self.kite.r.cookies['enctoken']
        with open(os.path.join(IniConfiguration.get_value("system", "data_dir_path"),
                               IniConfiguration.get_value("kite", "session_file")), "wb") as fp:
            pickle.dump(self.kite.reqsession, fp)

        self.kite.set_access_token()
        return True

    def get_profile(self):
        return self.kite.profile()
