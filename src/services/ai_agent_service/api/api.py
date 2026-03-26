from fastapi import FastAPI, Depends
from ..agents.agent import Agent
from ..schemas.agent_schema import (GenerateDescriptionInput, GenerateDescriptionOutput,SuggestTasksInput, 
                                    SuggestTasksOutput, RefineProjectNameInput, RefineProjectNameOutput)
from ..utils.jwt_handler import get_current_user
from ..database.mongo import log_collection
from ..repositories.agent_repository_mongodb import MongoAgentRepository
from ..models.agent_models import LogResponse
import logging

logger = logging.getLogger(__name__)

class AgentApi:
    def __init__(self):
        self.app = FastAPI()
        self.agent = Agent()
        self.register_routes()
        self.collection = log_collection
        self.repo = MongoAgentRepository(log_collection)
        
    
    def register_routes(self):
        @self.app.post("/api/agent/generate_description")
        def generate_description(request: GenerateDescriptionInput, payload: dict = Depends(get_current_user)) -> GenerateDescriptionOutput:
            logger.info(f"Generate description request received from user_id: {payload.get("id")}")

            response = self.agent.generate_description(request.project_name)
            
            self.repo.log_response(LogResponse(user_id=payload["id"], task_type="Generate Description",
                                               project_name=request.project_name, response=response))
            
            return GenerateDescriptionOutput(project_description=response)

        @self.app.post("/api/agent/suggest_tasks")
        def suggest_tasks(request: SuggestTasksInput, payload: dict = Depends(get_current_user)) -> SuggestTasksOutput:
            logger.info(f"Suggest tasks request received from user_id: {payload.get("id")}")
            
            response = self.agent.suggest_tasks(request.project_name,request.project_description)
            
            self.repo.log_response(LogResponse(user_id=payload["id"], task_type="Suggest Tasks", project_name=request.project_name,
                                               project_description=request.project_description, response=response))
            
            return {"suggested_tasks": response}
        
        @self.app.post("/api/agent/refine_project_name")
        def refine_project_name(request: RefineProjectNameInput, payload: dict = Depends(get_current_user)) -> RefineProjectNameOutput:
            logger.info(f"Refine project name request received from user_id: {payload.get("id")}")
            
            response = self.agent.refine_project_name(request.project_name)
            
            self.repo.log_response(LogResponse(user_id=payload["id"], task_type="Refine Project Name",
                                               project_name=request.project_name, response=response))
            
            return RefineProjectNameOutput(refined_project_name=response)