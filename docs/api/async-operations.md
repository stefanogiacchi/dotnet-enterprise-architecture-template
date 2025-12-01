# Asynchronous Operations Specification
**.NET Enterprise Architecture Template**

---

## Document Control

| Attribute | Value |
|-----------|-------|
| Document ID | API-ASYNC-001 |
| Document Version | 1.0 |
| Status | Active |
| Document Type | Technical Specification |
| Target Audience | Solution Architects, API Designers, Development Teams, DevOps Engineers, C-Level Executives |
| Classification | Internal/Public |
| Last Updated | 2025 |
| Document Owner | Enterprise Architecture Team |
| Review Cycle | Annual |
| Related Documents | API Guidelines (API-GUIDELINES-001), Error Handling (API-ERRORHANDLING-001), API Flow (ARCH-API-003) |
| Prerequisites | Understanding of HTTP protocol, REST principles, Asynchronous processing patterns |

---

## Executive Summary

This document specifies the asynchronous operation pattern for the .NET Enterprise Architecture Template, implementing the industry-standard HTTP 202 Accepted pattern with job tracking for long-running operations. This approach enables reliable processing of complex operations that exceed typical HTTP request timeout constraints while maintaining client responsiveness and system scalability.

**Strategic Business Value:**
- Enhanced user experience through non-blocking operation initiation
- Improved system reliability by preventing timeout-related failures
- Increased scalability through asynchronous processing decoupling
- Better resource utilization via background worker distribution
- Reduced infrastructure costs through efficient request handling
- Enhanced operational resilience through retry and failure management

**Key Technical Capabilities:**
- HTTP 202 Accepted response pattern for operation initiation
- Job tracking mechanism with unique identifiers
- Status polling interface with progress indication
- Result retrieval endpoints for completed operations
- Idempotency support preventing duplicate processing
- Comprehensive failure handling and error reporting
- Integration with background processing frameworks

**Compliance and Standards:**
- HTTP/1.1 specification (RFC 7230-7235) compliance
- RFC 7807 Problem Details for error responses
- Industry patterns from Azure, AWS, Google Cloud, Kubernetes
- ITIL 4 service operation management principles
- ISO/IEC 25010 quality characteristics (reliability, efficiency)

---

## Table of Contents

1. Introduction and Scope
2. Use Case Identification
3. Asynchronous Operation Lifecycle
4. HTTP 202 Accepted Pattern Specification
5. Job Status Tracking
6. Result Retrieval Patterns
7. Job Data Model Specification
8. Response Format Standards
9. Architecture Patterns
10. Implementation Technologies
11. Idempotency Requirements
12. Error Handling for Async Operations
13. Testing Requirements
14. Performance and Scalability
15. Glossary
16. Recommendations and Next Steps
17. References

---

## 1. Introduction and Scope

### 1.1. Purpose

This document provides comprehensive specifications for implementing asynchronous operations within the .NET Enterprise Architecture Template. It defines the HTTP 202 Accepted pattern, job tracking mechanisms, polling interfaces, and result retrieval patterns necessary for reliable long-running operation support in enterprise REST APIs.

### 1.2. Scope

**In Scope:**
- HTTP 202 Accepted response pattern
- Job submission and tracking mechanisms
- Status polling specifications
- Result retrieval endpoints
- Job lifecycle management
- Idempotency patterns for async operations
- Error handling for long-running operations
- Background processing integration
- Client polling guidelines
- Job expiration and cleanup

**Out of Scope:**
- Specific background processing framework implementation details
- Message queue configuration and infrastructure
- Worker service internal implementation
- Database schema implementation details
- Specific business domain operation logic
- Real-time notification mechanisms (WebSocket, SignalR)
- Event sourcing implementations
- CQRS read model projections

### 1.3. Document Audience

This specification addresses multiple stakeholder groups:

**Solution Architects:** Asynchronous operation patterns and system design

**API Designers:** Endpoint specifications and contract design

**Development Teams:** Implementation guidelines and code examples

**DevOps Engineers:** Background processing infrastructure and monitoring

**Frontend Developers:** Client polling patterns and integration

**C-Level Executives:** Business value and operational benefits

### 1.4. Pattern Context

The asynchronous operation pattern addresses:

**Problem Domain:**
- Operations exceeding HTTP timeout thresholds (typically 30-120 seconds)
- Resource-intensive processing (report generation, data analysis)
- External dependency coordination with variable latency
- Batch processing requirements
- Operations requiring resilience to transient failures

**Industry Adoption:**
- Microsoft Azure Resource Manager
- Amazon Web Services Asynchronous APIs
- Google Cloud Long-Running Operations
- GitHub Actions API
- Stripe Asynchronous Webhooks
- Kubernetes Asynchronous Resource Operations

---

## 2. Use Case Identification

### 2.1. Criteria for Asynchronous Operations

#### 2.1.1. Processing Duration Threshold

**Guideline:** Use asynchronous pattern when operation duration exceeds 1-2 seconds.

**Rationale:**
- HTTP client timeout defaults typically 30-60 seconds
- User experience degradation beyond 2 seconds
- Connection resource consumption
- Gateway timeout risks

#### 2.1.2. Resource Intensity

**Use Asynchronous When:**
- CPU-intensive operations (data transformation, encryption, compression)
- Memory-intensive operations (large file processing, report generation)
- I/O-intensive operations (bulk database operations, file system operations)
- Concurrent external API calls with aggregation

#### 2.1.3. External Dependency Coordination

