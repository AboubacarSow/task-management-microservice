<<<<<<< HEAD
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

    async def add_user(self, user: User):
        if not hasattr(user, "id") or user.id is None:
            user.id = str(uuid.uuid4())
        self.users[user.id] = user
        return user

    async def get_user(self, user_id: str):
        return self.users.get(user_id)

    async def update_user(self, user_id: str, data: dict):
        user = self.users.get(user_id)
        if not user:
            return None

        for key, value in data.items():
            setattr(user, key, value)

        return user

    async def delete_user(self, user_id: str):
        return self.users.pop(user_id, None)
    
    async def get_user_by_email(self, email: str):
        for user in self.users.values():
            if user.email == email:
                return user
        return None

@pytest.mark.asyncio
async def test_register_user():
    repo = FakeUserRepository()
    service = UserService(repo)

    user = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )

    result = await service.add_user(user)

    assert result.email == "ali@test.com"
    assert len(repo.users) == 1

@pytest.mark.asyncio
async def test_duplicate_email():
    repo = FakeUserRepository()
    service = UserService(repo)

    user1 = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )

    user2 = User(
        first_name="Ahmet",
        last_name="Yilmaz",
        email="ali@test.com",   # same email
        password="abcdef"
    )

    await service.add_user(user1)

    with pytest.raises(ValueError, match="User with this email already exists"):
        await service.add_user(user2)
  
@pytest.mark.asyncio      
async def test_get_user():
    repo = FakeUserRepository()
    service = UserService(repo)

    user = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )

    await service.add_user(user)

    retrieved_user = await service.get_user(user.id)

    assert isinstance(retrieved_user, User)
    assert retrieved_user.id == user.id
 
@pytest.mark.asyncio   
async def test_get_user_not_found():
    repo = FakeUserRepository()
    service = UserService(repo)

    with pytest.raises(ValueError, match="User with this id does not exist"):
        await service.get_user("non-existing-id")
        
@pytest.mark.asyncio   
async def test_update_user():
    repo = FakeUserRepository()
    service = UserService(repo)

    user = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )

    await service.add_user(user)

    user_dict = {
        "first_name": "Ahmet",
        "last_name": "Yilmaz"
    }

    updated_user = await service.update_user(user.id, user_dict)

    assert isinstance(updated_user, User)
    assert updated_user.id == user.id
    assert updated_user.first_name == "Ahmet"
    assert updated_user.last_name == "Yilmaz"
 
@pytest.mark.asyncio   
async def test_update_user_not_found():
    repo = FakeUserRepository()
    service = UserService(repo)

    user_id = str(uuid.uuid4())
    user_dict = {
        "first_name": "Ahmet",
        "last_name": "Yilmaz"
    }
    
    with pytest.raises(ValueError, match="User with this id does not exist"):
        await service.update_user(user_id, user_dict)
  
@pytest.mark.asyncio      
async def test_delete_user():
    repo = FakeUserRepository()
    service = UserService(repo)

    user = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )

    await service.add_user(user)

    deleted_user = await service.delete_user(user.id)
    assert deleted_user is None
 
@pytest.mark.asyncio   
async def test_delete_user_id_not_found():
    repo = FakeUserRepository()
    service = UserService(repo)

    user_id = str(uuid.uuid4())
    
    with pytest.raises(ValueError, match="User with this id does not exist"):
        await service.delete_user(user_id)
        
@pytest.mark.asyncio
async def test_authenticate_user():
    repo = FakeUserRepository()
    service = UserService(repo)
    
    user = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )
    
    await service.add_user(user)
    
    validated_user = await service.authenticate_user("ali@test.com", "123456")
    assert isinstance(validated_user, User)
    assert validated_user.id == user.id
    
@pytest.mark.asyncio
async def test_authenticate_user_user_not_found():
    repo = FakeUserRepository()
    service = UserService(repo)
    
    validated_user = await service.authenticate_user("ali@test.com", "123456")
