from abc import ABC, abstractmethod
from ..models.user_model import User

class UserRepositoryInterface(ABC):

    @abstractmethod
    async def add_user(self, user: User):
        pass

    @abstractmethod
    async def get_user(self, user_id: str):
        pass

    @abstractmethod
    async def update_user(self, user_id: str, data: dict):
        pass

    @abstractmethod
    async def delete_user(self, user_id: str):
        pass
    
    @abstractmethod
    async def get_user_by_email(self, email: str):
        pass