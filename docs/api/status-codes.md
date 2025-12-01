Perfetto.
Ecco **`docs/api/status-codes.md`**, scritto in stile **enterprise**, con una **matrice decisionale completa**, uguale a quella usata da Azure, Microsoft Graph, Stripe, AWS e GitHub.

È uno dei file più importanti dell’intera documentazione.

---

```markdown
# HTTP Status Codes — Decision Matrix  
.NET Enterprise Architecture Template

This document defines the **official status code mapping** for all APIs  
implemented in the `.NET Enterprise Architecture Template`.

The goal is to ensure:

- Predictable responses  
- Consistent error semantics  
- Proper use of HTTP standards  
- Clear expectations for client applications  
- Full alignment with REST best practices  
- Enterprise-grade interoperability  

---

# 🧭 Guiding Principles

- **Use standard HTTP semantics**  
- **Never overload status codes** (keep meaning consistent)
- **Use ProblemDetails (RFC 7807)** for all errors  
- **Avoid business logic encoded in status codes**  
- **Prefer 4xx for client issues, 5xx for server issues**

---

# 🟢 2xx Success Codes

## **200 OK**
Used when a request is successfully processed and returns data.

Examples:
- GET resource
- GET collection (paginated)
- PATCH with response body
- PUT returning updated resource
- Command that returns result information

---

## **201 Created**
Used for successful resource creation.

Rules:
- MUST include `Location: /api/.../{id}`
- Response body SHOULD include the created resource or `id`

Example:
```

POST /products → 201 Created
Location: /api/products/123

```

---

## **202 Accepted**
Used for **asynchronous operations** that will continue in background.

Used when:
- Long-running operations
- Background jobs (reports, batches, AI workflows)
- External system sync

Response includes:
- Job ID
- Location header pointing to job status

See: `api/async-operations.md`

---

## **204 No Content**
Used when the operation succeeds but returns **no body**.

Examples:
- DELETE
- PUT without response
- Successful state transitions

---

# 🟡 3xx Redirects

Redirect codes are NOT used by this API unless:

- OAuth2 login flows  
- External redirects through API Gateway  

Under normal operation:

❌ Avoid 3xx responses in internal microservices.

---

# 🔴 4xx Client Errors

All 4xx errors MUST return **ProblemDetails**.

---

## **400 Bad Request**
Used when:

- Request is malformed
- Model validation fails
- Query parameters invalid
- Unsupported filtering/sorting/pagination
- Invalid enum, invalid value, invalid format
- Business rule violated by malformed input

Examples:
- `pageSize=10000`
- `fields=doesNotExist`
- `sort=unknown_field`

---

## **401 Unauthorized**
Used when:

- Authentication token missing
- Token expired
- Invalid signature
- Unsupported authentication scheme

Important:
- **Do NOT** use 401 for missing permissions → use 403 instead.

---

## **403 Forbidden**
Used when:

- User authenticated
- User **does not have permission** to perform action

Examples:
- Trying to delete a resource without required role
- Accessing another user’s resource without authorization rule

---

## **404 Not Found**
Use only when:

- Resource does not exist  
- ID is valid but resource missing  
- Parent resource not found (`/orders/{id}/items`)

Important:
- MUST NOT reveal existence of restricted resources
- SHOULD NOT be used for authorization errors

---

## **405 Method Not Allowed**
Used when the right URL is hit using the wrong HTTP method.

Example:
```

PATCH /reports → 405

```

---

## **409 Conflict**
Used when:

- Business invariants block the operation
- Resource already exists (duplicate)
- Concurrent update conflict
- State transition forbidden

Examples:
- Attempt to update product price but ETag mismatch
- Duplicate SKU or email
- Cannot cancel an order already shipped

---

## **410 Gone**
Used when:

- Resource existed but was deleted  
- Async job expired (retention window)  
- Soft-deleted access after TTL

Used for:

- Expired background jobs  
- Deleted files in object storage  
- Removed endpoints (rare)

---

## **415 Unsupported Media Type**
Used when:

- Content-Type is not supported  
- Only JSON accepted

Example:

```

