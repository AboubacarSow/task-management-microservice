from services.ai_agent_service.api.api import AgentApi
import uvicorn

api_instance = AgentApi()

uvicorn.run(api_instance.app)