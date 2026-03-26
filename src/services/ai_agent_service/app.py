from dotenv import load_dotenv
import os

loaded = load_dotenv()
MONGO_URL = print(os.getenv("MONGO_URL"))
DB_NAME = print(os.getenv("DB_NAME"))
LOG_COLLECTION = print(os.getenv("LOG_COLLECTION"))

from .api.api import AgentApi
from .utils.logger import setup_logger
import uvicorn

setup_logger()

api_instance = AgentApi()
app = api_instance.app

if __name__ == "__main__":
    uvicorn.run(app)