**Use Asynchronous When:**
- Multiple external service calls required
- External service latency unpredictable
- Third-party API rate limiting necessitates queuing
- Distributed transaction coordination needed

### 2.2. Common Use Case Examples

#### 2.2.1. Document Generation

**Scenario:** Generate complex PDF reports from database queries and external data

**Characteristics:**
- Processing time: 5-30 seconds
- Resource intensive: CPU, memory
- Asynchronous benefit: High

**Pattern:**
```
POST /api/v1/reports/generate
→ 202 Accepted with jobId
→ Poll GET /api/v1/jobs/{jobId}
→ Retrieve GET /api/v1/reports/{reportId}
```

#### 2.2.2. Bulk Data Import

**Scenario:** Import large CSV/Excel file with validation and transformation

**Characteristics:**
- Processing time: 30 seconds to several minutes
- Record count: Thousands to millions
- Validation complexity: High
- Asynchronous benefit: Critical

#### 2.2.3. AI/ML Processing

**Scenario:** Execute machine learning inference or training operations

**Characteristics:**
- Processing time: Seconds to hours
- Computational requirements: High
- Unpredictable duration
- Asynchronous benefit: Critical

#### 2.2.4. System Integration Workflows

**Scenario:** Multi-step workflow across multiple external systems

**Characteristics:**
- External dependencies: Multiple
- Failure handling: Complex
- Retry requirements: High
- Asynchronous benefit: High

### 2.3. Anti-Patterns (When NOT to Use)

**Do Not Use Asynchronous Pattern For:**
- Simple CRUD operations (< 100ms)
- Real-time data retrieval requirements
- Operations where immediate result essential
- Simple validation operations
- Operations with predictable sub-second duration

---

## 3. Asynchronous Operation Lifecycle

### 3.1. Lifecycle States

#### 3.1.1. State Diagram

```
┌─────────────────────────────────────────────────────────┐
│                    Job Lifecycle                        │
└─────────────────────────────────────────────────────────┘

    Client Request
          ↓
    [1. Queued] ──────────────────────────┐
          ↓                                │
    [2. Running] ─────────┐                │
          ↓               ↓                ↓
    [3. Completed]   [4. Failed]   [5. Cancelled]
          ↓               ↓                ↓
    [Result Available] [Error Info]  [Cleanup]
          ↓               ↓                ↓
    [6. Expired/Deleted After Retention Period]
```

#### 3.1.2. State Definitions

| State | Description | Duration | Client Action |
|-------|-------------|----------|---------------|
| Queued | Job accepted, awaiting worker | Seconds to minutes | Poll periodically |
| Running | Worker processing job | Seconds to hours | Poll periodically |
| Completed | Processing successful | Until expiration | Retrieve result |
| Failed | Processing failed | Until expiration | Review error details |
| Cancelled | Client or system cancelled | Until expiration | No further action |
| Expired | Past retention period | N/A | 410 Gone response |

### 3.2. Complete Request-Response Flow

#### 3.2.1. Sequence Diagram

```
┌────────┐          ┌──────────┐          ┌──────────┐          ┌─────────┐
│ Client │          │ API      │          │ Job      │          │ Worker  │
│        │          │ Endpoint │          │ Queue    │          │ Service │
└───┬────┘          └────┬─────┘          └────┬─────┘          └────┬────┘
    │                    │                     │                     │
    │ POST /reports      │                     │                     │
    ├───────────────────>│                     │                     │
    │                    │                     │                     │
    │                    │ Create Job Record   │                     │
    │                    ├────────────────────>│                     │
    │                    │                     │                     │
    │                    │ Queue Job Message   │                     │
    │                    ├────────────────────>│                     │
    │                    │                     │                     │
    │ 202 Accepted       │                     │                     │
    │ + Location Header  │                     │                     │
    │ + Job ID           │                     │                     │
    │<───────────────────┤                     │                     │
    │                    │                     │                     │
    │                    │                     │ Dequeue Job         │
    │                    │                     ├────────────────────>│
    │                    │                     │                     │
    │                    │                     │                     │ Process
    │                    │                     │                     │  Job
    │ GET /jobs/{id}     │                     │                     │
    ├───────────────────>│                     │                     │
    │                    │                     │                     │
    │                    │ Query Job Status    │                     │
    │                    ├────────────────────>│                     │
    │                    │                     │                     │
    │ 202 Running        │                     │                     │
    │ + Progress: 45%    │                     │                     │
    │<───────────────────┤                     │                     │
    │                    │                     │                     │
    │ ... wait ...       │                     │                     │
    │                    │                     │                     │
    │ GET /jobs/{id}     │                     │                     │
    ├───────────────────>│                     │                     │
    │                    │                     │                     │
    │                    │ Query Job Status    │                     │
    │                    ├────────────────────>│                     │
    │                    │                     │                     │
    │ 200 OK             │                     │                     │
    │ + Completed        │                     │                     │
    │ + Result URL       │                     │                     │
    │<───────────────────┤                     │                     │
    │                    │                     │                     │
    │ GET /results/{id}  │                     │                     │
    ├───────────────────>│                     │                     │
    │                    │                     │                     │
    │ 200 OK             │                     │                     │
    │ + Result Data      │                     │                     │
    │<───────────────────┤                     │                     │
```

---

## 4. HTTP 202 Accepted Pattern Specification

### 4.1. Operation Initiation

#### 4.1.1. Client Request

**HTTP Request:**
```http
POST /api/v1/reports/generate HTTP/1.1
Host: api.example.com
Content-Type: application/json
Accept: application/json
Authorization: Bearer {token}
Idempotency-Key: a1b2c3d4-e5f6-7890-abcd-ef1234567890

{
  "type": "market-analysis",
  "parameters": {
    "region": "EU",
    "year": 2025,
    "format": "PDF"
  }
}
```

