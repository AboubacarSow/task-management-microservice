from pydantic import BaseModel, EmailStr, Field
from typing import Optional
from datetime import datetime

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
    created_at: datetime
    updated_at: datetime
    
class UserGet(BaseModel):
    id: str 
    first_name: str
    last_name: str
    email: EmailStr
    created_at: datetime
    updated_at: datetime
    
class UserUpdate(BaseModel):
    first_name: Optional[str] = None
    last_name: Optional[str] = None
    email: Optional[EmailStr] = None
    
class UserUpdated(BaseModel):
    id: str
    first_name: str
    last_name: str
    email: EmailStr
    created_at: datetime
    updated_at: datetime
    
class UserDeleted(BaseModel):
    id: str
    first_name: str
    last_name: str
    email: EmailStr
    created_at: datetime
    updated_at: datetime
    
class UserActive(BaseModel):
    id: str
    is_active: bool
    
class UserAuth(BaseModel):
    email: EmailStr
    password: str
    
class UserAuthed(BaseModel):
    id: str
    first_name: str
    last_name: str
    email: EmailStr
    created_at: datetime
    updated_at: datetime