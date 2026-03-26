from dotenv import load_dotenv

loaded = load_dotenv()

from .api.api import AgentApi
from .utils.logger import setup_logger
import uvicorn

setup_logger()

api_instance = AgentApi()
app = api_instance.app
if __name__ == "__main__":
    uvicorn.run(app)