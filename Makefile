.PHONY: help build up down restart logs ps clean test test-domain

# Variables
DOCKER_COMPOSE = docker compose
APP_NAME = sacra-scriptura
TEST_IMAGE_NAME = $(APP_NAME)-test

# Colors for terminal output
GREEN = \033[0;32m
YELLOW = \033[0;33m
NC = \033[0m # No Color

help: ## Show this help message
	@echo "Usage: make [target]"
	@echo ""
	@echo "Targets:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2}'

build: ## Build the application containers
	@echo "$(GREEN)Building application containers...$(NC)"
	@$(DOCKER_COMPOSE) build

up: ## Start the application
	@echo "$(GREEN)Starting application...$(NC)"
	@$(DOCKER_COMPOSE) up -d
	@echo "$(GREEN)Application is running!$(NC)"
	@echo "$(YELLOW)API is available at: http://localhost:8080$(NC)"

down: ## Stop the application
	@echo "$(GREEN)Stopping application...$(NC)"
	@$(DOCKER_COMPOSE) down

restart: down up ## Restart the application

logs: ## View application logs
	@$(DOCKER_COMPOSE) logs -f

ps: ## List running containers
	@$(DOCKER_COMPOSE) ps

clean: ## Remove all containers, images, and volumes
	@echo "$(GREEN)Cleaning up...$(NC)"
	@$(DOCKER_COMPOSE) down -v --rmi all --remove-orphans

test: test-domain ## Run all tests

test-domain: ## Run domain tests
	@echo "$(GREEN)Running domain tests...$(NC)"
	@echo "$(YELLOW)Building test image...$(NC)"
	@docker build -t $(TEST_IMAGE_NAME)-domain -f tests/Dockerfile.domain.tests .
	@echo "$(YELLOW)Running tests...$(NC)"
	@docker run --rm $(TEST_IMAGE_NAME)-domain
	@echo "$(GREEN)Domain tests completed!$(NC)"

# Alias for common commands
start: up ## Alias for 'up'
stop: down ## Alias for 'down'