#### 4.1.2. Server Processing

**API Layer Actions:**
1. Validate request format and authentication
2. Generate unique job identifier (GUID)
3. Create job record in persistent storage
4. Enqueue job message to processing queue
5. Return 202 Accepted response immediately

**Processing Duration:** < 200ms (job creation only, not execution)

#### 4.1.3. HTTP 202 Response

**Response Specification:**
```http
HTTP/1.1 202 Accepted
Location: /api/v1/jobs/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91
Content-Type: application/json; charset=utf-8
X-Correlation-ID: 550e8400-e29b-41d4-a716-446655440000
Retry-After: 5

{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Queued",
  "statusUrl": "/api/v1/jobs/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "submittedAt": "2025-11-30T10:32:10Z"
}
```

**Required Headers:**

| Header | Value | Purpose |
|--------|-------|---------|
| Status Code | 202 Accepted | Indicates async processing |
| Location | /api/v1/jobs/{jobId} | Job status polling URL |
| Content-Type | application/json | Response format |
| X-Correlation-ID | {guid} | Request tracing |
| Retry-After | {seconds} | Suggested polling interval |

**Response Body Fields:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| jobId | string (GUID) | Yes | Unique job identifier |
| status | string | Yes | Current job status (initially "Queued") |
| statusUrl | string (URI) | Yes | Full URL for status polling |
| submittedAt | string (ISO 8601) | Yes | Job submission timestamp |
| estimatedDuration | integer | No | Estimated seconds to completion |

### 4.2. Status Code Selection

#### 4.2.1. Appropriate 202 Usage

**Use 202 Accepted When:**
- Operation will be processed asynchronously
- Processing may take more than 2 seconds
- Result will not be available in immediate response
- Client must poll for completion

**Do Not Use 202 When:**
- Operation completes immediately
- Synchronous processing appropriate
- Result available in response

#### 4.2.2. Alternative Status Codes

**201 Created:** When resource immediately created (synchronous)

**200 OK:** When operation completes synchronously

**400 Bad Request:** When request validation fails before job creation

---

## 5. Job Status Tracking

### 5.1. Status Polling Endpoint

#### 5.1.1. Endpoint Specification

**HTTP Request:**
```http
GET /api/v1/jobs/{jobId} HTTP/1.1
Host: api.example.com
Accept: application/json
Authorization: Bearer {token}
```

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| jobId | string (GUID) | Yes | Unique job identifier |

#### 5.1.2. Status Response - Queued

**HTTP Response:**
```http
HTTP/1.1 202 Accepted
Content-Type: application/json; charset=utf-8
Retry-After: 5

{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Queued",
  "submittedAt": "2025-11-30T10:32:10Z",
  "updatedAt": "2025-11-30T10:32:10Z",
  "queuePosition": 5
}
```

#### 5.1.3. Status Response - Running

**HTTP Response:**
```http
HTTP/1.1 202 Accepted
Content-Type: application/json; charset=utf-8
Retry-After: 5

{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Running",
  "progress": 45,
  "message": "Processing data from external systems...",
  "submittedAt": "2025-11-30T10:32:10Z",
  "startedAt": "2025-11-30T10:32:15Z",
  "updatedAt": "2025-11-30T10:32:30Z",
  "estimatedCompletionAt": "2025-11-30T10:33:00Z"
}
```

**Progress Field:**
- Type: Integer (0-100)
- Optional but recommended
- Percentage of completion
- Enables progress bar implementation

#### 5.1.4. Status Response - Completed

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Completed",
  "submittedAt": "2025-11-30T10:32:10Z",
  "startedAt": "2025-11-30T10:32:15Z",
  "completedAt": "2025-11-30T10:32:45Z",
  "duration": 35,
  "resultUrl": "/api/v1/reports/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91"
}
```

**Status Code Change:** 200 OK indicates job completion

#### 5.1.5. Status Response - Failed

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Failed",
  "submittedAt": "2025-11-30T10:32:10Z",
  "startedAt": "2025-11-30T10:32:15Z",
  "failedAt": "2025-11-30T10:32:25Z",
  "error": {
    "type": "ExternalServiceError",
    "message": "Unable to fetch supplier data.",
    "detail": "Timeout reaching ERP backend after 3 retry attempts.",
    "errorCode": "ERP_TIMEOUT",
    "retryable": false
  }
}
```

**Note:** Use 200 OK with error details in body, not 500/422, as job status retrieval succeeded.

