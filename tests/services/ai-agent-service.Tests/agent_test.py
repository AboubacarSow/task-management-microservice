import pytest
import sys
import os

# Add the src folder to sys.path
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '../../../src')))

from services.ai_agent_service import Agent


def test_generate_description():
    agent = Agent()
    response = agent.generate_description("AI integration in hospitals")
    assert isinstance(response, str)
    assert len(response) > 0
    
def test_suggest_tasks():
    agent = Agent()
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
    response = agent.suggest_tasks("AI integration in hospitals",description)
    assert isinstance(response, list)
    assert all(isinstance(item, str) for item in response)
    assert len(response) > 0
    response = agent.suggest_tasks("AI integration in hospitals")
    assert isinstance(response, list)
    assert all(isinstance(item, str) for item in response)
    assert len(response) > 0