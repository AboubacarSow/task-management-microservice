import logging

def setup_logger():
    logger = logging.getLogger()
    logger.setLevel(logging.INFO)

    formatter = logging.Formatter(
        "[%(levelname)s] - %(asctime)s - %(name)s - %(message)s"
    )

    console_handler = logging.StreamHandler()  # stdout (Docker reads this)
    console_handler.setFormatter(formatter)

    logger.handlers.clear()  # avoids duplicate logs
    logger.addHandler(console_handler)