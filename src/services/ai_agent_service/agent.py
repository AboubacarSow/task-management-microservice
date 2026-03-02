from langchain_ollama import ChatOllama
from langchain_core.prompts import ChatPromptTemplate

class Agent:
    def __init__(self):
        self.llm = ChatOllama(model="llama3.2")
    def generate_description(self, project_name):
        prompt = ChatPromptTemplate([
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
        response = generate_description_chain.invoke(project_name)
        return response.content
    
    def suggest_tasks(self, project_name, project_description=None):
        return [ "Prapare the dataset", "Analyse the dataset"]
    
    
if __name__ == "__main__":
    a = Agent()
    response =a.generate_description("AI in healthcare")
    print(response)