# Meteora DLMM API Reference

## Overview
**Base URL:** `https://dlmm-api.meteora.ag`  
**Version:** 1.0.9  
**Description:** Keeper backend to index analytic information for liquidity book CLMM (Concentrated Liquidity Market Maker)

**Contact:**
- **Name:** tian
- **Email:** tian@racoons.dev

---

## API Categories

The Meteora DLMM API provides comprehensive endpoints for interacting with Dynamic Liquidity Market Maker pools, analytics, positions, and wallet information.

---

## Endpoints

### Protocol Information

#### GET `/info/protocol_metrics`
Get protocol-wide metrics and statistics.

**Operation ID:** `get_protocol_metrics`

**Response Schema:** `ProtocolMetrics`
- `total_tvl` (required): Number (double) - Total value locked across all pools
- `daily_trade_volume` (required): Number (double) - Daily trading volume
- `total_trade_volume` (required): Number (double) - Cumulative trading volume
- `daily_fee` (required): Number (double) - Daily trading fees collected
- `total_fee` (required): Number (double) - Total fees collected

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/info/protocol_metrics"
```

**Response:**
```json
{
  "total_tvl": 50000000.00,
  "daily_trade_volume": 5000000.00,
  "total_trade_volume": 1000000000.00,
  "daily_fee": 15000.00,
  "total_fee": 3000000.00
}
```

---

### Pair Endpoints

#### GET `/pair/all`
Retrieve all trading pairs available on the protocol.

**Operation ID:** `all`

**Query Parameters:**
- `include_unknown` (optional): Boolean - Include pools with unverified tokens (default: true)

**Returns:** Array of `PairInfo` objects

**Response:** Direct array of trading pair information (not wrapped in an object)

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/pair/all?include_unknown=false"
```

---

#### GET `/pair/all_by_groups`
Get all pairs organized by groups with advanced filtering options.

**Operation ID:** `all_by_groups`

**Query Parameters:**
- `page` (optional): Integer (minimum: 0) - Page number (default: 0)
- `limit` (optional): Integer (minimum: 0) - Items per page (default: 50)
- `skip_size` (optional): Integer (minimum: 0) - Items to skip (default: 0)
- `pools_to_top` (optional): Array of strings - Pools to sort to top
- `sort_key` (optional): PairSortKey enum - Sort key (default: Volume)
- `order_by` (optional): OrderBy enum - Sort order (default: Descending)
- `search_term` (optional): String - Search term
- `include_unknown` (optional): Boolean - Include unverified tokens (default: true)
- `hide_low_tvl` (optional): Number (double) - Hide pools with TVL below this value
- `hide_low_apr` (optional): Boolean - Hide pools with low APR
- `include_token_mints` (optional): Array of strings - Filter by token mints
- `include_pool_token_pairs` (optional): Array of strings - Filter by pool token pairs (format: mint1-mint2)
- `tags` (optional): Array of strings - Filter by tags
- `launchpad` (optional): Array of strings - Filter by launchpad

**Response Schema:** `AllGroupOfPairs`
- `groups` (required): Array of `PairGroup` objects
- `total` (required): Total count

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/pair/all_by_groups?page=0&limit=20&sort_key=Volume&order_by=Descending"
```

---

#### GET `/pair/all_by_groups_metadata`
Get metadata for all pair groups with advanced filtering.

**Operation ID:** `all_by_groups_metadata`

**Query Parameters:** (Same as `/pair/all_by_groups`)

**Response Schema:** `AllGroupOfPairsMetadata`
- `metadatas` (required): Array of `PairGroupMetadata` objects
- `total` (required): Total count

---

#### GET `/pair/all_with_pagination`
Get paginated list of all pairs with filtering capabilities.

**Operation ID:** `all_with_pagination`

**Query Parameters:** (Same as `/pair/all_by_groups`)

**Response Schema:** `AllPairsWithPagination`
- `pairs` (required): Array of `PairInfo` objects
- `total` (required): Total count

---

#### GET `/pair/group_pair/{lexical_order_mints}`
Get information for a single group pair.

**Operation ID:** `get_single_group_pair`

**Path Parameters:**
- `lexical_order_mints` (required): String - Lexically ordered token mints of the pair

**Response:** Array of `PairInfo` objects

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/pair/group_pair/EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v-So11111111111111111111111111111111111111112"
```

