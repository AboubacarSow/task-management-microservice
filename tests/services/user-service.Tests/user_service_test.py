import pytest
import sys
import os
import uuid

sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '../../../src')))

from services.user_service.services.user_service import UserService
from services.user_service.models.user_model import User


class FakeUserRepository:
    def __init__(self):
        self.users = {}

    def add_user(self, user: User):
        if not hasattr(user, "id") or user.id is None:
            user.id = str(uuid.uuid4())
        self.users[user.id] = user
        return user

    def get_user(self, user_id: str):
        return self.users.get(user_id)

    def update_user(self, user_id: str, data: dict):
        user = self.users.get(user_id)
        if not user:
            return None

        for key, value in data.items():
            setattr(user, key, value)

        return user

    def delete_user(self, user_id: str):
        return self.users.pop(user_id, None)


def test_register_user():
    repo = FakeUserRepository()
    service = UserService(repo)

    user = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )

    result = service.add_user(user)

    assert result.email == "ali@test.com"
    assert len(repo.users) == 1