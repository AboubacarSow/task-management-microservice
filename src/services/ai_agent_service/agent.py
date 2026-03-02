from langchain_ollama import ChatOllama
from langchain_core.prompts import ChatPromptTemplate
from typing import List, Optional
from pydantic import BaseModel, Field

class Agent:
    def __init__(self):
        self.llm = ChatOllama(model="llama3.2")
    def generate_description(self, project_name: str) -> str:
        prompt = ChatPromptTemplate.from_messages([
            ("system",
             """You are a professinal project description writer.
             Your job is to write a one paragraph sutible, relevant and supported porject description based on the project name.
             Don't output anything else just the generated paragraph.
             """),
            ("human",
             """Project name: {project_name}
             """)
        ])
        
        generate_description_chain = prompt | self.llm
        response = generate_description_chain.invoke({"project_name":project_name})
        return response.content
    
    def suggest_tasks(self, project_name: str, project_description: Optional[str]=None) -> List[str]:
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
            suggest_tasks_chain = project_description_prompt | self.llm.with_structured_output(TasksOutput)
            response = suggest_tasks_chain.invoke({"project_name":project_name, "project_description":project_description})
        else:
            suggest_tasks_chain = project_name_prompt | self.llm.with_structured_output(TasksOutput)
            response = suggest_tasks_chain.invoke({"project_name":project_name})
            
        return response.tasks
    
    def refine_project_name(self, project_name):
        return "Implementing AI models in healthcare"
    
if __name__ == "__main__":
    a = Agent()
    response =a.generate_description("AI in healthcare")
    print(response)
    response = a.suggest_tasks("AI in healthcare", "the radiology project")
    print(response)