#### 5.1.6. Status Response - Cancelled

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Cancelled",
  "submittedAt": "2025-11-30T10:32:10Z",
  "cancelledAt": "2025-11-30T10:32:20Z",
  "cancelledBy": "user",
  "reason": "User-initiated cancellation"
}
```

#### 5.1.7. Status Response - Expired

**HTTP Response:**
```http
HTTP/1.1 410 Gone
Content-Type: application/json; charset=utf-8

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.9",
  "title": "Job Expired",
  "status": 410,
  "detail": "This job is no longer available. Job results are retained for 7 days after completion.",
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "expiredAt": "2025-12-07T10:32:45Z"
}
```

**410 Gone:** Indicates job record deleted after retention period

### 5.2. Status Field Specification

#### 5.2.1. Standard Status Values

| Status | Description | HTTP Code | Polling Continue |
|--------|-------------|-----------|------------------|
| Queued | Job accepted, awaiting processing | 202 | Yes |
| Running | Job currently being processed | 202 | Yes |
| Completed | Job successfully completed | 200 | No |
| Failed | Job processing failed | 200 | No |
| Cancelled | Job cancelled by user or system | 200 | No |

#### 5.2.2. Optional Extended Status Values

| Status | Description | HTTP Code | Polling Continue |
|--------|-------------|-----------|------------------|
| Validating | Validating input parameters | 202 | Yes |
| Preparing | Preparing resources | 202 | Yes |
| Finalizing | Completing final steps | 202 | Yes |
| PartiallyCompleted | Completed with warnings | 200 | No |

### 5.3. Client Polling Guidelines

#### 5.3.1. Polling Strategy

**Recommended Pattern: Exponential Backoff**

```
Initial poll: After 2 seconds
Poll interval: Start at 2s, double until max 30s
Maximum interval: 30 seconds
Maximum duration: 5 minutes (for typical operations)

Polling sequence:
2s → 4s → 8s → 16s → 30s → 30s → 30s → ...
```

**Implementation:**
```csharp
public async Task<JobStatus> PollJobStatusAsync(Guid jobId)
{
    var delay = TimeSpan.FromSeconds(2);
    var maxDelay = TimeSpan.FromSeconds(30);
    var maxDuration = TimeSpan.FromMinutes(5);
    var stopwatch = Stopwatch.StartNew();

    while (stopwatch.Elapsed < maxDuration)
    {
        var status = await _client.GetJobStatusAsync(jobId);

        if (status.IsTerminal) // Completed, Failed, or Cancelled
            return status;

        await Task.Delay(delay);
        
        // Exponential backoff
        delay = TimeSpan.FromSeconds(
            Math.Min(delay.TotalSeconds * 2, maxDelay.TotalSeconds));
    }

    throw new TimeoutException("Job polling exceeded maximum duration");
}
```

#### 5.3.2. Retry-After Header Usage

**Server Provides Hint:**
```http
Retry-After: 5
```

**Client Should:**
- Respect Retry-After value if provided
- Use as minimum interval between polls
- Apply backoff strategy on top of base interval

---

## 6. Result Retrieval Patterns

### 6.1. Result Endpoint Specification

#### 6.1.1. Standard Result Retrieval

**HTTP Request:**
```http
GET /api/v1/reports/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91 HTTP/1.1
Host: api.example.com
Accept: application/pdf
Authorization: Bearer {token}
```

**HTTP Response (PDF):**
```http
HTTP/1.1 200 OK
Content-Type: application/pdf
Content-Disposition: attachment; filename="market-analysis-2025.pdf"
Content-Length: 2457600

[Binary PDF content]
```

**HTTP Response (JSON):**
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
  "reportId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "type": "market-analysis",
  "generatedAt": "2025-11-30T10:32:45Z",
  "data": {
    "summary": "...",
    "metrics": {...}
  }
}
```

#### 6.1.2. Large Result Handling

**Option 1: Blob Storage URL**

```json
{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Completed",
  "resultType": "file",
  "downloadUrl": "https://storage.example.com/reports/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91.pdf?expires=...",
  "expiresAt": "2025-11-30T11:32:45Z"
}
```

**Option 2: Streaming Response**

```http
GET /api/v1/reports/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91/download HTTP/1.1

HTTP/1.1 200 OK
Content-Type: application/octet-stream
Transfer-Encoding: chunked
```

### 6.2. Result Retention Policy

#### 6.2.1. Retention Configuration

| Result Type | Retention Period | Rationale |
|-------------|------------------|-----------|
| Small JSON results | 30 days | Low storage cost |
| Large files (< 10MB) | 7 days | Moderate storage cost |
| Large files (> 10MB) | 1 day | High storage cost |
| Failed jobs | 7 days | Debugging purposes |

**Configuration Example:**
```json
{
  "jobRetention": {
    "completedJobs": {
      "defaultDays": 7,
      "smallResultsDays": 30,
      "largeResultsDays": 1
    },
    "failedJobs": {
      "defaultDays": 7
    }
  }
}
```

#### 6.2.2. Cleanup Process

**Automated Cleanup:**
- Background job runs daily
- Deletes expired job records
- Removes associated result files
- Logs cleanup operations

---

## 7. Job Data Model Specification

### 7.1. Job Entity Schema

#### 7.1.1. Relational Database Schema

```sql
CREATE TABLE Jobs (
    JobId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Type NVARCHAR(100) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    Progress INT DEFAULT 0,
    Message NVARCHAR(500),
    
    SubmittedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    StartedAt DATETIME2,
    CompletedAt DATETIME2,
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ExpiresAt DATETIME2,
    
    SubmittedBy NVARCHAR(256),
    WorkerNodeId NVARCHAR(100),
    
    InputJson NVARCHAR(MAX),
    OutputJson NVARCHAR(MAX),
    ErrorJson NVARCHAR(MAX),
    
    IdempotencyKey NVARCHAR(100),
    CorrelationId NVARCHAR(100),
    
    RetryCount INT DEFAULT 0,
    MaxRetries INT DEFAULT 3,
    
    INDEX IX_Jobs_Status (Status, SubmittedAt),
    INDEX IX_Jobs_IdempotencyKey (IdempotencyKey),
    INDEX IX_Jobs_ExpiresAt (ExpiresAt)
);
```

#### 7.1.2. NoSQL Document Schema

