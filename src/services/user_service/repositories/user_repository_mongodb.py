from bson import ObjectId
from services.user_service.models.user_model import User
from services.user_service.repositories.user_repository_interface import UserRepositoryInterface


class MongoUserRepository(UserRepositoryInterface):

    def __init__(self, collection):
        self.collection = collection

    def add_user(self, user: User):
        user_dict = user.model_dump(exclude={"id"})
        result = self.collection.insert_one(user_dict)
        user.id = str(result.inserted_id)
        return user

    def get_user(self, user_id: str):
        data = self.collection.find_one({"_id": ObjectId(user_id)})

        if not data:
            return None

        return User(
            id=str(data["_id"]),
            first_name=data["first_name"],
            last_name=data["last_name"],
            email=data["email"],
            password=data["password"]
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
            password=data["password"]
        )

    def update_user(self, user_id: str, data: dict):
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