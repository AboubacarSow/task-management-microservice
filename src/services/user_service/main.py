from dotenv import load_dotenv

load_dotenv()

from fastapi import FastAPI
import uvicorn
from .api.user_router import UserRouter
from .utils.logger import setup_logger
from prometheus_fastapi_instrumentator import Instrumentator

setup_logger()

app = FastAPI()
user_router = UserRouter()
app.include_router(user_router.router)

@app.get("/health")
def health():
    return {"status": "ok"}

Instrumentator().instrument(app).expose(app, endpoint="/metrics")

if __name__ == "__main__":
    uvicorn.run("src.services.user_service.main:app", reload=True)