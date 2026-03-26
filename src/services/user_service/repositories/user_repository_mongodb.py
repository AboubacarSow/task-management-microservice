from ..models.user_model import User
from ..repositories.user_repository_interface import UserRepositoryInterface
from datetime import datetime


class MongoUserRepository(UserRepositoryInterface):

    def __init__(self, collection):
        self.collection = collection

    async def add_user(self, user) -> User:
        await self.collection.insert_one(user.model_dump())
        return user

    async def get_user(self, user_id: str):
        data = await self.collection.find_one({"id": user_id})

        if not data:
            return None

        return User(**data)

    async def get_user_by_email(self, email: str):
        data = await self.collection.find_one({"email": email})

        if not data:
            return None

        return User(**data)

    async def update_user(self, user_id: str, data: dict):
        data["updated_at"] = datetime.now()
        result = await self.collection.update_one(
            {"id": user_id},
            {"$set": data}
        )

        if result.matched_count == 0:
            return None

        return await self.get_user(user_id)

    async def delete_user(self, user_id: str):
        user = await self.get_user(user_id)

        if not user:
            return None

        await self.collection.delete_one({"id": user_id})
        return user