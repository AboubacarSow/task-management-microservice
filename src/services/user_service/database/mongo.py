<<<<<<< HEAD
from motor.motor_asyncio import AsyncIOMotorClient
import os
import logging

logger = logging.getLogger(__name__)

MONGO_URL = os.getenv("MONGO_URL")
DB_NAME = os.getenv("DB_NAME")
USER_COLLECTION = os.getenv("USER_COLLECTION")

if not MONGO_URL or not DB_NAME or not USER_COLLECTION:
    logger.error("Missing MongoDB environment variables")
    raise ValueError("Missing environment variables")

try:
    logger.info("Connecting to MongoDB")
    client = AsyncIOMotorClient(MONGO_URL)
    db = client[DB_NAME]
    user_collection = db[USER_COLLECTION]
    logger.info("MongoDB connection established")
except Exception as e:
    logger.error(f"MongoDB connection failed: {e}")
=======
from motor.motor_asyncio import AsyncIOMotorClient
import os
import logging

logger = logging.getLogger(__name__)

MONGO_URL = os.getenv("MONGO_URL")
DB_NAME = os.getenv("DB_NAME")
USER_COLLECTION = os.getenv("USER_COLLECTION")

if not MONGO_URL or not DB_NAME or not USER_COLLECTION:
    logger.error("Missing MongoDB environment variables")
    raise ValueError("Missing environment variables")

try:
    logger.info("Connecting to MongoDB")
    client = AsyncIOMotorClient(MONGO_URL)
    db = client[DB_NAME]
    user_collection = db[USER_COLLECTION]
    logger.info("MongoDB connection established")
except Exception as e:
    logger.error(f"MongoDB connection failed: {e}")
>>>>>>> 05b451b (new_update)
    raise