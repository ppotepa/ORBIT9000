# Orca Public API Reference

## Overview
**Base URL:** `https://api.orca.so/v2/{chain}`  
**Version:** 1.0.0  
**Description:** A comprehensive list of public endpoints for the Orca protocol on Solana.

**Contact:**
- **Team:** Orca Team
- **Website:** https://www.orca.so
- **Email:** team@orca.so

**Supported Chains:**
- `solana` - Solana mainnet
- `eclipse` - Eclipse chain

---

## API Categories

### Health
Health and diagnostic endpoints for service monitoring.

### Protocol
Orca protocol information and statistics endpoints.

### Tokens
Token information and metadata endpoints.

### Whirlpools
Whirlpool (concentrated liquidity) information endpoints.

---

## Endpoints

### Whirlpool Endpoints

#### GET `/lock/{address}`
Get locked liquidity for a specific whirlpool.

**Path Parameters:**
- `address` (required): String - The Solana account address of the whirlpool

**Description:** Returns the locked liquidity information for the specified whirlpool.

**Response:** Array of `LockInfo` objects

**Example:**
```bash
GET /v2/solana/lock/HJPjoWUrhoZzkNfRpHuieeFk9WcZWjwy6PBjZ81ngndJ
```

---

#### GET `/pools`
List whirlpools with optional filtering and pagination.

**Query Parameters:**
- `sortBy` (optional): SortField - Field to sort whirlpools by
  - Options: `tvl`, `volume_24h`, `fees_24h`, `apr`
- `sortDirection` (optional): SortDirection - Sort direction
  - Options: `asc`, `desc`
- `limit` (optional): Number - Number of results to return
- `offset` (optional): Number - Number of results to skip
- `tokenA` (optional): String - Filter by token A mint address
- `tokenB` (optional): String - Filter by token B mint address
- `hasRewards` (optional): Boolean - Filter whirlpools that have rewards
- `hasWarning` (optional): Boolean - Filter whirlpools that have warnings
- `useAdaptiveFee` (optional): Boolean - Filter whirlpools using adaptive fees
- `minTvl` (optional): Number - Minimum TVL in USDC
- `minVolume` (optional): Number - Minimum 24h volume in USDC
- `minLockedLiquidityPct` (optional): Number - Minimum locked liquidity percentage
- `token` (optional): String - Filter whirlpools containing this token
- `tokensBothOf` (optional): String - Filter whirlpools containing both specified tokens

**Response:** Array of `PublicWhirlpool` objects

**Example:**
```bash
GET /v2/solana/pools?sortBy=tvl&sortDirection=desc&limit=10
```

---

#### GET `/pools/search`
Search for whirlpools by token symbols or addresses.

**Query Parameters:**
- `query` (required): String - Search query

**Description:** Search for whirlpools by:
- Token symbols (e.g., "SOL USDC" to find pools with both tokens)
- Single token symbol (e.g., "SOL" to find all pools containing SOL)
- Whirlpool address (exact match)

The search is case-insensitive and supports partial matching for token symbols.

**Response:** Array of `PublicWhirlpool` objects

**Example:**
```bash
GET /v2/solana/pools/search?query=SOL USDC
```

---

#### GET `/pools/{address}`
Get whirlpool data by address.

**Path Parameters:**
- `address` (required): String - Whirlpool address

**Response:** `PublicWhirlpool` object

**Example:**
```bash
GET /v2/solana/pools/HJPjoWUrhoZzkNfRpHuieeFk9WcZWjwy6PBjZ81ngndJ
```

---

### Protocol Endpoints

#### GET `/protocol`
Get recent protocol statistics for Orca.

**Description:** Returns general information about the Orca protocol including:
- Total Value Locked (TVL) in USDC
- 24-hour trading volume in USDC
- 24-hour fees collected in USDC
- 24-hour protocol revenue in USDC

**Response:** `ProtocolInfo` object

**Example:**
```bash
GET /v2/solana/protocol
```

**Response Format:**
```json
{
  "tvl": 1234567.89,
  "volume_24h": 987654.32,
  "fees_24h": 12345.67,
  "revenue_24h": 6172.84
}
```

---

#### GET `/protocol/token`
Get information about the Orca token.

**Description:** Returns detailed information about the Orca token, including:
- Token symbol, name, and description
- Token image URL
- Current price in USDC
- Circulating and total supply
- 24-hour trading volume statistics

**Response:** `ProtocolInfoOrcaToken` object

**Example:**
```bash
GET /v2/solana/protocol/token
```

---

#### GET `/protocol/token/circulating_supply`
Get circulating supply of the Orca token.

**Description:** Returns the circulating supply calculated as total supply minus tokens held in excluded addresses (treasury, grants, etc.).

**Response:** `CirculatingSupplyResponse` object

**Example:**
```bash
GET /v2/solana/protocol/token/circulating_supply
```

---

#### GET `/protocol/token/total_supply`
Get total supply of the Orca token.

**Description:** Returns the total supply of all minted tokens, including those in excluded addresses. The response is adjusted for token decimals (6 decimal places) and rounded up to the nearest whole number.

**Response:** `TotalSupplyResponse` object

**Example:**
```bash
GET /v2/solana/protocol/token/total_supply
```

---

### Token Endpoints

#### GET `/tokens`
List tokens with pagination and filtering options.

**Query Parameters:**
- `limit` (optional): Number - Number of tokens to return (default: 50, max: 100)
- `offset` (optional): Number - Number of tokens to skip
- `sortBy` (optional): TokenSortField - Field to sort by
  - Options: `symbol`, `name`, `tvl`, `volume_24h`
- `sortDirection` (optional): TokenSortDirection - Sort direction
  - Options: `asc`, `desc`
