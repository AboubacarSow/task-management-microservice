from .api.api import AgentApi
from .utils.logger import setup_logger
import uvicorn

setup_logger()

api_instance = AgentApi()

if __name__ == "__main__":
    uvicorn.run(api_instance.app)