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