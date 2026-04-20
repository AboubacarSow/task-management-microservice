from langchain_ollama import ChatOllama
from langchain_core.prompts import ChatPromptTemplate
from typing import List, Optional
from pydantic import BaseModel, Field
import logging
import os

logger = logging.getLogger(__name__)

class Agent:
    def __init__(self):
        model = os.getenv("OLLAMA_MODEL")
        base_url = os.getenv("OLLAMA_BASE_URL")
        self.llm = ChatOllama(model=model, base_url=base_url)
        logger.info(f"Agent initialized with model: {model}")
        
    def generate_description(self, project_name: str) -> str:
        logger.info(f"Generating description for project: {project_name}")
        prompt = ChatPromptTemplate.from_messages([
            ("system",
             """You are a professional project description writer.
             Your job is to write a one paragraph sutible, relevant and supported porject description based on the project name.
             Don't output anything else just the generated paragraph.
             """),
            ("human",
             """Project name: {project_name}
             """)
        ])
        
        generate_description_chain = prompt | self.llm
        response = generate_description_chain.invoke({"project_name":project_name})
        logger.info("Description generated successfully")
        return response.content
    
    def suggest_tasks(self, project_name: str, project_description: Optional[str]=None) -> List[str]:
        
        logger.info(f"Suggesting tasks for project: {project_name}")
        
        class TasksOutput(BaseModel):
            tasks : List[str] = Field(min_length=5, description="List of suggested tasks.")
        project_name_prompt = ChatPromptTemplate.from_messages([
            ("system",
             """Your are a professional project manager.
             Your job is to suggest a list of 5 important tasks based on the project name.
             Don't add anything extra.
             """),
            ("human",
             """Project name: {project_name}
             """)
        ])
        project_description_prompt = ChatPromptTemplate.from_messages([
            ("system",
             """Your are a professional project manager.
             Your job is to suggest a list of 5 important tasks based on the project name and description.
             Don't add anything extra.
             """),
            ("human",
             """Project Name: {project_name}
                Project Description: {project_description}
             """)
        ])
        if project_description:
            logger.info("Using project description for task generation")
            suggest_tasks_chain = project_description_prompt | self.llm.with_structured_output(TasksOutput)
            response = suggest_tasks_chain.invoke({"project_name":project_name, "project_description":project_description})
        else:
            logger.info("Using only project name for task generation")
            suggest_tasks_chain = project_name_prompt | self.llm.with_structured_output(TasksOutput)
            response = suggest_tasks_chain.invoke({"project_name":project_name})
        
        logger.info(f"Generated {len(response.tasks)} tasks successfully")
        return response.tasks
    
    def refine_project_name(self, project_name: str) -> str:
        logger.info(f"Refining project name: {project_name}")
        prompt = ChatPromptTemplate.from_messages([
            ("system",
            """You are a professional text refiner agent.
            Your task is to refine the provided Project Name.
            Only output the refined name — do NOT include explanations, punctuation, or extra text.
            """),
            ("human",
            """Project Name: {project_name}""")
        ])
        refine_project_name_chain = prompt | self.llm
        response = refine_project_name_chain.invoke({"project_name":project_name})
        logger.info(f"Project name refined successfully. Refined name: {response.content}")
        return response.content
    
if __name__ == "__main__":
    a = Agent()
    response =a.generate_description("AI in healthcare")
    print(response)
    response = a.suggest_tasks("AI in healthcare", "the radiology project")
    print(response)
    response = a.refine_project_name("AI in healthcare radiology works")
    print("Refined name: ", response)