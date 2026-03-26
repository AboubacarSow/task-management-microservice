from jose import jwt, JWTError
from fastapi import HTTPException, Depends
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
import os
import logging

logger = logging.getLogger(__name__)

REQUIRED_CLAIMS = ["id", "first_name", "last_name", "email"]

security = HTTPBearer()


def verify_token(token: str) -> dict:
    secret_key = os.getenv("JWT_SECRET")
    algorithm = os.getenv("JWT_ALGORITHM", "HS256")

    if not secret_key:
        logger.error("JWT_SECRET is not set")
        raise RuntimeError("JWT_SECRET is not configured")

    try:
        logger.info("Verifying JWT token...")
        payload = jwt.decode(token, secret_key, algorithms=[algorithm])

        missing_fields = [field for field in REQUIRED_CLAIMS if not payload.get(field)]
        if missing_fields:
            logger.warning(f"JWT missing required claims: {missing_fields}")
            raise HTTPException(
                status_code=401,
                detail=f"Missing required claims: {', '.join(missing_fields)}"
            )

        logger.info(f"JWT verified for user_id={payload.get('id')}")
        return payload

    except JWTError:
        logger.warning("Invalid JWT token")
        raise HTTPException(
            status_code=401,
            detail="Invalid token"
        )


def get_current_user(credentials: HTTPAuthorizationCredentials = Depends(security)) -> dict:
    token = credentials.credentials
    return verify_token(token)