.PHONY: help db app db-logs stop-db clean-db dependencies migrations tests domain-tests

# Colors for terminal output
GREEN = \033[0;32m
YELLOW = \033[0;33m
NC = \033[0m # No Color

help:
	@echo "Usage: make [target]"
	@echo ""
	@echo "Targets:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2}'

db:
	@echo "$(GREEN)Starting database...$(NC)"
	docker compose up -d

app:
	@echo "$(GREEN)Launching application...$(NC)"
	dotnet run --launch-profile http --project=src/SacraScriptura.API

db-logs:
	docker compose logs -f

stop-db:
	@echo "$(GREEN)Stopping database...$(NC)"
	docker compose down

clean-db: ## Remove all containers, images, and volumes
	@echo "$(GREEN)Cleaning up database...$(NC)"
	docker compose down -v --rmi all --remove-orphans

dependencies:
	@echo "$(GREEN)Restoring dependencies...$(NC)"
	dotnet restore

migrations:
	@echo "$(GREEN)Running database migrations...$(NC)"
	dotnet ef database update --project src/SacraScriptura.Infrastructure/SacraScriptura.Infrastructure.csproj --startup-project src/SacraScriptura.API/SacraScriptura.API.csproj

tests: domain-tests ## Run all tests

domain-tests:
	@echo "$(GREEN)Running domain tests...$(NC)"
	dotnet test tests/SacraScriptura.Domain.Tests
