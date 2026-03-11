from fastapi import FastAPI
from pydantic import BaseModel
from .agent import Agent

class GenerateDescriptionInput(BaseModel):
    project_name: str

class GenerateDescriptionOutput(BaseModel):
    project_description: str

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