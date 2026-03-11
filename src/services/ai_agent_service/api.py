from fastapi import FastAPI
from pydantic import BaseModel
from .agent import Agent

class GenerateDescriptionInput(BaseModel):
    project_name: str

class AgentApi:
    def __init__(self):
        self.app = FastAPI()
        self.agent = Agent()
        self.register_routes()
        
    def register_routes(self):
        @self.app.post("/agent/generate_description")
        def generate_description(request: GenerateDescriptionInput):
            response = self.agent.generate_description(request.project_name)
            return {"project_description":response}
        