```json
{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "type": "report-generation",
  "status": "Completed",
  "progress": 100,
  "message": "Report generated successfully",
  
  "timestamps": {
    "submittedAt": "2025-11-30T10:32:10Z",
    "startedAt": "2025-11-30T10:32:15Z",
    "completedAt": "2025-11-30T10:32:45Z",
    "updatedAt": "2025-11-30T10:32:45Z",
    "expiresAt": "2025-12-07T10:32:45Z"
  },
  
  "metadata": {
    "submittedBy": "user@example.com",
    "workerNodeId": "worker-01",
    "correlationId": "550e8400-e29b-41d4-a716-446655440000",
    "idempotencyKey": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
  },
  
  "input": {
    "type": "market-analysis",
    "parameters": {
      "region": "EU",
      "year": 2025
    }
  },
  
  "output": {
    "resultUrl": "/api/v1/reports/8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
    "fileSize": 2457600,
    "format": "PDF"
  },
  
  "error": null,
  
  "retry": {
    "count": 0,
    "maxRetries": 3
  }
}
```

### 7.2. Required Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| jobId | GUID | Yes | Unique job identifier |
| type | string | Yes | Job type classification |
| status | enum | Yes | Current job status |
| submittedAt | datetime | Yes | Job submission timestamp |
| inputJson | JSON | Yes | Job input parameters |
| submittedBy | string | No | User/system identifier |
| idempotencyKey | string | No | Idempotency key |

---

## 8. Response Format Standards

### 8.1. Standard Response Structure

#### 8.1.1. Queued/Running Response

```json
{
  "jobId": "string (GUID)",
  "status": "Queued|Running",
  "progress": "integer (0-100, optional)",
  "message": "string (optional)",
  "submittedAt": "string (ISO 8601)",
  "startedAt": "string (ISO 8601, optional)",
  "updatedAt": "string (ISO 8601)",
  "estimatedCompletionAt": "string (ISO 8601, optional)",
  "queuePosition": "integer (optional)"
}
```

#### 8.1.2. Completed Response

```json
{
  "jobId": "string (GUID)",
  "status": "Completed",
  "submittedAt": "string (ISO 8601)",
  "startedAt": "string (ISO 8601)",
  "completedAt": "string (ISO 8601)",
  "duration": "integer (seconds)",
  "resultUrl": "string (URI)",
  "resultType": "string (JSON|PDF|XLSX|etc)",
  "resultSize": "integer (bytes, optional)"
}
```

#### 8.1.3. Failed Response

```json
{
  "jobId": "string (GUID)",
  "status": "Failed",
  "submittedAt": "string (ISO 8601)",
  "startedAt": "string (ISO 8601, optional)",
  "failedAt": "string (ISO 8601)",
  "error": {
    "type": "string",
    "message": "string",
    "detail": "string",
    "errorCode": "string",
    "retryable": "boolean"
  },
  "retryCount": "integer",
  "maxRetries": "integer"
}
```

### 8.2. Error Format

**Use RFC 7807 Problem Details for job-level errors, not processing errors within job.**

**Job Not Found (404):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Job not found",
  "status": 404,
  "detail": "No job found with ID 8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "traceId": "00-550e8400e29b41d4a716446655440000-01"
}
```

---

## 9. Architecture Patterns

### 9.1. System Architecture

#### 9.1.1. Component Diagram

```
┌──────────────────────────────────────────────────────────┐
│                     Client Layer                         │
│  - Web Browser                                           │
│  - Mobile App                                            │
│  - External System                                       │
└────────────────┬─────────────────────────────────────────┘
                 │ HTTP/HTTPS
                 ↓
┌────────────────┴─────────────────────────────────────────┐
│              API Gateway / Load Balancer                 │
└────────────────┬─────────────────────────────────────────┘
                 │
                 ↓
┌────────────────┴─────────────────────────────────────────┐
│                   API Layer (ASP.NET Core)               │
│  ┌────────────────────────────────────────────────────┐  │
│  │  Async Operation Controllers                       │  │
│  │  - POST /reports/generate → Create Job             │  │
│  │  - GET /jobs/{id} → Query Status                   │  │
│  │  - GET /reports/{id} → Retrieve Result             │  │
│  └────────────┬───────────────────────────────────────┘  │
└───────────────┼──────────────────────────────────────────┘
                │
                ↓
┌───────────────┴──────────────────────────────────────────┐
│                Application Layer (CQRS)                  │
│  - CreateReportJobCommand                                │
│  - GetJobStatusQuery                                     │
│  - Job record creation                                   │
│  - Message queue publishing                              │
└────────────┬─────────────────┬─────────────────────────┬─┘
             │                 │                         │
             ↓                 ↓                         ↓
┌────────────┴────┐  ┌─────────┴──────────┐  ┌──────────┴───────┐
│   Job Store     │  │  Message Queue     │  │  Result Storage  │
│  (Database)     │  │  - RabbitMQ        │  │  - Blob Storage  │
│                 │  │  - Azure Queue     │  │  - File System   │
│                 │  │  - AWS SQS         │  │                  │
└─────────────────┘  └─────────┬──────────┘  └──────────────────┘
                               │
                               ↓
                     ┌─────────┴──────────┐
                     │  Background Worker │
                     │  Service           │
                     │  - Poll queue      │
                     │  - Process jobs    │
                     │  - Update status   │
                     │  - Store results   │
                     └────────────────────┘
