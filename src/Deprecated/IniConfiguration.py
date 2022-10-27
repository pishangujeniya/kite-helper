import configparser


class IniConfiguration:
    config: any

    @staticmethod
    def read_ini(path: str = "./config.ini"):
        IniConfiguration.config = configparser.ConfigParser()
        IniConfiguration.config.read(path)

    @staticmethod
    def get_value(section: str, key: str) -> str:
        return IniConfiguration.config[section][key]
