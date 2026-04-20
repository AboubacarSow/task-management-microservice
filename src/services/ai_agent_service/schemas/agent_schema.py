from pydantic import BaseModel
from typing import Optional, List

class GenerateDescriptionInput(BaseModel):
    project_name: str

class GenerateDescriptionOutput(BaseModel):
    project_description: str
    
class SuggestTasksInput(BaseModel):
    project_name: str
    project_description: Optional[str] = None
    
class SuggestTasksOutput(BaseModel):
    suggested_tasks: List[str]
    
class RefineProjectNameInput(BaseModel):
    project_name: str
    
class RefineProjectNameOutput(BaseModel):
    refined_project_name: str