from pydantic import BaseModel, EmailStr
from typing import Optional

class User(BaseModel):
    id: Optional[str] = None
    first_name: str
    last_name: str
    email: EmailStr
    password: str