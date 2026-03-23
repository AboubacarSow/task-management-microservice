import pytest
from fastapi.testclient import TestClient
from httpx import AsyncClient, ASGITransport
import sys
import os
import uuid

sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '../../../src')))
from services.user_service.models.user_model import User
from services.user_service.main import app, user_router
from services.user_service.services.user_service import UserService

class FakeUserRepository:
    def __init__(self):
        self.users = {}

    async def add_user(self, user):
        new_user = User(
            id=str(uuid.uuid4()),
            first_name=user.first_name,
            last_name=user.last_name,
            email=user.email,
            password=user.password
        )
        self.users[new_user.id] = new_user
        return new_user

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
    
repo = FakeUserRepository()
service = UserService(repo)
app.dependency_overrides[user_router.get_user_service] = lambda: service
transport = ASGITransport(app=app)

@pytest.mark.asyncio
async def test_add_new_user():
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response = await client.post("/api/users/", json={
            "first_name": "Ali",
            "last_name": "Veli",
            "email": "ali@test.com",
            "password": "123456"
        })
    
    data = response.json()
    assert response.status_code == 200
    assert "id" in data
    assert data["first_name"] == "Ali"
    assert data["email"] == "ali@test.com"

@pytest.mark.asyncio 
async def test_add_new_user_duplicate_email():
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response1 = await client.post("/api/users/", json={
            "first_name": "Ali",
            "last_name": "Veli",
            "email": "ali@test1.com",
            "password": "123456"
        })

    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response2 = await client.post("/api/users/", json={
            "first_name": "Ali",
            "last_name": "Veli",
            "email": "ali@test1.com",
            "password": "123456"
        })

    data1 = response1.json()
    data2 = response2.json()

    assert response1.status_code == 200
    assert response2.status_code == 409

    assert "id" in data1
    assert data1["email"] == "ali@test1.com"

    assert "detail" in data2
    assert data2["detail"] == "User with this email already exists"
  
@pytest.mark.asyncio  
async def test_get_user():
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response1 = await client.post("/api/users/", json={
            "first_name": "Ali",
            "last_name": "Veli",
            "email": "ali@test2.com",
            "password": "123456"
        })


    data1 = response1.json()
    assert response1.status_code == 200
    assert "id" in data1
    user_id = data1["id"]
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response2 = await client.get(f"/api/users/{user_id}")
    data2 = response2.json()
    assert response2.status_code == 200
    assert data1["id"] == data2["id"]
    assert data2["email"] == "ali@test2.com"
 
@pytest.mark.asyncio
async def test_get_user_not_exist():
    user_id = str(uuid.uuid4())
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response = await client.get(f"/api/users/{user_id}")
    
    data = response.json()
    assert response.status_code == 404
    assert "detail" in data
    assert data["detail"] == "User with this id does not exist"

@pytest.mark.asyncio   
async def test_update_user():
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response1 = await client.post("/api/users/", json={
            "first_name": "Ali",
            "last_name": "Veli",
            "email": "ali@tes.com",
            "password": "123456"
        })
    data1 = response1.json()
    
    assert response1.status_code == 200
    assert "id" in data1
    
    user_id = data1["id"]
    
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response2 = await client.put(f"/api/users/{user_id}", json={
            "first_name":"Updated",
            "email":"data@updated.com"
        })
    
    data2 = response2.json()
    
    assert response2.status_code == 200
    assert "id" in data2
    assert "first_name" in data2
    assert data2["first_name"] == "Updated"
    assert "last_name" in  data2
    assert data2["last_name"] == "Veli"
    assert "email" in data2
    assert data2["email"] == "data@updated.com"

@pytest.mark.asyncio 
async def test_update_user_not_exsit():
    user_id = str(uuid.uuid4())
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response = await client.put(f"/api/users/{user_id}", json={
            "first_name":"Updated",
            "email":"data@updated.com"
        })
    
    data = response.json()
    
    assert response.status_code == 404
    assert "detail" in data
    assert data["detail"] == "User with this id does not exist"
 
@pytest.mark.asyncio   
async def test_delete_user():
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response1 = await client.post("/api/users/", json={
            "first_name": "Ali",
            "last_name": "Veli",
            "email": "ali@tes1.com",
            "password": "123456"
        })
    data1 = response1.json()
    
    assert response1.status_code == 200
    assert "id" in data1
    
    user_id = data1["id"]
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response2 = await client.delete(f"/api/users/{user_id}")
    
    data2 = response2.json()
    assert response2.status_code == 200
    assert "id" in data2
    assert data2["id"] == user_id

@pytest.mark.asyncio 
async def test_delete_user_not_exist():
    user_id = str(uuid.uuid4())
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response = await client.delete(f"/api/users/{user_id}")
    
    data = response.json()
    assert response.status_code == 404
    assert "detail" in data
    assert data["detail"] == "User with this id does not exist"

@pytest.mark.asyncio
async def test_is_user_active():
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response1 = await client.post("/api/users/", json={
            "first_name": "Ali",
            "last_name": "Veli",
            "email": "ali@te.com",
            "password": "123456"
        })
    data1 = response1.json()
    assert response1.status_code == 200
    assert "id" in data1
    user_id = data1["id"]
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response2 = await client.get(f"/api/users/{user_id}/active")
    data2 = response2.json()
    assert "is_active" in data2
    assert data2["is_active"] == True
 
@pytest.mark.asyncio   
async def test_is_user_active_user_not_found():
    user_id = str(uuid.uuid4())
    
    async with AsyncClient(transport=transport, base_url="http://test") as client:
        response = await client.get(f"/api/users/{user_id}/active")
    
    data = response.json()
    assert response.status_code == 404
    assert "detail" in data
    assert data["detail"] == "User with this id does not exist"