---

#### GET `/pair/{pair_address}`
Get detailed information for a specific trading pair.

**Operation ID:** `get_pair`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Response Schema:** `PairInfo`

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/pair/LBUZKhRxPF3XUpBCjp4YzTKgLccjZhTSDM9YuVaPwxo"
```

---

### Pair Analytics

#### GET `/pair/{pair_address}/analytic/bin_trade_volume` ⚠️ Deprecated
Get bin trade volume analytics for a pair by days.

**Operation ID:** `get_bin_trade_volume_by_days`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Query Parameters:**
- `num_of_days` (required): Integer (int32, minimum: 0) - Number of days before today (max: 255)

**Response:** Array of `BinTradeVolume` objects

---

#### GET `/pair/{pair_address}/analytic/pair_fee_bps`
Get pair fee basis points analytics by days.

**Operation ID:** `get_pair_fee_bps_by_days`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Query Parameters:**
- `num_of_days` (required): Integer (int32, minimum: 0) - Number of days before today (max: 255)

**Response:** Array of `PairFeeBps` objects

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/pair/LBUZKhRxPF3XUpBCjp4YzTKgLccjZhTSDM9YuVaPwxo/analytic/pair_fee_bps?num_of_days=7"
```

---

#### GET `/pair/{pair_address}/analytic/pair_trade_volume`
Get daily trade volume analytics for a pair.

**Operation ID:** `get_pair_daily_trade_volume_by_days`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Query Parameters:**
- `num_of_days` (required): Integer (int32, minimum: 0) - Number of days before today (max: 255)

**Response:** Array of `PairTradeVolume` objects

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/pair/LBUZKhRxPF3XUpBCjp4YzTKgLccjZhTSDM9YuVaPwxo/analytic/pair_trade_volume?num_of_days=30"
```

---

#### GET `/pair/{pair_address}/analytic/pair_tvl`
Get Total Value Locked (TVL) analytics for a pair by days.

**Operation ID:** `get_pair_tvl_by_days`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Query Parameters:**
- `num_of_days` (required): Integer (int32, minimum: 0) - Number of days before today (max: 255)

**Response:** Array of `PairTvlSnapshotByDay` objects

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/pair/LBUZKhRxPF3XUpBCjp4YzTKgLccjZhTSDM9YuVaPwxo/analytic/pair_tvl?num_of_days=14"
```

---

#### GET `/pair/{pair_address}/analytic/swap_history`
Get swap history records for a pair.

**Operation ID:** `get_pair_swap_records`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Query Parameters:**
- `rows_to_take` (required): Integer (int32, minimum: 0) - Number of records to take (max: 255)

**Response:** Array of `Swap` objects

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/pair/LBUZKhRxPF3XUpBCjp4YzTKgLccjZhTSDM9YuVaPwxo/analytic/swap_history?rows_to_take=100"
```

---

### Pair Data

#### GET `/pair/{pair_address}/bin_arrays` ⚠️ Deprecated
Get bin arrays for a specific pair.

**Operation ID:** `get_bin_arrays`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Response:** Array of `BinArrayCache` objects

---

#### GET `/pair/{pair_address}/positions_lock`
Get position locks for a specific pair.

**Operation ID:** `get_pair_positions_lock`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Response:** Array of `PositionLock` objects

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/pair/LBUZKhRxPF3XUpBCjp4YzTKgLccjZhTSDM9YuVaPwxo/positions_lock"
```

---

### Position Endpoints

#### GET `/position/{position_address}`
Get detailed information for a specific position.

