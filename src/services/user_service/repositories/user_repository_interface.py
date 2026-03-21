from abc import ABC, abstractmethod

class UserRepositoryInterface(ABC):

    @abstractmethod
    async def add_user(self, user):
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