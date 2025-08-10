# Raydium API V3 Reference

## Overview
**Base URL:** `https://api-v3-devnet.raydium.io`  
**Version:** 3.0.0  
**Description:** All APIs have outermost status information wrapped around them.

**Response Format:**
- Success: `{ id: string, success: true, data: object }`
- Failure: `{ id: string, success: false, msg: string }`

---

## API Categories

### MAIN - Main API
Core functionality and system information endpoints.

### MINT - Mint Info
Token and mint-related information endpoints.

### POOLS - Pools Info And Keys
Pool information, keys, and liquidity data endpoints.

### FARMS - Farms Info And Keys
Farm pool information and key endpoints.

### IDO - IDO Pool Info
Initial DEX Offering pool information endpoints.

---

## Endpoints

### Farm Endpoints

#### GET `/farms/info/ids`
Get farm pool information by IDs.

**Parameters:**
- `ids` (required): String - Comma-separated farm IDs

**Example:**
```
GET /farms/info/ids?ids=farm1,farm2,farm3
```

---

#### GET `/farms/info/lp`
Search farm by LP mint address.

**Parameters:**
- `lp` (required): String - Stake LP token address
- `pageSize` (required): Number - Page size (max 100)
- `page` (required): Number - Page index

**Example:**
```
GET /farms/info/lp?lp=<LP_MINT>&pageSize=50&page=1
```

---

#### GET `/farms/key/ids`
Get farm pool keys by IDs.

**Parameters:**
- `ids` (required): String - Comma-separated farm IDs

---

### IDO Endpoints

#### GET `/ido/key/ids`
Get IDO pool keys by IDs.

**Parameters:**
- `ids` (required): String - Comma-separated IDO pool IDs

---

### Main Endpoints

#### GET `/main/version`
Get UI V3 current version.

**Description:** Returns the current version of the Raydium UI.

---

#### GET `/main/rpcs`
Get UI RPC endpoints.

**Description:** Returns available RPC endpoints for the UI.

---

#### GET `/main/chain-time`
Get chain time information.

**Description:** Returns chain time with offset calculation: `offset = local utc time - chain time`

---

#### GET `/main/info`
Get TVL and 24-hour volume statistics.

**Description:** Returns statistical information including Total Value Locked and 24-hour trading volume.

---

#### GET `/main/stake-pools`
Get RAY staking pools.

**Description:** Returns information about RAY staking pools.

---

#### GET `/main/migrate-lp`
Get migrate LP pool list.

**Description:** Returns advised transfer position pool information for LP migration.

---

#### GET `/main/auto-fee`
Get transaction auto fee estimation.

**Description:** Returns estimated on-chain costs based on recent transactions.

---

#### GET `/main/clmm-config`
Get CLMM (Concentrated Liquidity Market Maker) configuration.

**Description:** Returns configuration info allowed when the UI creates CLMM pools.

---

#### GET `/main/cpmm-config`
Get CPMM (Constant Product Market Maker) configuration.

**Description:** Returns configuration info allowed when the UI creates CPMM pools.

---

#### GET `/main/mint-filter-config`
Get mint filter configuration.

**Description:** Returns configuration info for mint filtering when creating pools.

---

### Mint Endpoints

#### GET `/mint/list`
Get default mint list.

**Description:** Returns the UI default mint token list.

---

#### GET `/mint/ids`
Get mint information by IDs.

**Parameters:**
- `ids` (required): String - Comma-separated mint addresses

**Example:**
```
GET /mint/ids?ids=So11111111111111111111111111111111111111112,EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v
```

---

#### GET `/mint/price`
Get mint prices.

**Parameters:**
- `mints` (optional): String - Comma-separated mint addresses

**Description:** Returns current prices for specified token mints.

**Example:**
```
GET /mint/price?mints=So11111111111111111111111111111111111111112,EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v
```

**Response Format:**
```json
{
  "id": "request-id",
  "success": true,
  "data": {
    "So11111111111111111111111111111111111111112": "160.50",
    "EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v": "1.00"
  }
}
```