**Operation ID:** `get_position`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response Schema:** `PositionWithApy`

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/position/3K6rftdAaQYMPunrtNRHgnK2UAtjm2JwyT2oCiTDouYE"
```

---

#### GET `/position/{position_address}/claim_fees`
Get claimable fees for a position.

**Operation ID:** `get_claim_fees`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response:** Array of `ClaimFee` objects

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/position/3K6rftdAaQYMPunrtNRHgnK2UAtjm2JwyT2oCiTDouYE/claim_fees"
```

---

#### GET `/position/{position_address}/claim_rewards`
Get claimable rewards for a position.

**Operation ID:** `get_claim_rewards`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response:** Array of `ClaimReward` objects

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/position/3K6rftdAaQYMPunrtNRHgnK2UAtjm2JwyT2oCiTDouYE/claim_rewards"
```

---

#### GET `/position/{position_address}/deposits`
Get deposit history for a position.

**Operation ID:** `get_deposits`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response:** Array of `DepositWithdraw` objects

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/position/3K6rftdAaQYMPunrtNRHgnK2UAtjm2JwyT2oCiTDouYE/deposits"
```

---

#### GET `/position/{position_address}/snapshots` ⚠️ Deprecated
Get recent snapshots for a position.

**Operation ID:** `get_recent_n_snapshot`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Query Parameters:**
- `rows_to_take` (required): Integer (int32, minimum: 0) - Number of records to take (max: 255)

**Response:** Array of `PositionSnapshot` objects

---

#### GET `/position/{position_address}/withdraws`
Get withdrawal history for a position.

**Operation ID:** `get_withdraws`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response:** Array of `DepositWithdraw` objects

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/position/3K6rftdAaQYMPunrtNRHgnK2UAtjm2JwyT2oCiTDouYE/withdraws"
```

---

#### GET `/position_v2/{position_address}`
Get enhanced position information (version 2).

**Operation ID:** `get_position_v2`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response Schema:** `Position`

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/position_v2/3K6rftdAaQYMPunrtNRHgnK2UAtjm2JwyT2oCiTDouYE"
```

---

### Wallet Endpoints

#### GET `/wallet/{wallet_address}/{pair_address}/earning`
Get earning information for a wallet on a specific pair.

**Operation ID:** `get_wallet_earning`

**Path Parameters:**
- `wallet_address` (required): String - Address of the wallet
- `pair_address` (required): String - Address of the pair

**Response:** Array of `WalletEarning` objects

**Example:**
```bash
curl "https://dlmm-api.meteora.ag/wallet/9WzDXwBbmkg8ZTbNMqUxvQRAyrZzDsGYdLVL9zYtAWWM/LBUZKhRxPF3XUpBCjp4YzTKgLccjZhTSDM9YuVaPwxo/earning"
```

---

## Data Models

### Core Types

#### PairInfo
Main trading pair information object with comprehensive pair data.

**Required Fields:**
- `address`: String - Address of the liquidity pair
- `name`: String - Name of the liquidity pair
- `mint_x`: String - Address of token X mint
- `mint_y`: String - Address of token Y mint
- `reserve_x`: String - Address of token X reserve
- `reserve_y`: String - Address of token Y reserve
- `reserve_x_amount`: Integer (int64, minimum: 0) - Token X amount held by the pair
- `reserve_y_amount`: Integer (int64, minimum: 0) - Token Y amount held by the pair
- `bin_step`: Integer (int32, minimum: 0) - Bin step for price increments
- `base_fee_percentage`: String - Base fee rate
- `max_fee_percentage`: String - Maximum fee rate
- `protocol_fee_percentage`: String - Protocol fee rate (cut from trade fee)
- `liquidity`: String - Total liquidity (Total Value Locked)
- `reward_mint_x`: String - Address of farming reward X token
- `reward_mint_y`: String - Address of farming reward Y token
- `fees_24h`: Number (double) - Trading fees earned in last 24 hours
- `today_fees`: Number (double) - Trading fees earned since beginning of day
- `trade_volume_24h`: Number (double) - Trading volume in last 24 hours
- `cumulative_trade_volume`: String - Cumulative trading volume
- `cumulative_fee_volume`: String - Cumulative fee volume
- `current_price`: Number (double) - Current price of the liquidity pair
- `apr`: Number (double) - 24-hour APR
- `apy`: Number (double) - 24-hour APY
- `farm_apr`: Number (double) - Farm reward APR
- `farm_apy`: Number (double) - Farm reward APY
- `hide`: Boolean - Whether pair should be hidden in UI
- `is_blacklisted`: Boolean - Whether pair is blacklisted
- `fees`: FeeData object - Fee data in different timeframes
- `fee_tvl_ratio`: FeeTvlRatioData object - Fee TVL ratio percentages
- `volume`: VolumeData object - Volume data in different timeframes
- `tags`: Array of strings - Tags for categorization
- `is_verified`: Boolean - Whether pool is verified