=======
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

    async def add_user(self, user: User):
        if not hasattr(user, "id") or user.id is None:
            user.id = str(uuid.uuid4())
        self.users[user.id] = user
        return user

    async def get_user(self, user_id: str):
        return self.users.get(user_id)

    async def update_user(self, user_id: str, data: dict):
        user = self.users.get(user_id)
        if not user:
            return None

        for key, value in data.items():
            setattr(user, key, value)

        return user

    async def delete_user(self, user_id: str):
        return self.users.pop(user_id, None)
    
    async def get_user_by_email(self, email: str):
        for user in self.users.values():
            if user.email == email:
                return user
        return None

@pytest.mark.asyncio
async def test_register_user():
    repo = FakeUserRepository()
    service = UserService(repo)

    user = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )

    result = await service.add_user(user)

    assert result.email == "ali@test.com"
    assert len(repo.users) == 1

@pytest.mark.asyncio
async def test_duplicate_email():
    repo = FakeUserRepository()
    service = UserService(repo)

    user1 = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )

    user2 = User(
        first_name="Ahmet",
        last_name="Yilmaz",
        email="ali@test.com",   # same email
        password="abcdef"
    )

    await service.add_user(user1)

    with pytest.raises(ValueError, match="User with this email already exists"):
        await service.add_user(user2)
  
@pytest.mark.asyncio      
async def test_get_user():
    repo = FakeUserRepository()
    service = UserService(repo)

    user = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )

    await service.add_user(user)

    retrieved_user = await service.get_user(user.id)

    assert isinstance(retrieved_user, User)
    assert retrieved_user.id == user.id
 
@pytest.mark.asyncio   
async def test_get_user_not_found():
    repo = FakeUserRepository()
    service = UserService(repo)

    with pytest.raises(ValueError, match="User with this id does not exist"):
        await service.get_user("non-existing-id")
        
@pytest.mark.asyncio   
async def test_update_user():
    repo = FakeUserRepository()
    service = UserService(repo)

    user = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )

    await service.add_user(user)

    user_dict = {
        "first_name": "Ahmet",
        "last_name": "Yilmaz"
    }

    updated_user = await service.update_user(user.id, user_dict)

    assert isinstance(updated_user, User)
    assert updated_user.id == user.id
    assert updated_user.first_name == "Ahmet"
    assert updated_user.last_name == "Yilmaz"
 
@pytest.mark.asyncio   
async def test_update_user_not_found():
    repo = FakeUserRepository()
    service = UserService(repo)

    user_id = str(uuid.uuid4())
    user_dict = {
        "first_name": "Ahmet",
        "last_name": "Yilmaz"
    }
    
    with pytest.raises(ValueError, match="User with this id does not exist"):
        await service.update_user(user_id, user_dict)
  
@pytest.mark.asyncio      
async def test_delete_user():
    repo = FakeUserRepository()
    service = UserService(repo)

    user = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )

    await service.add_user(user)

    deleted_user = await service.delete_user(user.id)
    assert deleted_user is None
 
@pytest.mark.asyncio   
async def test_delete_user_id_not_found():
    repo = FakeUserRepository()
    service = UserService(repo)

    user_id = str(uuid.uuid4())
    
    with pytest.raises(ValueError, match="User with this id does not exist"):
        await service.delete_user(user_id)
        
@pytest.mark.asyncio
async def test_authenticate_user():
    repo = FakeUserRepository()
    service = UserService(repo)
    
    user = User(
        first_name="Ali",
        last_name="Khan",
        email="ali@test.com",
        password="123456"
    )
    
    await service.add_user(user)
    
    validated_user = await service.authenticate_user("ali@test.com", "123456")
    assert isinstance(validated_user, User)
    assert validated_user.id == user.id
    
@pytest.mark.asyncio
async def test_authenticate_user_user_not_found():
    repo = FakeUserRepository()
    service = UserService(repo)
    
    validated_user = await service.authenticate_user("ali@test.com", "123456")
>>>>>>> 05b451b (new_update)
    assert validated_user is None