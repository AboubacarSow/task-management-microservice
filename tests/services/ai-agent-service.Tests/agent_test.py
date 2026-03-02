import pytest
from services.ai_agent_services.agent import Agent


def test_generate_description():
    agent = Agent()
    response = agent.generate_description("AI integration in hospitals")
    assert isinstance(response, str)
    assert len(response) > 0