**Optional Fields:**
- `launchpad`: String (nullable) - Associated launchpad

#### BinTradeVolume
Trading volume data for individual bins in the liquidity book.

**Required Fields:**
- `bin_id`: String - Unique identifier of the bin
- `total_amount_x`: String - Total amount of token X traded (e.g., "0")
- `total_amount_y`: String - Total amount of token Y traded (e.g., "0")
- `total_amount_usd`: Number (double) - Total USD value of trades

#### PairFeeBps
Fee information expressed in basis points.

**Required Fields:**
- `pair_address`: String - Address of the trading pair
- `hour_date`: String - Timestamp for the data point
- `fee_bps`: Number - Fee in basis points (1 basis point = 0.01%)

#### PairTradeVolume
Daily trading volume information for pairs.

**Required Fields:**
- `day_date`: String - Date of the trading data
- `volume_usd`: Number (double) - Trading volume in USD
- `volume_x`: String - Volume of token X
- `volume_y`: String - Volume of token Y

#### PairTvlSnapshotByDay
Daily Total Value Locked snapshots.

**Required Fields:**
- `day_date`: String - Date of the snapshot
- `tvl_usd`: Number (double) - TVL in USD
- `reserve_x`: String - Reserve amount of token X
- `reserve_y`: String - Reserve amount of token Y

#### Position
Enhanced position information with APY calculations.

**Required Fields:**
- `created_at`: String - Position creation timestamp
- `address`: String - Position address
- `owner`: String - Position owner address
- `pair_address`: String - Associated pair address
- `lower_bin_id`: Integer - Lower bound bin ID
- `upper_bin_id`: Integer - Upper bound bin ID
- `liquidity_x`: String - Liquidity of token X
- `liquidity_y`: String - Liquidity of token Y
- `fee_x`: String - Accumulated fees in token X
- `fee_y`: String - Accumulated fees in token Y

#### Swap
Individual swap transaction information.

**Required Fields:**
- `out_token`: String - Output token mint address
- `in_token`: String - Input token mint address
- `out_amount`: String - Amount of output token
- `in_amount`: String - Amount of input token
- `price`: Number (double) - Swap price
- `tx_hash`: String - Transaction hash
- `timestamp`: Number - Unix timestamp
- `user`: String - User wallet address

#### ClaimFee
Claimable fee information for positions.

**Required Fields:**
- `onchain_timestamp`: Number - On-chain timestamp
- `fee_x`: String - Claimable fee in token X
- `fee_y`: String - Claimable fee in token Y
- `fee_x_usd`: Number (double) - Fee X value in USD
- `fee_y_usd`: Number (double) - Fee Y value in USD

#### ClaimReward
Claimable reward information for positions.

**Required Fields:**
- `onchain_timestamp`: Number - On-chain timestamp
- `reward_mint`: String - Reward token mint address
- `reward_amount`: String - Claimable reward amount
- `reward_amount_usd`: Number (double) - Reward value in USD

#### WalletEarning
Earning information for a wallet on a specific pair.

**Properties:**
- `total_fees_earned`: Number (double) - Total fees earned
- `total_rewards_earned`: Number (double) - Total rewards earned
- `positions`: Array - Associated positions
- `current_value`: Number (double) - Current position value

---

## Enums and Constants

