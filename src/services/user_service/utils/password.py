<<<<<<< HEAD
import bcrypt

def hash_password(password: str)->str:
    salt = bcrypt.gensalt()
    hashed = bcrypt.hashpw(password.encode(), salt)
    return hashed.decode()

def verify_password(password: str, hashed: str)->bool:
=======
import bcrypt

def hash_password(password: str)->str:
    salt = bcrypt.gensalt()
    hashed = bcrypt.hashpw(password.encode(), salt)
    return hashed.decode()

def verify_password(password: str, hashed: str)->bool:
>>>>>>> 05b451b (new_update)
    return bcrypt.checkpw(password.encode(), hashed.encode())