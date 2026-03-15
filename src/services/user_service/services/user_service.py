from services.user_service.models.user_model import User
class UserService:
    def __init__(self, user_repository):
        self.user_repository = user_repository

    def add_user(self, user: User):
        existing_user = self.user_repository.get_user_by_email(user.email)

        if existing_user:
            raise ValueError("User with this email already exists")

        return self.user_repository.add_user(user)
    
    def get_user(self, user_id: str):
        user = self.user_repository.get_user(user_id)
        
        if not user:
            raise ValueError("User with this id does not exsit")
        
        return user
    
    def update_user(self, user_id: str, data: dict):
        user = self.user_repository.update_user(user_id, data)
        if not user:
            raise ValueError("User with this id does not exsit")
        
        return user
    
    def delete_user(self, user_id: str):
        return self.user_repository.delete_user(user_id)