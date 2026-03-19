from pymongo import MongoClient
import os
from dotenv import load_dotenv

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
ENV_PATH = os.path.join(BASE_DIR, "../.env")

load_dotenv(ENV_PATH)

MONGO_URL = os.getenv("MONGO_URL")
DB_NAME = os.getenv("DB_NAME")
USER_COLLECTION = os.getenv("USER_COLLECTION")

if not MONGO_URL or not DB_NAME or not USER_COLLECTION:
    raise ValueError("Missing environment variables")

client = MongoClient(MONGO_URL)
db = client[DB_NAME]
user_collection = db[USER_COLLECTION]