### PairSortKey
Available sorting options for pairs:
- `Volume` - Sort by trading volume
- `TVL` - Sort by Total Value Locked
- `Fees` - Sort by fee generation
- `APR` - Sort by Annual Percentage Rate
- `APY` - Sort by Annual Percentage Yield

### OrderBy
Sort order directions:
- `Ascending` - Low to high
- `Descending` - High to low

---

## Advanced Features

### Fee Data Structure
The `FeeData` object provides fee information across different timeframes:
- `hour_24`: 24-hour fee data
- `hour_12`: 12-hour fee data
- `hour_6`: 6-hour fee data
- `hour_1`: 1-hour fee data

### Volume Data Structure
The `VolumeData` object provides volume information across different timeframes:
- `hour_24`: 24-hour volume data
- `hour_12`: 12-hour volume data
- `hour_6`: 6-hour volume data
- `hour_1`: 1-hour volume data

### Fee-to-TVL Ratio
The `FeeTvlRatioData` object shows the efficiency of fee generation relative to TVL:
- Higher ratios indicate more efficient fee generation
- Useful for comparing pool performance

---

## Usage Examples

### Get All Pairs with High TVL
```bash
curl "https://dlmm-api.meteora.ag/pair/all_with_pagination?sort_key=TVL&order_by=Descending&limit=20"
```

### Get Specific Pair Information
```bash
curl "https://dlmm-api.meteora.ag/pair/LBUZKhRxPF3XUpBCjp4YzTKgLccjZhTSDM9YuVaPwxo"
```

### Get Protocol-wide Metrics
```bash
curl "https://dlmm-api.meteora.ag/info/protocol_metrics"
```

### Get Position with Claimable Fees
```bash
curl "https://dlmm-api.meteora.ag/position/3K6rftdAaQYMPunrtNRHgnK2UAtjm2JwyT2oCiTDouYE/claim_fees"
```

### Get Trading Volume History
```bash
curl "https://dlmm-api.meteora.ag/pair/LBUZKhRxPF3XUpBCjp4YzTKgLccjZhTSDM9YuVaPwxo/analytic/pair_trade_volume?num_of_days=7"
```

### Search for SOL-USDC Pairs
```bash
curl "https://dlmm-api.meteora.ag/pair/group_pair/EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v-So11111111111111111111111111111111111111112"
```

---

## Error Handling

The API returns standard HTTP status codes:

- `200` - Success
- `400` - Bad Request (invalid parameters)
- `404` - Not Found (resource doesn't exist)
- `429` - Too Many Requests (rate limited)
- `500` - Internal Server Error

**Error Response Format:**
```json
{
  "error": "Invalid parameter",
  "message": "num_of_days must be between 0 and 255",
  "code": "INVALID_PARAMETER"
}
```

---

## Rate Limiting

The API implements rate limiting to ensure service stability. Please:
- Implement appropriate retry logic with exponential backoff
- Cache responses when possible
- Respect the rate limits indicated in response headers

---

## Pagination

List endpoints support pagination with these parameters:
- `page` - Page number (0-based)
- `limit` - Items per page
- `skip_size` - Items to skip

**Example Response with Pagination:**
```json
{
  "pairs": [...],
  "total": 1500,
  "page": 0,
  "limit": 50,
  "hasMore": true
}
```

---

## Best Practices

1. **Caching:** Cache responses for data that doesn't change frequently
2. **Batch Requests:** Use pagination efficiently to get required data
3. **Error Handling:** Implement robust error handling for network issues
4. **Rate Limiting:** Respect API limits and implement backoff strategies
5. **Data Validation:** Validate response data before processing
6. **Monitoring:** Monitor API health and response times

---

## Support

For technical support and API questions:
- **Email:** tian@racoons.dev
- **Documentation:** Check the official Meteora documentation
- **Community:** Join the Meteora Discord for community support

---

## Changelog

### Version 1.0.9
- Enhanced position tracking with V2 endpoints
- Improved analytics with more timeframe options
- Added wallet earning calculations
- Deprecated legacy bin array endpoints
