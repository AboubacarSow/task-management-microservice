<<<<<<< HEAD
from .agent_repository_interface import AgentRepositoryInterface
from ..models.agent_models import LogResponse


class MongoAgentRepository(AgentRepositoryInterface):

    def __init__(self, collection):
        self.collection = collection

    def log_response(self, log: LogResponse):
=======
from .agent_repository_interface import AgentRepositoryInterface
from ..models.agent_models import LogResponse


class MongoAgentRepository(AgentRepositoryInterface):

    def __init__(self, collection):
        self.collection = collection

    def log_response(self, log: LogResponse):
>>>>>>> 05b451b (new_update)
        self.collection.insert_one(log.model_dump())