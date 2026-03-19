from pydantic import BaseModel, EmailStr
from typing import Optional

class UserCreate(BaseModel):
    first_name: str
    last_name: str
    email: EmailStr
    password: str

class UserCreatedSuccesfully(BaseModel):
    id: str 
    first_name: str
    last_name: str
    email: EmailStr
    
class UserGet(BaseModel):
    id: str 
    first_name: str
    last_name: str
    email: EmailStr
    
class UserUpdate(BaseModel):
    first_name: Optional[str] = None
    last_name: Optional[str] = None
    email: Optional[EmailStr] = None
    
class UserUpdated(BaseModel):
    id: str
    first_name: str
    last_name: str
    email: EmailStr
    
class UserDeleted(BaseModel):
    id: str
    first_name: str
    last_name: str
    email: EmailStr