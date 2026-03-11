import pytest
import sys
import os
from unittest.mock import patch

sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '../../../src')))

from services.ai_agent_service import Agent


def test_generate_description():
    with patch.object(Agent, "generate_description") as mock_generate:
        mock_generate.return_value = "Mocked description"
        
        agent = Agent()
        response = agent.generate_description("AI integration in hospitals")
        
        assert isinstance(response, str)
        assert response == "Mocked description"
        mock_generate.assert_called_once_with("AI integration in hospitals")
    
def test_suggest_tasks():
    description = """
    This project aims to integrate Artificial Intelligence (AI) technologies into various aspects of healthcare,
    with a primary focus on improving patient outcomes and enhancing clinical decision-making. The proposed 
    system will leverage machine learning algorithms and natural language processing techniques to analyze large
    volumes of electronic health records, medical images, and genomic data, enabling early disease detection, 
    personalized treatment plans, and predictive analytics for risk management. By automating routine tasks,
    reducing diagnostic errors, and providing real-time insights to healthcare professionals, this project seeks
    to revolutionize the way healthcare is delivered, ultimately leading to better patient care, increased 
    efficiency, and reduced healthcare costs.
    """
    with patch.object(Agent, "suggest_tasks") as mock_suggest:
        mock_suggest.return_value = ["Mocked Task1", "Mocked Task2"]
        
        agent = Agent()
        response = agent.suggest_tasks("AI integration in hospitals",description)
        assert response == ["Mocked Task1", "Mocked Task2"]
        assert isinstance(response, list)
        assert all(isinstance(item, str) for item in response)
        assert len(response) > 0
    with patch.object(Agent, "suggest_tasks") as mock_suggest:
        mock_suggest.return_value = ["Mocked Task1", "Mocked Task2"]
        agent = Agent()
        
        response = agent.suggest_tasks("AI integration in hospitals")
        assert isinstance(response, list)
        assert all(isinstance(item, str) for item in response)
        assert len(response) > 0
        mock_suggest.assert_called_once_with("AI integration in hospitals")
    
def test_refine_project_name():
    project_name = "AI integration in hospitals"
    with patch.object(Agent, "refine_project_name") as mock_refine:
        mock_refine.return_value = "Refined Project Name"
        
        agent = Agent()
        response = agent.refine_project_name(project_name)
        assert response == "Refined Project Name"
        assert isinstance(response, str)
        assert len(response) > 0
        mock_refine.assert_called_once_with(project_name)