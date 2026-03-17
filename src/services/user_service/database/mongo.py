from pymongo import MongoClient
import os
from dotenv import load_dotenv

load_dotenv("../.env")

MONGO_URL = os.getenv("MONGO_URL")
DB_NAME = os.getenv("DB_NAME")
USER_COLLECTION = os.getenv("USER_COLLECTION")

client = MongoClient(MONGO_URL)

db = client[DB_NAME]

user_collection = db[USER_COLLECTION]