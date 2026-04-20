from dotenv import load_dotenv

loaded = load_dotenv()

from .api.api import AgentApi
from .utils.logger import setup_logger
import uvicorn
from prometheus_fastapi_instrumentator import Instrumentator

setup_logger()

api_instance = AgentApi()
app = api_instance.app

@app.get("/health")
def health():
    return {"status": "ok"}

Instrumentator().instrument(app).expose(app, endpoint="/metrics")

if __name__ == "__main__":
    uvicorn.run(app)