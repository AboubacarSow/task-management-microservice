from fastapi import FastAPI, Depends
from ..agents.agent import Agent
from ..schemas.agent_schema import (GenerateDescriptionInput, GenerateDescriptionOutput,SuggestTasksInput, 
                                    SuggestTasksOutput, RefineProjectNameInput, RefineProjectNameOutput)
from ..utils.jwt_handler import get_current_user
import logging

logger = logging.getLogger(__name__)

class AgentApi:
    def __init__(self):
        self.app = FastAPI()
        self.agent = Agent()
        self.register_routes()
        
    
    def register_routes(self):
        @self.app.post("/api/agent/generate_description")
        def generate_description(request: GenerateDescriptionInput, payload: dict = Depends(get_current_user)) -> GenerateDescriptionOutput:
            logger.info(f"Generate description request received from user_id: {payload.get("id")}")

            response = self.agent.generate_description(request.project_name)
            return GenerateDescriptionOutput(project_description=response)

        @self.app.post("/api/agent/suggest_tasks")
        def suggest_tasks(request: SuggestTasksInput, payload: dict = Depends(get_current_user)) -> SuggestTasksOutput:
            logger.info(f"Suggest tasks request received from user_id: {payload.get("id")}")
            
            response = self.agent.suggest_tasks(request.project_name,request.project_description)
            return {"suggested_tasks": response}
        
        @self.app.post("/api/agent/refine_project_name")
        def refine_project_name(request: RefineProjectNameInput, payload: dict = Depends(get_current_user)) -> RefineProjectNameOutput:
            logger.info(f"Refine project name request received from user_id: {payload.get("id")}")
            
            response = self.agent.refine_project_name(request.project_name)
            return RefineProjectNameOutput(refined_project_name=response)