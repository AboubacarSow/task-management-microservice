from fastapi.testclient import TestClient
from unittest.mock import patch
import sys
import os

# Add the src folder to sys.path
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '../../../src')))
from services.ai_agent_service import api_instance

client = TestClient(api_instance.app)

def override_get_current_user():
    return {
        "id": "test_user",
        "email": "test@test.com",
        "first_name": "Test",
        "last_name": "User"
    }
    
from services.ai_agent_service.utils.jwt_handler import get_current_user

api_instance.app.dependency_overrides[get_current_user] = override_get_current_user

def test_generate_description_mocked():
    
    with patch.object(api_instance.agent, "generate_description") as mock_generate:
        mock_generate.return_value = "Mocked description"

        response = client.post("/api/agent/generate_description", json={"project_name": "Build AI project"})

    assert response.status_code == 200
    data = response.json()
    assert data["project_description"] == "Mocked description"
    assert isinstance(data["project_description"], str)
    mock_generate.assert_called_once_with("Build AI project")
    
def test_suggest_tasks_mocked():
    with patch.object(api_instance.agent, "suggest_tasks") as mock_suggest:
        mock_suggest.return_value = ["Mocked Task1", "Mocked Task2"]
        
        response = client.post("/api/agent/suggest_tasks", json={"project_name": "Build AI project", "project_description": "Mocked Description"})
    assert response.status_code == 200
    data = response.json()
    assert data["suggested_tasks"] == ["Mocked Task1", "Mocked Task2"]
    assert isinstance(data["suggested_tasks"], list)
    assert all(isinstance(item, str) for item in data["suggested_tasks"])
    mock_suggest.assert_called_once_with("Build AI project","Mocked Description")
    
def test_refine_project_name_mocked():
    with patch.object(api_instance.agent, "refine_project_name") as mock_refine:
        mock_refine.return_value = "Mocked Refined Name"
        
        response = client.post("/api/agent/refine_project_name", json={"project_name": "Build AI project"})
        
    assert response.status_code == 200
    data = response.json()
    assert data["refined_project_name"] == "Mocked Refined Name"
    assert isinstance(data["refined_project_name"],str)
    mock_refine.assert_called_once_with("Build AI project")
    