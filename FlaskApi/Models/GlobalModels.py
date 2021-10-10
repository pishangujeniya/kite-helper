from typing import List


class KiteHelperCustomResponse:
    app_version: float
    error_code: int
    result: None
    error_messages: List[str]

    def __init__(self) -> None:
        self.app_version = 1.0
        self.error_code = 500
        self.result = None
        self.error_messages = []
