from fastapi import FastAPI
from pydantic import BaseModel
from typing import Optional, List
from .agent import Agent

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

class AgentApi:
    def __init__(self):
        self.app = FastAPI()
        self.agent = Agent()
        self.register_routes()
        
    def register_routes(self):
        @self.app.post("/api/agent/generate_description")
        def generate_description(request: GenerateDescriptionInput) -> GenerateDescriptionOutput:
            response = self.agent.generate_description(request.project_name)
            return GenerateDescriptionOutput(project_description=response)

        @self.app.post("/api/agent/suggest_tasks")
        def suggest_tasks(request: SuggestTasksInput) -> SuggestTasksOutput:
            response = self.agent.suggest_tasks(request.project_name,request.project_description)
            return {"suggested_tasks": response}
        
        @self.app.post("/api/agent/refine_project_name")
        def refine_project_name(request: RefineProjectNameInput):
            response = self.agent.refine_project_name(request.project_name)
            return {"refined_project_name":response}