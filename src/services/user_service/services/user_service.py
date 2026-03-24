from services.user_service.models.user_model import User
from services.user_service.utils.password import hash_password, verify_password


class UserService:
    def __init__(self, user_repository):
        self.user_repository = user_repository

    async def add_user(self, user: User):
        existing_user = await self.user_repository.get_user_by_email(user.email)

        if existing_user:
            raise ValueError("User with this email already exists")
        
        user.password = hash_password(user.password)

        return await self.user_repository.add_user(user)
    
    async def get_user(self, user_id: str):
        user = await self.user_repository.get_user(user_id)
        
        if not user:
            raise ValueError("User with this id does not exist")
        
        return user
    
    async def update_user(self, user_id: str, data: dict):
        current_user = await self.user_repository.get_user(user_id)
        if not current_user:
            raise ValueError("User with this id does not exist")
        
        if "email" in data:
            existing_user = await self.user_repository.get_user_by_email(data["email"])
            if existing_user:
                raise FileExistsError("User with this email already exists")
            
        user = await self.user_repository.update_user(user_id, data)
        
        return user
    
    async def delete_user(self, user_id: str):
        user = await self.user_repository.delete_user(user_id)
        
        if not user:
            raise ValueError("User with this id does not exist")
        
        return user
    
    async def authenticate_user(self, email: str, password: str):
        user = await self.user_repository.get_user_by_email(email)

        if not user:
            return None

        if not verify_password(password, user.password):
            return None

        return user