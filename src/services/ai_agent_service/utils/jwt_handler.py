<<<<<<< HEAD
import os
import logging
import httpx

from fastapi import HTTPException, Depends
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from jose import jwt, JWTError
import asyncio

logger = logging.getLogger(__name__)

security = HTTPBearer()

AUTHORITY = os.getenv("AUTHORITY", "http://localhost:5004")
AUDIENCE = os.getenv("JWT_AUDIENCE", "agent-service")
ALGORITHMS = ["RS256"]

REQUIRED_CLAIMS = ["sub", "given_name", "family_name", "email"]


async def get_openid_config() -> dict:
    try:
        async with httpx.AsyncClient(timeout=5.0) as client:
            response = await client.get(f"{AUTHORITY}/.well-known/openid-configuration")
            response.raise_for_status()
            return response.json()
    except Exception as e:
        logger.error(f"Failed to fetch OpenID configuration: {e}")
        raise HTTPException(status_code=500, detail="Authentication configuration error")


async def get_jwks() -> dict:
    try:
        config = await get_openid_config()
        jwks_uri = config.get("jwks_uri")

        if not jwks_uri:
            logger.error("jwks_uri not found in OpenID configuration")
            raise HTTPException(status_code=500, detail="JWKS URI not found")

        async with httpx.AsyncClient(timeout=5.0) as client:
            response = await client.get(jwks_uri)
            response.raise_for_status()
            return response.json()
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Failed to fetch JWKS: {e}")
        raise HTTPException(status_code=500, detail="Unable to fetch signing keys")


def get_signing_key(token: str, jwks: dict) -> dict:
    try:
        unverified_header = jwt.get_unverified_header(token)
        kid = unverified_header.get("kid")

        if not kid:
            logger.warning("Token header missing 'kid'")
            raise HTTPException(status_code=401, detail="Invalid token header")

        for key in jwks.get("keys", []):
            if key.get("kid") == kid:
                return key

        logger.warning(f"No matching JWK found for kid={kid}")
        raise HTTPException(status_code=401, detail="Signing key not found")

    except JWTError as e:
        logger.warning(f"Failed to parse token header: {e}")
        raise HTTPException(status_code=401, detail="Invalid token header")


def verify_required_claims(payload: dict) -> None:
    missing_fields = [field for field in REQUIRED_CLAIMS if not payload.get(field)]
    if missing_fields:
        logger.warning(f"JWT missing required claims: {missing_fields}")
        raise HTTPException(
            status_code=401,
            detail=f"Missing required claims: {', '.join(missing_fields)}"
        )


async def verify_token(token: str) -> dict:
    try:
        logger.info("Fetching JWKS for JWT verification")
        jwks = await get_jwks()
        signing_key = get_signing_key(token, jwks)

        payload = jwt.decode(
            token,
            signing_key,
            algorithms=ALGORITHMS,
            audience=AUDIENCE,
            options={
                "verify_aud": False,  # keep this False if your .NET side disables audience check
            },
        )

        verify_required_claims(payload)

        logger.info(f"JWT verified successfully for user_id={payload.get('sub')}")
        return payload

    except HTTPException:
        raise
    except JWTError as e:
        logger.warning(f"JWT verification failed: {e}")
        raise HTTPException(status_code=401, detail="Invalid token")
    except Exception as e:
        logger.error(f"Unexpected error during token verification: {e}")
        raise HTTPException(status_code=401, detail="Invalid token")


async def get_current_user(credentials: HTTPAuthorizationCredentials = Depends(security)) -> dict:
    return await verify_token(credentials.credentials)

async def main():
    token = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijg5NTZDRDYyNDg3RUIxNUY4OEZDNzUwRDZDMTkxMUU3IiwidHlwIjoiYXQrand0In0.eyJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMDQiLCJuYmYiOjE3NzQ2MDQxODgsImlhdCI6MTc3NDYwNDE4OCwiZXhwIjoxNzc0NjA3Nzg4LCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwidXNlcl9mdWxscGVybWlzc2lvbiIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJwYXNzd29yZCJdLCJjbGllbnRfaWQiOiJwb3N0bWFuLWNsaWVudCIsInN1YiI6ImZkM2FmMmRiLTBhOWMtNDNiMy1iZTlmLWE5OGIyZmY4MTMyZCIsImF1dGhfdGltZSI6MTc3NDYwNDE4OCwiaWRwIjoibG9jYWwiLCJqdGkiOiI4NjBEMTU0MUE4Njk1MEI4RjM4QkEzRjE4NDIxQUQ0QyJ9.FxZmoSECnCWr_U7LcinpsFgO8bfBQ3Tziap0IHNElDGXOu3ksLVLB8OQV2gufUqijHeqQJX3vz_gW-HlTPsmDrvIRzp_VrNB5Xwz4Jm2lT8rxxrcwepSGexUez7m6oZXxuaclkKLkO72LBWpiudQuYdGCZZJdvqIIVDseu3xnFTmcxFrxhWaFkDi_3Am2o4I82cK289nKkR_FO6EuAubGElcwtveznCS8WaaYUfSXII14gspDEtWotQ2gIn8EsErC-zHx7NKSbtXVflk9QveQ--XtjuVxvn_NKpf4txQm9_PW4_M7hLFSjIRiWM3LKpVqe8jeI2T1S4eIOeDQRDqEQ"

    credentials = HTTPAuthorizationCredentials(
        scheme="Bearer",
        credentials=token
    )

    payload = await get_current_user(credentials)
    print(payload)


