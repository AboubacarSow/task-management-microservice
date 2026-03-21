from bson import ObjectId
from services.user_service.models.user_model import User
from services.user_service.repositories.user_repository_interface import UserRepositoryInterface
from datetime import datetime


class MongoUserRepository(UserRepositoryInterface):

    def __init__(self, collection):
        self.collection = collection

    def add_user(self, user) -> User:
        user_dict = user.model_dump(exclude={"id"})
        result = self.collection.insert_one(user_dict)
        return User(
        id=str(result.inserted_id),
        first_name=user.first_name,
        last_name=user.last_name,
        email=user.email,
        password=user.password,
        created_at=user.created_at,
        updated_at=user.updated_at,
        is_active=user.is_active
    )

    def get_user(self, user_id: str):
        data = self.collection.find_one({"_id": ObjectId(user_id)})

        if not data:
            return None

        return User(
            id=str(data["_id"]),
            first_name=data["first_name"],
            last_name=data["last_name"],
            email=data["email"],
            password=data["password"],
            created_at=data["created_at"],
            updated_at=data["updated_at"],
            is_active=data["is_active"]
        )

    def get_user_by_email(self, email: str):
        data = self.collection.find_one({"email": email})

        if not data:
            return None

        return User(
            id=str(data["_id"]),
            first_name=data["first_name"],
            last_name=data["last_name"],
            email=data["email"],
            password=data["password"],
            created_at=data["created_at"],
            updated_at=data["updated_at"],
            is_active=data["is_active"]
        )

    def update_user(self, user_id: str, data: dict):
        data["updated_at"] = datetime.now()
        result = self.collection.update_one(
            {"_id": ObjectId(user_id)},
            {"$set": data}
        )

        if result.matched_count == 0:
            return None

        return self.get_user(user_id)

    def delete_user(self, user_id: str):
        user = self.get_user(user_id)

        if not user:
            return None

        self.collection.delete_one({"_id": ObjectId(user_id)})
        return user