---

### Pool Endpoints

#### GET `/pools/info/ids`
Get pool information by IDs.

**Parameters:**
- `ids` (required): String - Comma-separated pool IDs

---

#### GET `/pools/info/lps`
Get pool information by LP mint addresses.

**Parameters:**
- `lps` (required): String - Comma-separated LP mint addresses

---

#### GET `/pools/info/list`
Get pool information list with pagination.

**Parameters:**
- `page` (optional): Number - Page number
- `pageSize` (optional): Number - Page size (max 1000)
- `poolType` (optional): String - Pool type filter
- `poolSortField` (optional): String - Sort field
- `sortType` (optional): String - Sort direction

---

#### GET `/pools/info/mint`
Get pool information by token mint.

**Parameters:**
- `mint1` (required): String - First token mint address
- `mint2` (optional): String - Second token mint address
- `poolType` (optional): String - Pool type filter
- `poolSortField` (optional): String - Sort field
- `sortType` (optional): String - Sort direction
- `pageSize` (optional): Number - Page size (max 1000)
- `page` (optional): Number - Page number

---

#### GET `/pools/info/list-v2`
Get pool information list (version 2) with enhanced filtering.

**Parameters:**
- `page` (optional): Number - Page number
- `pageSize` (optional): Number - Page size (max 1000)
- `poolType` (optional): String - Pool type filter
- `poolSortField` (optional): String - Sort field
- `sortType` (optional): String - Sort direction
- `mint1` (optional): String - First token mint filter
- `mint2` (optional): String - Second token mint filter
- `tvl` (optional): Number - Minimum TVL filter
- `volume24h` (optional): Number - Minimum 24h volume filter

---

#### GET `/pools/key/ids`
Get pool keys by IDs.

**Parameters:**
- `ids` (required): String - Comma-separated pool IDs

---

#### GET `/pools/line/liquidity`
Get pool liquidity history.

**Parameters:**
- `id` (required): String - Pool ID

**Description:** Returns pool liquidity history (maximum 30 days).

---

#### GET `/pools/line/position`
Get CLMM position line data.

**Parameters:**
- `id` (required): String - Position ID

**Description:** Returns CLMM position line chart data.

---

## Data Models

### Mint Models

#### MintTagsItem
Enumeration of mint tags:
- `hasFreeze` - Token has freeze authority
- `hasTransferFee` - Token has transfer fee

#### MintExtensionsItem
Extended mint information:
- `coingeckoId` (optional): String - CoinGecko ID for price tracking
- `feeConfig` (optional): Object - Transfer fee configuration

#### ApiV3MintItem
Complete mint token information:
- `chainId`: Number - Blockchain chain ID
- `address`: String - Token mint address
- `programId`: String - Token program ID
- `logoURI`: String - Token logo URI
- `symbol`: String - Token symbol
- `name`: String - Token name
- `decimals`: Number - Token decimal places
- `tags`: Array of MintTagsItem - Token tags
- `extensions`: MintExtensionsItem - Extended information

---

## Usage Examples

### Get SOL and USDC Prices
```bash
curl "https://api-v3-devnet.raydium.io/mint/price?mints=So11111111111111111111111111111111111111112,EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v"
```

### Get Pool Information
```bash
curl "https://api-v3-devnet.raydium.io/pools/info/list?pageSize=10&page=1"
```

### Get Farm Information
```bash
curl "https://api-v3-devnet.raydium.io/farms/info/ids?ids=farm_id_1,farm_id_2"
```

---

## Error Handling

All API responses follow the standard format:

**Success Response:**
```json
{
  "id": "unique-request-id",
  "success": true,
  "data": {
    // Response data
  }
}
```

**Error Response:**
```json
{
  "id": "unique-request-id",
  "success": false,
  "msg": "Error description"
}
```

---

## Rate Limiting

Please respect rate limits when making API calls. The API may implement rate limiting to ensure fair usage across all users.

---

## Support

For technical support and API questions, please refer to the official Raydium documentation or community channels.