```

### 9.2. Processing Flow

#### 9.2.1. Job Creation Flow

```
1. Client → API: POST /api/v1/reports/generate
2. API validates request
3. API generates job ID
4. API creates job record in database (Status: Queued)
5. API publishes message to queue
6. API returns 202 Accepted with job ID
7. Worker dequeues message
8. Worker updates job status to Running
9. Worker executes processing logic
10. Worker stores result
11. Worker updates job status to Completed
```

### 9.3. Reliability Patterns

#### 9.3.1. Retry Logic

**Worker-Side Retry:**
```csharp
public async Task ProcessJobAsync(Job job)
{
    var maxRetries = job.MaxRetries;
    var retryCount = 0;

    while (retryCount <= maxRetries)
    {
        try
        {
            await ExecuteJobLogicAsync(job);
            await UpdateJobStatusAsync(job.Id, JobStatus.Completed);
            return;
        }
        catch (TransientException ex)
        {
            retryCount++;
            _logger.LogWarning(
                ex,
                "Job {JobId} failed (attempt {Attempt}/{MaxRetries})",
                job.Id,
                retryCount,
                maxRetries);

            if (retryCount > maxRetries)
            {
                await UpdateJobStatusAsync(
                    job.Id,
                    JobStatus.Failed,
                    error: ex.Message);
                throw;
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)));
        }
    }
}
```

#### 9.3.2. Dead Letter Queue

**Configuration:**
- Failed messages moved to dead letter queue after max retries
- Separate monitoring for DLQ
- Manual intervention process for DLQ items
- Alerting on DLQ threshold

---

## 10. Implementation Technologies

### 10.1. Cloud-Based Solutions

#### 10.1.1. Microsoft Azure

**Components:**
- **Azure Queue Storage:** Simple message queue
- **Azure Service Bus:** Enterprise messaging
- **Azure Functions:** Serverless worker execution
- **Durable Functions:** Stateful workflow orchestration
- **Azure Container Apps Jobs:** Container-based job execution

**Example Configuration:**
```csharp
services.AddAzureClients(builder =>
{
    builder.AddQueueServiceClient(configuration["Azure:Storage:ConnectionString"]);
});

services.AddHostedService<AzureQueueWorker>();
```

#### 10.1.2. Amazon Web Services

**Components:**
- **Amazon SQS:** Message queue service
- **Amazon SNS:** Pub/sub messaging
- **AWS Lambda:** Serverless functions
- **AWS Step Functions:** Workflow orchestration
- **Amazon ECS/Fargate:** Container-based workers

#### 10.1.3. Google Cloud Platform

**Components:**
- **Cloud Tasks:** Managed task queue
- **Cloud Pub/Sub:** Messaging service
- **Cloud Functions:** Serverless execution
- **Cloud Run Jobs:** Container execution
- **Workflows:** Serverless orchestration

### 10.2. Self-Hosted Solutions

#### 10.2.1. Hangfire

**Features:**
- .NET native integration
- Persistent job storage
- Dashboard UI
- Cron-style scheduling

**Configuration:**
```csharp
services.AddHangfire(configuration =>
{
    configuration.UseSqlServerStorage(connectionString);
});

services.AddHangfireServer();

// Enqueue job
BackgroundJob.Enqueue<ReportGenerator>(x => x.GenerateAsync(jobId));
```

#### 10.2.2. Quartz.NET

**Features:**
- Enterprise job scheduling
- Clustering support
- Persistent job stores
- Rich trigger system

#### 10.2.3. RabbitMQ + Worker Services

**Configuration:**
```csharp
// Worker Service
public class JobWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var jobId = Encoding.UTF8.GetString(ea.Body.ToArray());
            await ProcessJobAsync(jobId);
        };

        _channel.BasicConsume(queue: "jobs", autoAck: false, consumer: consumer);
    }
}
```

#### 10.2.4. MassTransit

**Features:**
- Message bus abstraction
- Multiple transport support
- Saga state machines
- Retry and error handling

---

## 11. Idempotency Requirements

### 11.1. Idempotency Pattern

#### 11.1.1. Idempotency Key Header

**Client Request:**
```http
POST /api/v1/reports/generate HTTP/1.1
Idempotency-Key: a1b2c3d4-e5f6-7890-abcd-ef1234567890
Content-Type: application/json

