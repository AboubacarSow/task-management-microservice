from services.user_service.services.user_service import UserService
from services.user_service.models.user_model import User
from services.user_service.schemas.api_schemas import (UserCreate, UserCreatedSuccesfully, UserGet, 
                                                       UserUpdate, UserUpdated, UserDeleted, UserActive)
from services.user_service.database.mongo import user_collection
from services.user_service.repositories.user_repository_mongodb import MongoUserRepository
from fastapi import APIRouter, HTTPException, Depends


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
                return await service.add_user(user)
            except ValueError as e:
                raise HTTPException(status_code=409, detail=str(e))
            
        @self.router.get("/{user_id}")
        async def get_user(user_id: str, service: UserService = Depends(self.get_user_service))-> UserGet:
            try:
                return await service.get_user(user_id)
            except ValueError as e:
                raise HTTPException(status_code=404, detail=str(e))
            
        @self.router.put("/{user_id}")
        async def update_user(user_id: str, data: UserUpdate, service: UserService = Depends(self.get_user_service))-> UserUpdated:
            try:
                return await service.update_user(user_id,data.model_dump(exclude_unset=True))
            except ValueError as e:
                raise HTTPException(status_code=404, detail=str(e))
            except FileExistsError as e:
                raise HTTPException(status_code=409, detail=str(e))
            
        @self.router.delete("/{user_id}")
        async def delete_user(user_id: str, service: UserService = Depends(self.get_user_service))-> UserDeleted:
            try:
                return await service.delete_user(user_id)
            except ValueError as e:
                raise HTTPException(status_code=404, detail=str(e))
            
        @self.router.get("/{user_id}/active")
        async def is_user_active(user_id: str, service: UserService = Depends(self.get_user_service))-> UserActive:
            try:
                return await service.get_user(user_id)
            except ValueError as e:
                raise HTTPException(status_code=404, detail=str(e))