from fastapi import FastAPI
import uvicorn
from services.user_service.api.user_router import UserRouter

app = FastAPI()

user_router = UserRouter()
app.include_router(user_router.router)

if __name__ == "__main__":
    uvicorn.run("services.user_service.main:app", reload=True)