- `search` (optional): String - Search query for token name or symbol

**Description:** Returns a paginated list of tokens with optional filtering and sorting.

**Response:** `UtoipaApiResponse_Vec_PublicToken` object

**Example:**
```bash
GET /v2/solana/tokens?limit=20&sortBy=tvl&sortDirection=desc
```

---

#### GET `/tokens/search`
Search for tokens by query string.

**Query Parameters:**
- `query` (required): String - Search query

**Description:** Returns tokens that match the query string against token name, symbol, and address.

**Response:** Array of `PublicToken` objects

**Example:**
```bash
GET /v2/solana/tokens/search?query=USDC
```

---

#### GET `/tokens/{mint_address}`
Get token details by mint address.

**Path Parameters:**
- `mint_address` (required): String - Token mint address

**Description:** Returns detailed information for a specific token identified by its mint address.

**Response:** `PublicToken` object

**Example:**
```bash
GET /v2/solana/tokens/EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v
```

---

## Data Models

### Core Types

#### PublicWhirlpool
A concentrated liquidity pool with metadata and statistics.

**Key Fields:**
- `address`: String - Whirlpool address
- `tokenA`: PublicToken - First token in the pair
- `tokenB`: PublicToken - Second token in the pair
- `tickSpacing`: Number - Tick spacing for the pool
- `price`: String - Current price
- `sqrtPrice`: String - Square root price
- `tickCurrentIndex`: Number - Current tick index
- `tvl`: Number - Total Value Locked in USDC
- `volume24h`: Number - 24-hour trading volume in USDC
- `fees24h`: Number - 24-hour fees in USDC
- `apr`: Number - Annual Percentage Rate
- `feeRate`: Number - Fee rate (in basis points)
- `protocolFeeRate`: Number - Protocol fee rate
- `liquidity`: String - Current liquidity
- `reward`: Array - Reward information
- `whirlpoolsConfig`: String - Configuration address
- `hasRewards`: Boolean - Whether pool has rewards
- `isVerified`: Boolean - Whether tokens are verified

#### PublicToken
Token information for whirlpools.

**Key Fields:**
- `mint`: String - Token mint address
- `symbol`: String - Token symbol
- `name`: String - Token name
- `decimals`: Number - Token decimal places
- `logoURI`: String - Token logo URL
- `coingeckoId`: String - CoinGecko ID
- `whirlpoolsCount`: Number - Number of whirlpools containing this token
- `tvl`: Number - Total TVL across all pools
- `volume24h`: Number - 24-hour volume across all pools
- `tags`: Array - Token tags (e.g., "verified", "lst")

#### LockInfo
Information about locked liquidity positions.

**Key Fields:**
- `name`: String - Lock name/identifier
- `amount`: String - Locked amount
- `lockedUntil`: Number - Unix timestamp when lock expires
- `owner`: String - Owner address

#### ProtocolInfo
Protocol-wide statistics and information.

**Key Fields:**
- `tvl`: Number - Total Value Locked in USDC
- `volume24h`: Number - 24-hour trading volume
- `fees24h`: Number - 24-hour fees collected
- `revenue24h`: Number - 24-hour protocol revenue

---

## Enums and Constants

### SortField
Fields available for sorting whirlpools:
- `tvl` - Total Value Locked
- `volume_24h` - 24-hour volume
- `fees_24h` - 24-hour fees
- `apr` - Annual Percentage Rate

### SortDirection
Sort direction options:
- `asc` - Ascending order
- `desc` - Descending order

### TokenSortField
Fields available for sorting tokens:
- `symbol` - Token symbol
- `name` - Token name
- `tvl` - Total Value Locked
- `volume_24h` - 24-hour volume

### PoolType
Types of pools:
- `concentrated` - Concentrated liquidity pools (Whirlpools)

---

## Usage Examples

### Get Top TVL Whirlpools
```bash
curl "https://api.orca.so/v2/solana/pools?sortBy=tvl&sortDirection=desc&limit=10"
```

### Search for SOL Pools
```bash
curl "https://api.orca.so/v2/solana/pools/search?query=SOL"
```

### Get Protocol Statistics
```bash
curl "https://api.orca.so/v2/solana/protocol"
```

### Get Token Information
```bash
curl "https://api.orca.so/v2/solana/tokens/EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v"
```

### Get Orca Token Supply
```bash
curl "https://api.orca.so/v2/solana/protocol/token/total_supply"
```

---

## Error Handling

The API returns standard HTTP status codes:

- `200` - Success
- `400` - Bad Request (invalid parameters)
- `404` - Not Found (resource doesn't exist)
- `500` - Internal Server Error

**Error Response Format:**
```json
{
  "status": 400,
  "message": "Invalid parameter: sortBy must be one of: tvl, volume_24h, fees_24h, apr",
  "code": "INVALID_PARAMETER"
}
```

---

## Rate Limiting

The API implements rate limiting to ensure fair usage. Please respect these limits and implement appropriate retry logic with exponential backoff for production applications.

---

## Pagination

List endpoints support cursor-based pagination:

**Request:**
```bash
GET /v2/solana/pools?limit=50&offset=100
```

**Response includes metadata:**
```json
{
  "data": [...],
  "meta": {
    "total": 1250,
    "limit": 50,
    "offset": 100,
    "hasMore": true
  }
}
```

---

## Webhooks and Real-time Data

This API provides REST endpoints for current data. For real-time updates, consider using WebSocket connections or polling at appropriate intervals based on your use case requirements.

---

## Support

For API support and questions:
- **Email:** team@orca.so
- **Website:** https://www.orca.so
- **Documentation:** Check the official Orca documentation for additional resources
