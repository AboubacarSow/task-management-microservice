from abc import ABC, abstractmethod

class AgentRepositoryInterface(ABC):

    @abstractmethod
    def log_response(self, log):
       pass