if __name__ == "__main__":
=======
import os
import logging
import httpx

from fastapi import HTTPException, Depends
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from jose import jwt, JWTError
import asyncio

logger = logging.getLogger(__name__)

security = HTTPBearer()

AUTHORITY = os.getenv("AUTHORITY", "http://localhost:5004")
AUDIENCE = os.getenv("JWT_AUDIENCE", "agent-service")
ALGORITHMS = ["RS256"]

REQUIRED_CLAIMS = ["sub", "given_name", "family_name", "email"]


async def get_openid_config() -> dict:
    try:
        async with httpx.AsyncClient(timeout=5.0) as client:
            response = await client.get(f"{AUTHORITY}/.well-known/openid-configuration")
            response.raise_for_status()
            return response.json()
    except Exception as e:
        logger.error(f"Failed to fetch OpenID configuration: {e}")
        raise HTTPException(status_code=500, detail="Authentication configuration error")


async def get_jwks() -> dict:
    try:
        config = await get_openid_config()
        jwks_uri = config.get("jwks_uri")

        if not jwks_uri:
            logger.error("jwks_uri not found in OpenID configuration")
            raise HTTPException(status_code=500, detail="JWKS URI not found")

        async with httpx.AsyncClient(timeout=5.0) as client:
            response = await client.get(jwks_uri)
            response.raise_for_status()
            return response.json()
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Failed to fetch JWKS: {e}")
        raise HTTPException(status_code=500, detail="Unable to fetch signing keys")


def get_signing_key(token: str, jwks: dict) -> dict:
    try:
        unverified_header = jwt.get_unverified_header(token)
        kid = unverified_header.get("kid")

        if not kid:
            logger.warning("Token header missing 'kid'")
            raise HTTPException(status_code=401, detail="Invalid token header")

        for key in jwks.get("keys", []):
            if key.get("kid") == kid:
                return key

        logger.warning(f"No matching JWK found for kid={kid}")
        raise HTTPException(status_code=401, detail="Signing key not found")

    except JWTError as e:
        logger.warning(f"Failed to parse token header: {e}")
        raise HTTPException(status_code=401, detail="Invalid token header")


def verify_required_claims(payload: dict) -> None:
    missing_fields = [field for field in REQUIRED_CLAIMS if not payload.get(field)]
    if missing_fields:
        logger.warning(f"JWT missing required claims: {missing_fields}")
        raise HTTPException(
            status_code=401,
            detail=f"Missing required claims: {', '.join(missing_fields)}"
        )


async def verify_token(token: str) -> dict:
    try:
        logger.info("Fetching JWKS for JWT verification")
        jwks = await get_jwks()
        signing_key = get_signing_key(token, jwks)

        payload = jwt.decode(
            token,
            signing_key,
            algorithms=ALGORITHMS,
            audience=AUDIENCE,
            options={
                "verify_aud": False,  # keep this False if your .NET side disables audience check
            },
        )

        verify_required_claims(payload)

        logger.info(f"JWT verified successfully for user_id={payload.get('sub')}")
        return payload

    except HTTPException:
        raise
    except JWTError as e:
        logger.warning(f"JWT verification failed: {e}")
        raise HTTPException(status_code=401, detail="Invalid token")
    except Exception as e:
        logger.error(f"Unexpected error during token verification: {e}")
        raise HTTPException(status_code=401, detail="Invalid token")


async def get_current_user(credentials: HTTPAuthorizationCredentials = Depends(security)) -> dict:
    return await verify_token(credentials.credentials)

async def main():
    token = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijg5NTZDRDYyNDg3RUIxNUY4OEZDNzUwRDZDMTkxMUU3IiwidHlwIjoiYXQrand0In0.eyJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMDQiLCJuYmYiOjE3NzQ2MDQxODgsImlhdCI6MTc3NDYwNDE4OCwiZXhwIjoxNzc0NjA3Nzg4LCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwidXNlcl9mdWxscGVybWlzc2lvbiIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJwYXNzd29yZCJdLCJjbGllbnRfaWQiOiJwb3N0bWFuLWNsaWVudCIsInN1YiI6ImZkM2FmMmRiLTBhOWMtNDNiMy1iZTlmLWE5OGIyZmY4MTMyZCIsImF1dGhfdGltZSI6MTc3NDYwNDE4OCwiaWRwIjoibG9jYWwiLCJqdGkiOiI4NjBEMTU0MUE4Njk1MEI4RjM4QkEzRjE4NDIxQUQ0QyJ9.FxZmoSECnCWr_U7LcinpsFgO8bfBQ3Tziap0IHNElDGXOu3ksLVLB8OQV2gufUqijHeqQJX3vz_gW-HlTPsmDrvIRzp_VrNB5Xwz4Jm2lT8rxxrcwepSGexUez7m6oZXxuaclkKLkO72LBWpiudQuYdGCZZJdvqIIVDseu3xnFTmcxFrxhWaFkDi_3Am2o4I82cK289nKkR_FO6EuAubGElcwtveznCS8WaaYUfSXII14gspDEtWotQ2gIn8EsErC-zHx7NKSbtXVflk9QveQ--XtjuVxvn_NKpf4txQm9_PW4_M7hLFSjIRiWM3LKpVqe8jeI2T1S4eIOeDQRDqEQ"

    credentials = HTTPAuthorizationCredentials(
        scheme="Bearer",
        credentials=token
    )

    payload = await get_current_user(credentials)
    print(payload)


if __name__ == "__main__":
>>>>>>> 05b451b (new_update)
    asyncio.run(main())