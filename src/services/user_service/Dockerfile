FROM python:3.12-slim

WORKDIR /app

COPY src/services/user_service/requirements.txt .

RUN pip install --no-cache-dir -r requirements.txt

EXPOSE 5002

CMD ["uvicorn", "src.services.user_service.main:app", "--host", "0.0.0.0", "--port", "5002"]