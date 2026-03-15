class UserService:
    def __init__(self, user_repository):
        self.user_repository = user_repository

    def add_user(self, user):
        return self.user_repository.add_user(user)