{
  "type": "market-analysis",
  "parameters": {...}
}
```

**Purpose:**
- Prevent duplicate job creation
- Enable safe retries
- Support at-least-once delivery

#### 11.1.2. Implementation Logic

```csharp
public async Task<IActionResult> CreateJob(
    CreateJobRequest request,
    [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
{
    if (!string.IsNullOrEmpty(idempotencyKey))
    {
        // Check for existing job with same idempotency key
        var existingJob = await _jobRepository
            .GetByIdempotencyKeyAsync(idempotencyKey);

        if (existingJob != null)
        {
            // Return existing job instead of creating duplicate
            return StatusCode(202, new JobResponse
            {
                JobId = existingJob.Id,
                Status = existingJob.Status,
                StatusUrl = $"/api/v1/jobs/{existingJob.Id}",
                SubmittedAt = existingJob.SubmittedAt
            });
        }
    }

    // Create new job
    var job = await _jobService.CreateJobAsync(request, idempotencyKey);
    
    return StatusCode(202, new JobResponse
    {
        JobId = job.Id,
        Status = job.Status,
        StatusUrl = $"/api/v1/jobs/{job.Id}",
        SubmittedAt = job.SubmittedAt
    });
}
```

### 11.2. Idempotency Key Management

#### 11.2.1. Key Generation

**Client Responsibility:**
- Generate unique GUID for each logical operation
- Reuse key for retries of same operation
- Store key with operation context

**Example:**
```typescript
// Client-side
const idempotencyKey = generateUUID();
localStorage.setItem('report-generation-key', idempotencyKey);

await fetch('/api/v1/reports/generate', {
  method: 'POST',
  headers: {
    'Idempotency-Key': idempotencyKey,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify(request)
});
```

#### 11.2.2. Key Retention

**Database Retention:**
- Store idempotency key with job record
- Retain for duration of job lifecycle
- Clean up after job expiration

**Index:**
```sql
CREATE UNIQUE INDEX IX_Jobs_IdempotencyKey 
ON Jobs(IdempotencyKey) 
WHERE IdempotencyKey IS NOT NULL;
```

---

## 12. Error Handling for Async Operations

### 12.1. Job Submission Errors

#### 12.1.1. Validation Errors (400 Bad Request)

**Before Job Creation:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "parameters.year": ["Year must be between 2000 and 2030"]
  }
}
```

#### 12.1.2. Authentication Errors (401 Unauthorized)

**Standard RFC 7807 Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authentication credentials are required or invalid."
}
```

### 12.2. Job Processing Errors

#### 12.2.1. Error Information in Job Status

**Failed Job Response:**
```json
{
  "jobId": "8f9a1d22-cc31-4a3c-9345-d16f42f4bb91",
  "status": "Failed",
  "error": {
    "type": "ExternalServiceError",
    "message": "Unable to connect to data source",
    "detail": "Connection to https://data.example.com timed out after 30 seconds",
    "errorCode": "EXT_SERVICE_TIMEOUT",
    "retryable": true,
    "timestamp": "2025-11-30T10:32:25Z"
  }
}
```

#### 12.2.2. Retryable vs. Non-Retryable Errors

**Retryable Errors:**
- Network timeouts
- Transient database errors
- Rate limit exceeded
- Temporary service unavailability

**Non-Retryable Errors:**
- Invalid input data
- Business rule violations
- Authentication failures
- Permanent resource not found

---

## 13. Testing Requirements

### 13.1. Unit Tests

#### 13.1.1. Job Creation Tests

```csharp
[Fact]
public async Task CreateJob_ValidRequest_Returns202WithJobId()
{
    // Arrange
    var request = new CreateReportRequest
    {
        Type = "market-analysis",
        Parameters = new { Region = "EU", Year = 2025 }
    };

    // Act
    var response = await _controller.CreateReport(request, null);

    // Assert
    var result = Assert.IsType<ObjectResult>(response);
    Assert.Equal(202, result.StatusCode);
    
    var jobResponse = Assert.IsType<JobResponse>(result.Value);
    Assert.NotEqual(Guid.Empty, jobResponse.JobId);
    Assert.Equal("Queued", jobResponse.Status);
}
```

#### 13.1.2. Idempotency Tests

```csharp
[Fact]
public async Task CreateJob_SameIdempotencyKey_ReturnsSameJob()
{
    // Arrange
    var idempotencyKey = Guid.NewGuid().ToString();
    var request = new CreateReportRequest { Type = "test" };

    // Act
    var response1 = await _controller.CreateReport(request, idempotencyKey);
    var response2 = await _controller.CreateReport(request, idempotencyKey);

    // Assert
    var job1 = ((ObjectResult)response1).Value as JobResponse;
    var job2 = ((ObjectResult)response2).Value as JobResponse;
    
    Assert.Equal(job1.JobId, job2.JobId);
}
```

### 13.2. Integration Tests

#### 13.2.1. End-to-End Job Flow

```csharp
[Fact]
public async Task JobFlow_CreatePollRetrieve_CompletesSuccessfully()
{
    // 1. Create job
    var createResponse = await _client.PostAsJsonAsync(
        "/api/v1/reports/generate",
        new { type = "test-report" });
    
    Assert.Equal(HttpStatusCode.Accepted, createResponse.StatusCode);
    
    var jobResponse = await createResponse.Content
        .ReadFromJsonAsync<JobResponse>();
    
    Assert.NotNull(jobResponse);
    var jobId = jobResponse.JobId;

    // 2. Poll until completed
    JobStatusResponse status;
    var attempts = 0;
    do
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
        
        var statusResponse = await _client.GetAsync($"/api/v1/jobs/{jobId}");
        status = await statusResponse.Content
            .ReadFromJsonAsync<JobStatusResponse>();
        
        attempts++;
        Assert.True(attempts < 30, "Job did not complete within timeout");
    }
    while (status.Status == "Queued" || status.Status == "Running");

    // 3. Verify completion
    Assert.Equal("Completed", status.Status);
    Assert.NotNull(status.ResultUrl);

    // 4. Retrieve result
    var resultResponse = await _client.GetAsync(status.ResultUrl);
    Assert.Equal(HttpStatusCode.OK, resultResponse.StatusCode);
}
```

### 13.3. Load Tests

#### 13.3.1. Concurrent Job Creation

**Test Scenario:**
- Create 100 jobs concurrently
- Verify all return 202
- Verify all have unique job IDs
- Verify all jobs eventually process

---

## 14. Performance and Scalability

### 14.1. Performance Targets

#### 14.1.1. API Response Times

| Operation | Target p50 | Target p95 | Target p99 |
|-----------|-----------|-----------|-----------|
| Job creation (POST) | < 100ms | < 200ms | < 500ms |
| Status poll (GET) | < 50ms | < 100ms | < 200ms |
| Result retrieval (GET) | < 200ms | < 500ms | < 1000ms |

#### 14.1.2. Throughput Targets

**Job Creation:**
- Target: > 500 jobs/second per instance
- Bottleneck: Database write performance

**Status Polling:**
- Target: > 2000 requests/second per instance
- Optimization: Read replicas, caching

### 14.2. Scalability Considerations

#### 14.2.1. Horizontal Scaling

**API Layer:**
- Stateless design enables horizontal scaling
- Load balancer distributes requests
- No session affinity required

**Worker Layer:**
- Multiple worker instances process queue
- Competing consumer pattern
- Dynamic scaling based on queue depth

#### 14.2.2. Queue Depth Monitoring

**Metrics:**
- Current queue depth
- Messages per second (enqueue rate)
- Processing rate (dequeue rate)
- Average message age

**Auto-Scaling Trigger:**
```
IF queue_depth > 100 AND message_age > 60s
THEN scale_workers(+2)
```

---

## 15. Glossary

**Asynchronous Operation:** An operation that completes in the background after the initial HTTP request returns.

**Background Worker:** A service that processes queued jobs independently of the API request-response cycle.

**Dead Letter Queue (DLQ):** A queue for messages that cannot be processed successfully after maximum retry attempts.

**Idempotency Key:** A unique identifier provided by clients to ensure duplicate requests produce the same result.

**Job:** A unit of work representing an asynchronous operation with trackable status and retrievable result.

**Long-Running Operation:** An operation that exceeds typical HTTP timeout thresholds, requiring asynchronous processing.

**Polling:** The practice of repeatedly checking job status at intervals until completion.

**Retry-After:** HTTP header indicating the minimum time before the next status poll.

**Transient Error:** A temporary error condition that may succeed on retry (network timeout, temporary unavailability).

---

## 16. Recommendations and Next Steps

### 16.1. For Development Teams

#### 16.1.1. Implementation Checklist

**When Implementing Async Endpoints:**
- [ ] Create job record before queuing message
- [ ] Return 202 Accepted with Location header
- [ ] Include Retry-After header with suggested interval
- [ ] Implement status polling endpoint
- [ ] Implement result retrieval endpoint
- [ ] Support idempotency keys
- [ ] Add comprehensive error handling
- [ ] Implement job expiration and cleanup
- [ ] Write unit and integration tests
- [ ] Document expected processing duration

**Queue Integration:**
- [ ] Configure message queue connection
- [ ] Implement worker service
- [ ] Add retry logic with exponential backoff
- [ ] Configure dead letter queue
- [ ] Add monitoring and alerting
- [ ] Implement graceful shutdown

### 16.2. For Operations Teams

#### 16.2.1. Monitoring Requirements

**Key Metrics:**
- Job creation rate
- Queue depth and message age
- Worker processing rate
- Job completion rate
- Job failure rate
- Average processing duration
- P50/P95/P99 processing times

**Alerting:**
- Queue depth > threshold (e.g., 1000 messages)
- Message age > threshold (e.g., 5 minutes)
- Failure rate > threshold (e.g., 5%)
- Worker unavailability
- Dead letter queue not empty

### 16.3. For Architects

#### 16.3.1. Architecture Reviews

**Periodic Assessment:**
- Job pattern usage appropriateness
- Queue technology suitability
- Worker scaling effectiveness
- Result storage costs
- Job retention policy adequacy

**Evolution Planning:**
- Consider workflow orchestration (Azure Durable Functions, AWS Step Functions)
- Evaluate event-driven architecture patterns
- Assess real-time notification needs (SignalR, WebSockets)
- Review distributed tracing integration

### 16.4. Related Documentation

**Must Read:**
- API Guidelines (API-GUIDELINES-001)
- Error Handling Specification (API-ERRORHANDLING-001)
- API Flow Specification (ARCH-API-003)

**Recommended Reading:**
- Background Processing Best Practices
- Message Queue Configuration Guide
- Distributed System Patterns
- Idempotency Implementation Guide

---

## 17. References

### 17.1. Standards and Specifications

**HTTP/1.1 Specification**
- RFC 7231 Section 6.3.3: 202 Accepted - https://tools.ietf.org/html/rfc7231#section-6.3.3

**RFC 7807 - Problem Details for HTTP APIs**
- https://tools.ietf.org/html/rfc7807

### 17.2. Industry Patterns

**Azure Long-Running Operations**
- https://docs.microsoft.com/azure/architecture/patterns/async-request-reply

**AWS Asynchronous API Patterns**
- https://docs.aws.amazon.com/apigateway/latest/developerguide/api-gateway-api-integration-types.html

**Google Cloud Long-Running Operations**
- https://cloud.google.com/apis/design/design_patterns#long_running_operations

**Kubernetes Asynchronous Operations**
- https://kubernetes.io/docs/reference/using-api/api-concepts/#asynchronous-operations

### 17.3. Framework Documentation

**Hangfire**
- https://www.hangfire.io/

**Quartz.NET**
- https://www.quartz-scheduler.net/

**MassTransit**
- https://masstransit-project.com/

**Azure Durable Functions**
- https://docs.microsoft.com/azure/azure-functions/durable/

### 17.4. Internal Documentation

**Project Repository:**
- https://github.com/stefanogiacchi/dotnet-enterprise-architecture-template

**Related Specifications:**
- ARCH-HL-001: High-Level Architecture
- ARCH-API-003: API Flow Specification
- API-GUIDELINES-001: API Design Guidelines
- API-ERRORHANDLING-001: Error Handling and Problem Details

---

## Document Revision History

| Version | Date | Author | Description |
|---------|------|--------|-------------|
| 1.0 | 2025 | Enterprise Architecture Team | Initial asynchronous operations specification with standardized structure |

---

**END OF DOCUMENT**