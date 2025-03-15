.PHONY: help db db-logs stop-db clean-db ollama ollama-models ollama-logs load-e5-model

# Colors for terminal output
GREEN = \033[0;32m
YELLOW = \033[0;33m
NC = \033[0m # No Color

help:
	@echo "Usage: make [target]"
	@echo ""
	@echo "Targets:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-18s\033[0m %s\n", $$1, $$2}'

db: ## Start only the database service
	@echo "$(GREEN)Starting database...$(NC)"
	docker compose up -d db

db-logs: ## Show logs from the database service
	docker compose logs -f db

stop-db: ## Stop the database service
	@echo "$(GREEN)Stopping database...$(NC)"
	docker compose stop db

ollama: ## Start Ollama service with Mistral 7B model
	@echo "$(GREEN)Starting Ollama service with Mistral 7B...$(NC)"
	docker compose up -d ollama
	@echo "$(YELLOW)Waiting for Ollama to initialize...$(NC)"
	@sleep 5
	@echo "$(GREEN)Ollama service is running. Mistral 7B will be loaded automatically.$(NC)"
	@echo "$(YELLOW)Note: The initial model download might take several minutes. Check status with 'make ollama-logs'$(NC)"

ollama-logs: ## Show logs from the Ollama service
	docker compose logs -f ollama

ollama-models: ## List all available models in Ollama
	@echo "$(GREEN)Listing available Ollama models...$(NC)"
	@if ! command -v jq &> /dev/null; then \
		echo "$(YELLOW)jq is not installed. Installing jq...$(NC)"; \
		brew install jq; \
	fi
	@curl -s http://localhost:11434/api/tags | jq '.models[] | {name: .name, size: .size, modified_at: .modified_at}'

load-e5-model: ## Load the multilingual-e5-base model (on-demand)
	@echo "$(GREEN)Loading multilingual-e5-base model...$(NC)"
	@echo "$(YELLOW)This may take several minutes for the first load.$(NC)"
	@curl -X POST http://localhost:11434/api/pull -d '{"name":"e5-base-multilingual"}' > /dev/null
	@echo "$(GREEN)Model loaded successfully!$(NC)"

unload-e5-model: ## Unload the multilingual-e5-base model
	@echo "$(GREEN)Unloading multilingual-e5-base model...$(NC)"
	@curl -X POST http://localhost:11434/api/push -d '{"name":"e5-base-multilingual"}' > /dev/null
	@echo "$(GREEN)Model unloaded successfully!$(NC)"

stop-ollama: ## Stop the Ollama service
	@echo "$(GREEN)Stopping Ollama...$(NC)"
	docker compose stop ollama

up: db ollama ## Start all services (database and Ollama)
down: stop-db stop-ollama ## Stop all services (database and Ollama)
clean: ## Remove all containers, images, and volumes
	@echo "$(GREEN)Cleaning up all services...$(NC)"
	docker compose down -v --rmi all --remove-orphans

app: ## Launch the application
	@echo "$(GREEN)Launching application...$(NC)"
	dotnet run --launch-profile http --project=src/SacraScriptura.API

dependencies:
	@echo "$(GREEN)Restoring dependencies...$(NC)"
	dotnet restore

migrations:
	@echo "$(GREEN)Running database migrations...$(NC)"
	dotnet ef database update --project src/SacraScriptura.Infrastructure/SacraScriptura.Infrastructure.csproj --startup-project src/SacraScriptura.API/SacraScriptura.API.csproj

add-migration: ## Create a new migration
	@read -p "Enter migration name: " name; \
	 echo "$(GREEN)Adding migration '$$name'...$(NC)"; \
	 dotnet ef migrations add $$name --project src/SacraScriptura.Infrastructure/SacraScriptura.Infrastructure.csproj --startup-project src/SacraScriptura.API/SacraScriptura.API.csproj

tests: domain-tests ## Run all tests

domain-tests: ## Run domain tests
	@echo "$(GREEN)Running domain tests...$(NC)"
	dotnet test tests/SacraScriptura.Domain.Tests
