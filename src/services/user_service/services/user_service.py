from ..models.user_model import User
from ..utils.password import hash_password, verify_password
import uuid
import logging

logger = logging.getLogger(__name__)

class UserService:
    def __init__(self, user_repository):
        self.user_repository = user_repository

    async def add_user(self, user: User):
        existing_user = await self.user_repository.get_user_by_email(user.email)

        if existing_user:
            logger.warning(f"Duplicate email detected: email={user.email}")
            raise ValueError("User with this email already exists")
        user.id = str(uuid.uuid4())        
        user.password = hash_password(user.password)

        created_user = await self.user_repository.add_user(user)
        logger.info(f"User created successfully: user_id={created_user.id}")
        return created_user
    
    async def get_user(self, user_id: str):
        user = await self.user_repository.get_user(user_id)
        
        if not user:
            logger.warning(f"User not found: user_id={user_id}")
            raise ValueError("User with this id does not exist")
        
        logger.info(f"User retrieved successfully: user_id={user_id}")
        return user
    
    async def update_user(self, user_id: str, data: dict):
        current_user = await self.user_repository.get_user(user_id)
        if not current_user:
            logger.warning(f"User not found: user_id={user_id}")
            raise ValueError("User with this id does not exist")
        
        if "email" in data:
            existing_user = await self.user_repository.get_user_by_email(data["email"])
            if existing_user and existing_user.id != user_id:
                logger.warning(f"Duplicate email detected: email={data["email"]}")
                raise FileExistsError("User with this email already exists")
            
        user = await self.user_repository.update_user(user_id, data)
        logger.info(f"User updated successfully: user_id={user_id}")
        
        return user
    
    async def delete_user(self, user_id: str):
        user = await self.user_repository.delete_user(user_id)
        
        if not user:
            logger.warning(f"User not found: user_id={user_id}")
            raise ValueError("User with this id does not exist")
        logger.info(f"User deleted successfully: user_id={user_id}")
    
    async def authenticate_user(self, email: str, password: str):
        user = await self.user_repository.get_user_by_email(email)

        if not user:
            return None

        if not verify_password(password, user.password):
            return None

        return user