# Minimal Workflow Engine (State-Machine API)

## Overview
This project implements a minimal backend service for configurable workflow state machines, as described in the assignment. It allows you to define workflows, start workflow instances, execute actions, and inspect state/history—all via a simple HTTP API.

## Quick Start

### Prerequisites
- .NET 8 SDK (already installed)

### Run the API
```sh
cd WorkFlowEngine/WorkflowApi
 dotnet run
```
The API will start (by default on http://localhost:5000 or http://localhost:8080).

## API Usage

### 1. Create a Workflow Definition
**POST** `/workflows`
```json
{
  "id": "wf1",
  "name": "Sample Workflow",
  "states": [
    { "id": "start", "name": "Start", "isInitial": true, "isFinal": false, "enabled": true },
    { "id": "end", "name": "End", "isInitial": false, "isFinal": true, "enabled": true }
  ],
  "actions": [
    { "id": "toEnd", "name": "Go to End", "enabled": true, "fromStates": ["start"], "toState": "end" }
  ]
}
```

### 2. Retrieve a Workflow Definition
**GET** `/workflows/wf1`

### 3. Start a Workflow Instance
**POST** `/workflows/wf1/instances`

### 4. Execute an Action
**POST** `/instances/{instanceId}/actions/toEnd`

### 5. Get Instance State & History
**GET** `/instances/{instanceId}`

## Assumptions & Notes
- All data is stored in memory; restarting the app will clear all workflows and instances.
- No authentication or authorization is implemented.
- Validation is enforced for state machine correctness (e.g., one initial state, no duplicate IDs).
- The API is intentionally minimal and designed for clarity and easy review.

## Shortcuts / Known Limitations
- No persistent storage (per assignment instructions).
- No unit tests (can be added if needed).
- No OpenAPI/Swagger UI (can be added for better API exploration).

## Contact
For any questions or clarifications, please refer to the assignment or contact the author. 