from pydantic import BaseModel, EmailStr

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