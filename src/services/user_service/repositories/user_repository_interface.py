from abc import ABC, abstractmethod

class UserRepositoryInterface(ABC):

    @abstractmethod
    def add_user(self, user):
        pass

    @abstractmethod
    def get_user(self, user_id: str):
        pass

    @abstractmethod
    def update_user(self, user_id: str, data: dict):
        pass

    @abstractmethod
    def delete_user(self, user_id: str):
        pass
    
    @abstractmethod
    def get_user_by_email(self, email: str):
        pass