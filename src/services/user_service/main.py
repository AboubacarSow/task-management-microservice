from services.user_service.api.user_router import UserRouter
from fastapi import FastAPI
import uvicorn

app = FastAPI()
user_router = UserRouter()
router = user_router.router
app.include_router(router)

if __name__ == "__main__":
    uvicorn.run(app)