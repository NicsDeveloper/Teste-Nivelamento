# **Questão 5 - API de Movimentação Bancária**

## **Introdução**
A API Questão 5 permite gerenciar movimentações bancárias (crédito e débito) e consultar saldos de contas. Desenvolvida em **.NET 6**, utiliza o padrão **CQRS** com MediatR para comandos e consultas.

---

## **Começando**

### **Pré-requisitos**
- SDK .NET 6
- Banco de dados SQLite

### **Configuração**
1. Navegue até a pasta do projeto.
2. Execute o comando para iniciar a API:
   ```bash
   dotnet run
   ```
3. Acesse a documentação Swagger no navegador:
   ```
   http://localhost:5000
   ```

---

## **Endpoints**

### **Consultar Saldo**
**Endpoint:** `GET /api/Balance`  
**Descrição:** Retorna o saldo de uma conta específica.

#### **Requisição**
```json
{
  "currentAccountId": "382D323D-7067-ED11-8866-7D5DFA4A16C9"
}
```

#### **Resposta de Sucesso (200 OK)**
```json
{
  "accountNumber": 789,
  "accountName": "Tevin Mcconnell",
  "balance": 10533.2,
  "responseDateTime": "2025-03-29T14:01:30.7216789Z"
}
```

#### **Possíveis Erros**
- **404 Not Found**: Conta não encontrada.
- **400 Bad Request**: Conta inativa.

---

### **Registrar Movimentação**
**Endpoint:** `POST /api/Movement`  
**Descrição:** Registra uma movimentação (crédito ou débito) em uma conta.

#### **Requisição**
```json
{
  "currentAccountId": "382D323D-7067-ED11-8866-7D5DFA4A16C9",
  "value": 12.00,
  "movementType": "C",
  "idempotencia": "unique-key-123"
}
```
**Observações:**  
- `movementType`: "C" para crédito, "D" para débito.
- `idempotencia`: Chave única para garantir idempotência.

#### **Resposta de Sucesso (200 OK)**
```json
{
  "movementId": "976786fb-1472-4bf9-bb55-c526569a7e5f"
}
```

#### **Possíveis Erros**
- **400 Bad Request**:
  - Valor inválido (deve ser positivo).
  - Tipo de movimentação inválido (deve ser "C" ou "D").
  - Conta não encontrada ou inativa.

---

## **Detalhes Técnicos**

### **Estrutura do Banco de Dados**
| Tabela             | Campos                          |
|---------------------|---------------------------------|
| `CurrentAccount`    | `CurrentAccountId`, `Number`, `Name`, `Active` |
| `Movement`         | `MovementId`, `CurrentAccountId`, `MovementData`, `MovementType`, `Value` |
| `Idempotencia`     | `Chave_Idempotencia`, `Resultado` |

### **Principais Classes**
- **Comandos/Consultas**:
  - `CreateMovementCommand`: Requisição para criar movimentação.
  - `GetBalanceByIdQuery`: Requisição para consultar saldo.
  
- **Handlers**:
  - `CreateMovementHandler`: Processa a criação de movimentação.
  - `GetBalanceByIdHandler`: Processa a consulta de saldo.

- **Respostas**:
  - `CreateMovementResponse`: Resposta da criação de movimentação.
  - `GetBalanceResponse`: Resposta da consulta de saldo.

---

## **Exemplos de Requisições**

### **Consulta de Saldo**
```http
GET /api/Balance HTTP/1.1
Content-Type: application/json

{
  "currentAccountId": "976786fb-1472-4bf9-bb55-c526569a7e5f"
}
```

### **Registro de Movimentação**
```http
POST /api/Movement HTTP/1.1
Content-Type: application/json

{
  "currentAccountId": "382D323D-7067-ED11-8866-7D5DFA4A16C9",
  "value": 12.00,
  "movementType": "C",
  "idempotencia": "1255"
}
```

---

## **Tratamento de Erros**
| Código de Status | Descrição                              | Mensagem de Erro                              |
|------------------|----------------------------------------|-----------------------------------------------|
| 400 Bad Request  | Conta inválida ou inativa              | `"INVALID_ACCOUNT: Current account not found."` ou `"INACTIVE_ACCOUNT: Current account is inactive."` |    |