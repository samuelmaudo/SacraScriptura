.PHONY: help up down clean logs db-logs ollama-logs ollama-models web admin dependencies migrations add-migration tests domain-tests

# Colors for terminal output
GREEN = \033[0;32m
YELLOW = \033[0;33m
NC = \033[0m # No Color

help:
	@echo "Usage: make [target]"
	@echo ""
	@echo "Targets:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-18s\033[0m %s\n", $$1, $$2}'

up: ## Start all services
	@echo "$(GREEN)Starting database and Ollama...$(NC)"
	docker compose up -d
	@echo "$(YELLOW)Waiting for Ollama to initialize...$(NC)"
	@while ! curl -s --output /dev/null --fail http://localhost:11434/api/version; do \
		echo "Waiting for Ollama..."; \
		sleep 2; \
	done
	@echo "$(GREEN)Loading multilingual-e5-base...$(NC)"
	docker exec ollama-main ollama pull yxchia/multilingual-e5-base
	@echo "$(GREEN)Loading mistral:7b...$(NC)"
	docker exec ollama-main ollama pull mistral:7b
	@echo "$(GREEN)Configuring model e5-embeddings...$(NC)"
	docker exec ollama-main bash -c "echo 'FROM yxchia/multilingual-e5-base' > /tmp/e5-embeddings.modelfile"
	docker exec ollama-main bash -c "echo 'PARAMETER temperature 0.0' >> /tmp/e5-embeddings.modelfile"
	docker exec ollama-main ollama create e5-embeddings -f /tmp/e5-embeddings.modelfile
	@echo "$(GREEN)Configuring model mistral-summarization...$(NC)"
	docker exec ollama-main bash -c "echo 'FROM mistral:7b' > /tmp/mistral-summarization.modelfile"
	docker exec ollama-main bash -c "echo 'PARAMETER temperature 0.1' >> /tmp/mistral-summarization.modelfile"
	docker exec ollama-main ollama create mistral-summarization -f /tmp/mistral-summarization.modelfile
	@echo "$(GREEN)Ollama service is running.$(NC)"

down: ## Stop all services
	@echo "$(GREEN)Stopping database and Ollama...$(NC)"
	docker compose stop

clean: ## Remove all containers, images, and volumes
	@echo "$(GREEN)Cleaning up all services...$(NC)"
	docker compose down -v --rmi all --remove-orphans

logs: ## Show logs from all services
	docker compose logs

db-logs: ## Show logs from the database service
	docker compose logs -f db

ollama-logs: ## Show logs from the Ollama service
	docker compose logs -f ollama

ollama-models: ## List all available models in Ollama
	@echo "$(GREEN)Listing available Ollama models...$(NC)"
	@if ! command -v jq &> /dev/null; then \
		echo "$(YELLOW)jq is not installed. Installing jq...$(NC)"; \
		brew install jq; \
	fi
	@curl -s http://localhost:11434/api/tags | jq '.models[] | {name: .name, size: .size, modified_at: .modified_at}'

web: ## Launch the web API
	@echo "$(GREEN)Launching web API...$(NC)"
	dotnet run --launch-profile http --project=src/SacraScriptura.Web.API

admin: ## Launch the admin API
	@echo "$(GREEN)Launching admin API...$(NC)"
	dotnet run --launch-profile http --project=src/SacraScriptura.Admin.API

dependencies:
	@echo "$(GREEN)Restoring dependencies...$(NC)"
	dotnet restore

migrations:
	@echo "$(GREEN)Running database migrations...$(NC)"
	dotnet ef database update --project src/SacraScriptura.Admin.Infrastructure --startup-project src/SacraScriptura.Admin.API

add-migration: ## Create a new migration
	@read -p "Enter migration name: " name; \
	 echo "$(GREEN)Adding migration '$$name'...$(NC)"; \
	 dotnet ef migrations add $$name --project src/SacraScriptura.Admin.Infrastructure --startup-project src/SacraScriptura.Admin.API

tests: domain-tests ## Run all tests

domain-tests: ## Run domain tests
	@echo "$(GREEN)Running domain tests...$(NC)"
	dotnet test tests/SacraScriptura.Shared.Domain.Tests