Content-Type: text/xml → 415

````

---

## **422 Unprocessable Entity**
Used for **semantic validation errors**, not syntactic ones.

Examples:
- Domain rule violation  
- Invalid price range  
- StartDate > EndDate  
- Business validation inside commands  

Difference from *400 Bad Request*:

| 400 Bad Request | 422 Unprocessable Entity |
|-----------------|--------------------------|
| Invalid structure | Valid JSON but invalid semantics |
| Invalid fields | Rule violations |
| Parsing errors | Business-level validation |

---

## **429 Too Many Requests**
Used when:

- Rate limiting triggered  
- Client exceeds throttling rules  
- API Gateway policies enforce limits  

Using headers:

- `Retry-After`
- `X-RateLimit-Remaining`
- `X-RateLimit-Reset`

---

# 🔥 5xx Server Errors

Use **5xx** only for server-side issues.

NEVER use 5xx to communicate business logic.

---

## **500 Internal Server Error**
Used when:

- Unexpected exceptions  
- Null reference  
- Infrastructure failure  
- Dependency failure without categorization  
- Any unhandled exception  

Always return ProblemDetails with:

```json
{
  "type": "https://httpstatuses.com/500",
  "title": "An unexpected error occurred.",
  "status": 500,
  "traceId": "00-abc..."
}
````

---

## **502 Bad Gateway**

Used when:

* API Gateway / Reverse Proxy cannot reach service
* External dependency returned non-HTTP/OK
* Wrong upstream protocol

---

## **503 Service Unavailable**

Used when:

* System overload
* Maintenance window
* Database temporarily unavailable
* Worker pool exhausted
* External provider offline

Must include:

```
Retry-After: 30
```

---

## **504 Gateway Timeout**

Used when:

* API gateway or proxy times out
* Long-running downstream task
* Cloud function exceeds max duration

Do NOT use 504 for:
❌ Background async workflows
→ use 202 + job pattern instead.

---

# 🧩 Status Code Matrix — Summary

| Category      | Code | Meaning                   |
| ------------- | ---- | ------------------------- |
| Success       | 200  | Standard success          |
|               | 201  | Resource created          |
|               | 202  | Async accepted            |
|               | 204  | No content                |
| Client Errors | 400  | Invalid request           |
|               | 401  | Unauthorized              |
|               | 403  | Forbidden                 |
|               | 404  | Not found                 |
|               | 405  | Wrong HTTP method         |
|               | 409  | Conflict                  |
|               | 410  | Gone                      |
|               | 415  | Unsupported media type    |
|               | 422  | Semantic validation error |
|               | 429  | Rate limit exceeded       |
| Server Errors | 500  | Server crash / bug        |
|               | 502  | Bad gateway               |
|               | 503  | Service unavailable       |
|               | 504  | Gateway timeout           |

---

# 🧪 Testing Expectations

API tests MUST verify:

* Every endpoint returns a predictable status code
* No controller returns mixed codes (e.g. sometimes 200, sometimes 204)
* Commands return only 200/201/202/204
* Queries return only 200
* All errors return RFC 7807 ProblemDetails
* 404 for missing resources
* 422 for business rule violations

---

# 📌 Summary

The `.NET Enterprise Architecture Template` enforces strict, modern HTTP semantics:

* **2xx** = success
* **4xx** = client errors
* **5xx** = server errors
* **ProblemDetails everywhere for errors**
* **202 Accepted pattern for async workflows**

This ensures:

* Predictable client integration
* Cleaner code
* Easier testing
* Faster debugging
* Professional-grade API behavior

---

## 📎 Related Documents

* `api/api-guidelines.md`
* `api/problem-details.md`
* `api/rest-conventions.md`
* `api/async-operations.md`
* `api/pagination-filtering-sorting.md`
* `architecture/03-api-flow.md`

```

---

Vuoi ora:

### ✔ `docs/api/headers.md` — correlation-id, idempotency-key, caching, rate-limits  
### ✔ `docs/api/resource-naming.md`  
### ✔ `docs/api/examples.md` (full request/response examples per pattern)  

Quale generiamo?
```
