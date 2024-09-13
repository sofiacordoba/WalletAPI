Wallet API Documentation
Author: Sofia Cordoba
Date: September 2024
Version: 1.0

Table of Contents
Introduction
Project Structure
Requirements
Design Patterns
Data Flow
API Endpoints
  Create Wallet
  List Wallets
  Transfer
  Get Transactions
Validations
Unit and Integration Tests
  Test Coverage
Technologies Used
Future Improvements
1. Introduction 
This project is a Wallet API that allows account management and wallet-to-wallet transactions. It provides endpoints to create accounts, list wallets with optional filters, process transfers with validation, and retrieve transaction histories. The implementation adheres to Clean Architecture principles, ensuring maintainability, modularity, and robust automated testing.

2. Project Structure 
The project is structured into multiple layers to maintain separation of concerns and allow scalability:

/Kata.Wallet.Api
    - Controllers (WalletController.cs, TransactionController.cs)
    - Program.cs
/Kata.Wallet.Services
    - Interfaces (IWalletService.cs, ITransactionService.cs)
    - Implementations (WalletService.cs, TransactionService.cs)
/Kata.Wallet.Domain
    - Entities (Wallet.cs, Transaction.cs)
/Kata.Wallet.Persistence
    - Repositories (WalletRepository.cs, TransactionRepository.cs)
/Kata.Wallet.Tests
    - Unit and Integration Tests
Each layer has a specific responsibility and is decoupled from the others to promote scalability and easy maintenance.

3. Requirements 
Framework: .NET Core 6.0
ORM: Entity Framework Core (with in-memory database for testing)
Language: C#
Testing Framework: xUnit
Additional Tools: AutoMapper, Swagger, InMemoryDatabase

5. Design Patterns 
The following design patterns were used in the project:
  Repository Pattern: Used to handle data access operations for the Wallet and Transaction entities.
  Dependency Injection: Injecting services and repositories into controllers and services.
  DTO (Data Transfer Objects): Used to decouple domain entities from the API responses.
  Custom Validation: Implemented custom validations using attributes, e.g., CurrencyValidation.

6. Data Flow 
The data flow in the API follows this sequence:
The client sends an HTTP request to an API endpoint.
The controller processes the request and delegates business logic to the appropriate service.
The service interacts with repositories to access the in-memory database (for testing) and perform required operations.
The result of the operation (e.g., account creation, transfer, transaction retrieval) is returned to the controller.
The controller sends back an HTTP response with the operation result.
This architecture ensures clear separation of concerns, enhancing scalability and maintainability.

7. API Endpoints 
6.1 Create Wallet (POST /api/wallet)
Description: Creates a new wallet account.
Request Body:
{
  "userDocument": "12345678",
  "userName": "John Doe",
  "currency": "USD",
  "balance": 100.00
}
Response Codes:
200 OK: Wallet successfully created.
400 Bad Request: Invalid or missing data.
500 Internal Server Error: Error during account creation.
6.2 List Wallets (GET /api/wallet)
Description: Returns a list of wallets, optionally filtered by currency or user document.

6.3 Transfer (POST /api/transactions/transfer)
Description: Processes a transfer between two wallets with balance and currency validations.
Request Body:
{
  "sourceWalletId": 1,
  "targetWalletId": 2,
  "amount": 50.00
}
Response Codes:
200 OK: Transfer successfully completed.
400 Bad Request: Insufficient balance or different currencies.
404 Not Found: One or both wallets not found.
6.4 Get Transactions (GET /api/transactions/{walletId})
Description: Retrieves the transactions associated with a specific wallet.

7. Validations 
Minimum Balance: The transfer amount must be greater than 0.
Currency: Transfers can only occur between wallets with the same currency.
User Document: User document is required and limited to 50 characters.
Existing Account: Throws an error if the user already has a wallet in the same currency.

9. Unit and Integration Tests
Tests Performed:
Unit Tests:
TransactionControllerTests.cs: Tests for creating transfers and error handling.
WalletControllerTests.cs: Tests for creating wallets and data validation.
Integration Tests: Simulates full API flows using an in-memory database.
WalletIntegrationTests.cs: Tests for creating accounts and transferring funds.

Test Coverage:
Coverage Results:
Module	Line	Branch	Method
Kata.Wallet.Api	70.22%	58.33%	87.5%
Kata.Wallet.Database	100%	100%	100%
Kata.Wallet.Domain	76.92%	100%	76.92%
Kata.Wallet.Dtos	55%	0%	78.57%
Kata.Wallet.Persistence	0%	0%	0%
Kata.Wallet.Services	87.87%	70.83%	85.71%
Total	64.5%	50%	69.64%
Average	65%	54.85%	71.45%

10. Technologies Used 
ASP.NET Core 6: Framework used for building the API.
Entity Framework Core: ORM used for database interaction.
xUnit: Testing framework for unit and integration tests.
AutoMapper: Used for mapping entities to DTOs.
InMemoryDatabase: For integration testing without a real database.
Swagger: Used for interactive API documentation.

12. Future Improvements
Potential future improvements include:
Authentication and Authorization: Add JWT-based authentication to protect endpoints.
Query Optimization: Implement caching to improve performance for fetching wallets and transactions.
Advanced Validations: Implement fraud detection and country-specific restrictions.
Extended Swagger Documentation: Add more detailed Swagger documentation for better API clarity.
