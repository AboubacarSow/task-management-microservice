from ..services.user_service import UserService
from ..models.user_model import User
from ..schemas.api_schemas import (UserCreate, UserCreatedSuccesfully, UserGet, UserUpdate,
                                   UserUpdated, UserActive, UserAuth, UserAuthed)
from ..database.mongo import user_collection
from ..repositories.user_repository_mongodb import MongoUserRepository
from fastapi import APIRouter, HTTPException, Depends
from ..utils.jwt_handler import get_current_user
import logging

logger = logging.getLogger(__name__)

class UserRouter:
    def __init__(self):
        self.router = APIRouter(prefix="/api/users", tags=["Users"])
        self.register_routes()

    def get_user_service(self):
        repo = MongoUserRepository(user_collection)
        return UserService(repo)
    
    def register_routes(self):
        @self.router.post("/")
        async def create_user(user: UserCreate, service: UserService = Depends(self.get_user_service))-> UserCreatedSuccesfully:
            try:
                user = User(**user.model_dump())
                logger.info(f"Create user request received for email={user.email}")
                return await service.add_user(user)
            
            except ValueError as e:
                raise HTTPException(status_code=409, detail=str(e))
            
        @self.router.get("/{user_id}")
        async def get_user(user_id: str, service: UserService = Depends(self.get_user_service),
                              payload: dict = Depends(get_current_user))-> UserGet:
            
            if payload.get("id") != user_id:
                raise HTTPException(status_code=403, detail="Forbidden")
            
            try:
                logger.info(f"Get user request received for user_id={user_id}")
                return await service.get_user(user_id)
            
            except ValueError as e:
                raise HTTPException(status_code=404, detail=str(e))
            
        @self.router.put("/{user_id}")
        async def update_user(user_id: str, data: UserUpdate, service: UserService = Depends(self.get_user_service),
                              payload: dict = Depends(get_current_user))-> UserUpdated:
            
            if payload.get("id") != user_id:
                raise HTTPException(status_code=403, detail="Forbidden")
            
            try:
                logger.info(f"Update user request received for user_id={user_id}")
                return await service.update_user(user_id,data.model_dump(exclude_unset=True))
            
            except ValueError as e:
                raise HTTPException(status_code=404, detail=str(e))
            except FileExistsError as e:
                raise HTTPException(status_code=409, detail=str(e))
            
        @self.router.delete("/{user_id}", status_code=204)
        async def delete_user(user_id: str, service: UserService = Depends(self.get_user_service),
                              payload: dict = Depends(get_current_user)):
            
            if payload.get("id") != user_id:
                raise HTTPException(status_code=403, detail="Forbidden")
            
            try:
                logger.info(f"Delete user request received for user_id={user_id}")
                await service.delete_user(user_id)
                
            except ValueError as e:
                raise HTTPException(status_code=404, detail=str(e))
            
        @self.router.get("/{user_id}/active")
        async def is_user_active(user_id: str, service: UserService = Depends(self.get_user_service),
                              payload: dict = Depends(get_current_user))-> UserActive:
            
            if payload.get("id") != user_id:
                raise HTTPException(status_code=403, detail="Forbidden")
            
            try:
                logger.info(f"Is user active request received for user_id={user_id}")
                return await service.get_user(user_id)
            
            except ValueError as e:
                raise HTTPException(status_code=404, detail=str(e))
            
        @self.router.post("/validate")
        async def authenticate_user(data: UserAuth, service: UserService = Depends(self.get_user_service))-> UserAuthed:
            logger.info(f"Authenticate request received for email={data.email}")
            user = await service.authenticate_user(data.email,data.password)
            
            if not user:
                logger.warning(f"Authentication failed for email={data.email}")
                raise HTTPException(status_code=401, detail="Invalid email or password")
            
            logger.info(f"Authentication successful for user_id={user.id}")
            return user
