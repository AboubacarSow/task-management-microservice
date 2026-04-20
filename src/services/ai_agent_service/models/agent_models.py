from pydantic import BaseModel, Field
from typing import Optional
from datetime import datetime


class LogResponse(BaseModel):
    user_id: str
    date: datetime = Field(default_factory=datetime.now)
    task_type: str
    project_name: str
    project_description: Optional[str] = None
    response: str | list[str]
    