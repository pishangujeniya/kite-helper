import pandas as pd

from ZerodhaTrader.ZerodhaTrader import ZerodhaTrader


class ZerodhaTraderStatic:
    zerodha_trader: ZerodhaTrader = ZerodhaTrader()
    instruments_df: pd.DataFrame = None
