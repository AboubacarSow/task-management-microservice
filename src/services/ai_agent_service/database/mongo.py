from pymongo import MongoClient
import os
import logging

logger = logging.getLogger(__name__)

MONGO_URL = os.getenv("MONGO_URL")
DB_NAME = os.getenv("DB_NAME")
LOG_COLLECTION = os.getenv("LOG_COLLECTION")

if not MONGO_URL or not DB_NAME or not LOG_COLLECTION:
    logger.error("Missing MongoDB environment variables")
    raise ValueError("Missing environment variables")

try:
    logger.info("Connecting to MongoDB")
    client = MongoClient(MONGO_URL)
    db = client[DB_NAME]
    log_collection = db[LOG_COLLECTION]
    logger.info("MongoDB connection established")
except Exception as e:
    logger.error(f"MongoDB connection failed: {e}")
    raise