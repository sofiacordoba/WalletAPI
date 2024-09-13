# Wallet API Documentation

**Author:** Sofia Cordoba  
**Date:** September 2024  
**Version:** 1.0  

## Table of Contents

1. [Introduction](#introduction)
2. [Project Structure](#project-structure)
3. [Requirements](#requirements)
4. [Design Patterns](#design-patterns)
5. [Data Flow](#data-flow)
6. [API Endpoints](#api-endpoints)
    - Create Wallet
    - List Wallets
    - Transfer
    - Get Transactions
7. [Validations](#validations)
8. [Unit and Integration Tests](#unit-and-integration-tests)
9. [Test Coverage](#test-coverage)
10. [Technologies Used](#technologies-used)
11. [Future Improvements](#future-improvements)

---

## 1. Introduction

This project is a Wallet API that allows account management and wallet-to-wallet transactions. It provides endpoints to create accounts, list wallets with optional filters, process transfers with validation, and retrieve transaction histories. The implementation adheres to Clean Architecture principles, ensuring maintainability, modularity, and robust automated testing.

---

## 2. Project Structure

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

---

## 3. Requirements

- **Framework:** .NET Core 6.0  
- **ORM:** Entity Framework Core (with in-memory database for testing)  
- **Language:** C#  
- **Testing Framework:** xUnit  
- **Additional Tools:** AutoMapper, Swagger, InMemoryDatabase  

---

## 4. Design Patterns

The project uses the following design patterns:
- **Repository Pattern:** For data access and database operations.
- **Service Layer Pattern:** To encapsulate business logic and orchestrate interactions between repositories and controllers.
- **Dependency Injection:** For injecting services and repositories into controllers and other components.
- **DTOs (Data Transfer Objects):** To transfer data between layers without exposing domain entities directly.

---

## 5. Data Flow

The flow of data through the system is as follows:

1. **API Request:** A request is made to an endpoint (e.g., create a wallet or make a transfer).
2. **Controller Layer:** The controller processes the request and calls the appropriate service.
3. **Service Layer:** The service validates the request, performs business logic, and interacts with repositories for data access.
4. **Repository Layer:** The repository accesses the database (or in-memory database for testing) to perform CRUD operations.
5. **Response:** The service returns the result to the controller, which then sends the response back to the client.

---

## 6. API Endpoints

### Create Wallet
**POST /api/wallet**

This endpoint creates a new wallet.

### List Wallets
**GET /api/wallet**

This endpoint retrieves a list of wallets with optional filters for currency and user document.

### Transfer
**POST /api/transaction/transfer**

This endpoint processes a wallet-to-wallet transfer.

### Get Transactions
**GET /api/transaction/{walletId}/transactions**

This endpoint retrieves transaction history for a specific wallet.

---

## 7. Validations

The API implements the following validations:
- **Balance Validation:** Ensures that transfers are only processed if the source wallet has sufficient balance.
- **Currency Validation:** Transfers are only allowed between wallets that share the same currency.
- **Amount Validation:** Transfers must have a positive amount greater than zero.

---

## 8. Unit and Integration Tests

The project includes both unit tests and integration tests to ensure the correctness of the API:

- **Unit Tests:** Test individual components such as services and repositories in isolation.
- **Integration Tests:** Test the interaction between multiple components (e.g., controllers, services, repositories).

---

## 9. Test Coverage

Coverage results for the project:

| Module                  | Line   | Branch | Method |
|-------------------------|--------|--------|--------|
| Kata.Wallet.Api         | 70.22% | 58.33% | 87.5%  |
| Kata.Wallet.Database    | 100%   | 100%   | 100%   |
| Kata.Wallet.Domain      | 76.92% | 100%   | 76.92% |
| Kata.Wallet.Dtos        | 55%    | 0%     | 78.57% |
| Kata.Wallet.Persistence | 0%     | 0%     | 0%     |
| Kata.Wallet.Services    | 87.87% | 70.83% | 85.71% |
| **Total**               | 64.5%  | 50%    | 69.64% |

---

## 10. Technologies Used

- **ASP.NET Core 6.0:** For building the RESTful API.
- **Entity Framework Core:** For data access and ORM.
- **AutoMapper:** For mapping between entities and DTOs.
- **xUnit:** For unit and integration testing.
- **Coverlet:** For measuring code coverage.
- **Swagger:** For API documentation and testing.

---

## 11. Future Improvements

Possible future improvements to the Wallet API:
- **Logging Enhancements:** Integrate more advanced logging mechanisms for better traceability.
- **Additional Validations:** Add more robust input validations, such as checking for duplicate wallets.
- **Optimizations:** Refactor services for improved performance in high-volume environments.
- **Authentication and Authorization:** Implement user authentication and role-based access control.
