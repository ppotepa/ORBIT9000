# Meteora DLMM API Reference

## Overview
**Base URL:** `https://dlmm-api.meteora.ag`  
**Version:** 1.0.9  
**Description:** Keeper backend to index analytic information for liquidity book CLMM

**Contact:**
- **Name:** tian
- **Email:** tian@racoons.dev

---

## Endpoints

### Protocol Information

#### GET `/info/protocol_metrics`
Get protocol-wide metrics and statistics.

**Operation ID:** `get_protocol_metrics`

**Response Schema:** `ProtocolMetrics`
- `total_tvl` (required): Number (double) - Total value locked
- `daily_trade_volume` (required): Number (double) - Daily trading volume
- `total_trade_volume` (required): Number (double) - Total trading volume
- `daily_fee` (required): Number (double) - Daily trading fee
- `total_fee` (required): Number (double) - Total trading fee

---

### Pair Endpoints

#### GET `/pair/all`
Retrieve all trading pairs available on the protocol.

**Operation ID:** `all`

**Query Parameters:**
- `include_unknown` (optional): Boolean - Include pool with unverified token. Default true.

**Returns:** Array of `PairInfo` objects

**Response:** Direct array of trading pair information (not wrapped in an object)

---

#### GET `/pair/all_by_groups`
Get all pairs organized by groups with filtering options.

**Operation ID:** `all_by_groups`

**Query Parameters:**
- `page` (optional): Integer (minimum: 0) - Page number. Default is 0
- `limit` (optional): Integer (minimum: 0) - Number of items per page. Default is 50
- `skip_size` (optional): Integer (minimum: 0) - Number of items to skip. Default is 0
- `pools_to_top` (optional): Array of strings - Pools to be sorted to top
- `sort_key` (optional): PairSortKey enum - Sort key. Default is Volume
- `order_by` (optional): OrderBy enum - Sort order. Default is Descending
- `search_term` (optional): String - Search term
- `include_unknown` (optional): Boolean - Include pool with unverified token. Default true
- `hide_low_tvl` (optional): Number (double) - Toggle pools with lower TVL than the value passed in
- `hide_low_apr` (optional): Boolean - Toggle pools with low APR
- `include_token_mints` (optional): Array of strings - Only include token mints. Allow list of token mints
- `include_pool_token_pairs` (optional): Array of strings - Only include pool token pairs. Allow list of pool token mints in format EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v-So11111111111111111111111111111111111111112
- `tags` (optional): Array of strings - Tags to filter by
- `launchpad` (optional): Array of strings - Launchpad to filter by

**Response Schema:** `AllGroupOfPairs`
- `groups` (required): Array of `PairGroup` objects
- `total` (required): Total count

---

#### GET `/pair/all_by_groups_metadata`
Get metadata for all pair groups with advanced filtering.

**Operation ID:** `all_by_groups_metadata`

**Query Parameters:**
- `page` (optional): Integer (minimum: 0) - Page number. Default is 0
- `limit` (optional): Integer (minimum: 0) - Number of items per page. Default is 50
- `skip_size` (optional): Integer (minimum: 0) - Number of items to skip. Default is 0
- `pools_to_top` (optional): Array of strings - Pools to be sorted to top
- `sort_key` (optional): PairSortKey enum - Sort key. Default is Volume
- `order_by` (optional): OrderBy enum - Sort order. Default is Descending
- `search_term` (optional): String - Search term
- `include_unknown` (optional): Boolean - Include pool with unverified token. Default true
- `hide_low_tvl` (optional): Number (double) - Toggle pools with lower TVL than the value passed in
- `hide_low_apr` (optional): Boolean - Toggle pools with low APR
- `include_token_mints` (optional): Array of strings - Only include token mints. Allow list of token mints
- `include_pool_token_pairs` (optional): Array of strings - Only include pool token pairs. Allow list of pool token mints in format EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v-So11111111111111111111111111111111111111112
- `tags` (optional): Array of strings - Tags to filter by
- `launchpad` (optional): Array of strings - Launchpad to filter by

**Response Schema:** `AllGroupOfPairsMetadata`
- `metadatas` (required): Array of `PairGroupMetadata` objects
- `total` (required): Total count

---

#### GET `/pair/all_with_pagination`
Get paginated list of all pairs with filtering capabilities.

**Operation ID:** `all_with_pagination`

**Query Parameters:**
- `page` (optional): Integer (minimum: 0) - Page number. Default is 0
- `limit` (optional): Integer (minimum: 0) - Number of items per page. Default is 50
- `skip_size` (optional): Integer (minimum: 0) - Number of items to skip. Default is 0
- `pools_to_top` (optional): Array of strings - Pools to be sorted to top
- `sort_key` (optional): PairSortKey enum - Sort key. Default is Volume
- `order_by` (optional): OrderBy enum - Sort order. Default is Descending
- `search_term` (optional): String - Search term
- `include_unknown` (optional): Boolean - Include pool with unverified token. Default true
- `hide_low_tvl` (optional): Number (double) - Toggle pools with lower TVL than the value passed in
- `hide_low_apr` (optional): Boolean - Toggle pools with low APR
- `include_token_mints` (optional): Array of strings - Only include token mints. Allow list of token mints
- `include_pool_token_pairs` (optional): Array of strings - Only include pool token pairs. Allow list of pool token mints in format EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v-So11111111111111111111111111111111111111112
- `tags` (optional): Array of strings - Tags to filter by
- `launchpad` (optional): Array of strings - Launchpad to filter by

**Response Schema:** `AllPairsWithPagination`
- `pairs` (required): Array of `PairInfo` objects
- `total` (required): Total count

---

#### GET `/pair/group_pair/{lexical_order_mints}`
Get information for a single group pair.

**Operation ID:** `get_single_group_pair`

**Path Parameters:**
- `lexical_order_mints` (required): String - Lexical ordered token mints of the pair

**Response:** Array of `PairInfo` objects

---

#### GET `/pair/{pair_address}`
Get detailed information for a specific trading pair.

**Operation ID:** `get_pair`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Response Schema:** `PairInfo`

---

### Pair Analytics

#### GET `/pair/{pair_address}/analytic/bin_trade_volume` ⚠️ Deprecated
Get bin trade volume analytics for a pair by days.

**Operation ID:** `get_bin_trade_volume_by_days`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Query Parameters:**
- `num_of_days` (required): Integer (int32, minimum: 0) - Number of days before today. Max 255

**Response:** Array of `BinTradeVolume` objects

---

#### GET `/pair/{pair_address}/analytic/pair_fee_bps`
Get pair fee basis points analytics by days.

**Operation ID:** `get_pair_fee_bps_by_days`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Query Parameters:**
- `num_of_days` (required): Integer (int32, minimum: 0) - Number of days before today. Max 255

**Response:** Array of `PairFeeBps` objects

---

#### GET `/pair/{pair_address}/analytic/pair_trade_volume`
Get daily trade volume analytics for a pair.

**Operation ID:** `get_pair_daily_trade_volume_by_days`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Query Parameters:**
- `num_of_days` (required): Integer (int32, minimum: 0) - Number of days before today. Max 255

**Response:** Array of `PairTradeVolume` objects

---

#### GET `/pair/{pair_address}/analytic/pair_tvl`
Get Total Value Locked (TVL) analytics for a pair by days.

**Operation ID:** `get_pair_tvl_by_days`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Query Parameters:**
- `num_of_days` (required): Integer (int32, minimum: 0) - Number of days before today. Max 255

**Response:** Array of `PairTvlSnapshotByDay` objects

---

#### GET `/pair/{pair_address}/analytic/swap_history`
Get swap history records for a pair.

**Operation ID:** `get_pair_swap_records`

**Path Parameters:**
- `pair_address` (required): String - Address of the liquidity pair

**Query Parameters:**
- `rows_to_take` (required): Integer (int32, minimum: 0) - Number of records to take. Max 255

**Response:** Array of `Swap` objects

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

---

### Position Endpoints

#### GET `/position/{position_address}`
Get detailed information for a specific position.

**Operation ID:** `get_position`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response Schema:** `PositionWithApy`

---

#### GET `/position/{position_address}/claim_fees`
Get claimable fees for a position.

**Operation ID:** `get_claim_fees`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response:** Array of `ClaimFee` objects

---

#### GET `/position/{position_address}/claim_rewards`
Get claimable rewards for a position.

**Operation ID:** `get_claim_rewards`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response:** Array of `ClaimReward` objects

---

#### GET `/position/{position_address}/deposits`
Get deposit history for a position.

**Operation ID:** `get_deposits`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response:** Array of `DepositWithdraw` objects

---

#### GET `/position/{position_address}/snapshots` ⚠️ Deprecated
Get recent snapshots for a position.

**Operation ID:** `get_recent_n_snapshot`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Query Parameters:**
- `rows_to_take` (required): Integer (int32, minimum: 0) - Number of records to take. Max 255

**Response:** Array of `PositionSnapshot` objects

---

#### GET `/position/{position_address}/withdraws`
Get withdrawal history for a position.

**Operation ID:** `get_withdraws`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response:** Array of `DepositWithdraw` objects

---

#### GET `/position_v2/{position_address}`
Get enhanced position information (version 2).

**Operation ID:** `get_position_v2`

**Path Parameters:**
- `position_address` (required): String - Address of the position

**Response Schema:** `Position`

---

### Wallet Endpoints

#### GET `/wallet/{wallet_address}/{pair_address}/earning`
Get earning information for a wallet on a specific pair.

**Operation ID:** `get_wallet_earning`

**Path Parameters:**
- `wallet_address` (required): String - Address of the wallet
- `pair_address` (required): String - Address of the pair

**Response:** Array of `WalletEarning` objects

---

## Data Models

### Core Types

#### PairInfo
Main trading pair information object with comprehensive pair data.

**Required Fields:**
- `address`: String - Address of the liquidity pair
- `name`: String - Name of the liquidity pair
- `mint_x`: String - Address of token X mint of the liquidity pair
- `mint_y`: String - Address of token Y mint of the liquidity pair
- `reserve_x`: String - Address of token X reserve of the liquidity pair
- `reserve_y`: String - Address of token Y reserve of the liquidity pair
- `reserve_x_amount`: Integer (int64, minimum: 0) - Token X amount the liquidity pair holds
- `reserve_y_amount`: Integer (int64, minimum: 0) - Token Y amount the liquidity pair holds
- `bin_step`: Integer (int32, minimum: 0) - Bin step
- `base_fee_percentage`: String - Base fee rate
- `max_fee_percentage`: String - Maximum fee rate
- `protocol_fee_percentage`: String - Protocol fee rate (a cut from trade fee)
- `liquidity`: String - Total liquidity the liquidity pair holding (Total Value Locked)
- `reward_mint_x`: String - Address of the farming reward X of the liquidity pair
- `reward_mint_y`: String - Address of the farming reward Y of the liquidity pair
- `fees_24h`: Number (double) - Trading fees earned in the last 24 hours
- `today_fees`: Number (double) - Trading fees earned since the beginning of the day
- `trade_volume_24h`: Number (double) - Trading volume in the last 24 hours
- `cumulative_trade_volume`: String - Cumulative trading volume
- `cumulative_fee_volume`: String - Cumulative fee volume
- `current_price`: Number (double) - Price of the liquidity pair
- `apr`: Number (double) - 24 hour APR
- `apy`: Number (double) - 24 hour APY
- `farm_apr`: Number (double) - Farm reward APR
- `farm_apy`: Number (double) - Farm reward APY
- `hide`: Boolean - Flag to indicate whether the pair should be shown in the UI
- `is_blacklisted`: Boolean - Flag to indicate whether the pair is blacklisted
- `fees`: FeeData object - Fee data in different timeframes
- `fee_tvl_ratio`: FeeTvlRatioData object - Fee TVL ratio in percentage in different timeframes
- `volume`: VolumeData object - Volume data in different timeframes
- `tags`: Array of strings - Tags of the pair
- `is_verified`: Boolean - Whether pool is verified

**Optional Fields:**
- `launchpad`: String (nullable) - Launchpad of the pair

#### BinTradeVolume
Trading volume data for individual bins.

**Required Fields:**
- `bin_id`: String - ID of the bin
- `total_amount_x`: String - Total amount of token X (example: "0")
- `total_amount_y`: String - Total amount of token Y (example: "0")
- `total_amount_usd`: Number (double) - Total USD value swapped

#### PairFeeBps
Fee information in basis points.

**Required Fields:**
- `pair_address`: String - Address of the pair
- `min_fee_bps`: Number (double) - Minimum fee charged in BPS
- `max_fee_bps`: Number (double) - Maximum fee charged in BPS
- `average_fee_bps`: Number (double) - Average fee charged in BPS
- `hour_date`: String - Timestamp (example: "2023-10-17T15:00:00")

#### PairTradeVolume
Daily trade volume data for pairs.

**Required Fields:**
- `pair_address`: String - Address of the liquidity pair
- `trade_volume`: Number (double) - Total trading volume in USD
- `fee_volume`: Number (double) - Total fee in USD
- `protocol_fee_volume`: Number (double) - Total protocol fee in USD
- `day_date`: String - Date with only day information (example: "2023-10-17")

#### PairTvlSnapshotByDay
Total Value Locked (TVL) snapshot data by day.

**Required Fields:**
- `pair_address`: String - Address of the liquidity pair
- `total_value_locked`: Number (double) - Total value locked
- `day_date`: String - Date with only day information (example: "2023-10-17")

#### Position
Position information with ownership and claim history.

**Required Fields:**
- `address`: String - Address of the position
- `pair_address`: String - Address of the liquidity pair for the position
- `owner`: String - Address of the position owner
- `total_fee_x_claimed`: Integer (int64) - Total fee X has been claimed
- `total_fee_y_claimed`: Integer (int64) - Total fee Y has been claimed
- `total_reward_x_claimed`: Integer (int64) - Total reward X has been claimed
- `total_reward_y_claimed`: Integer (int64) - Total reward Y has been claimed
- `total_fee_usd_claimed`: Number (double) - Total fee has been claimed in USD
- `total_reward_usd_claimed`: Number (double) - Total reward has been claimed in USD
- `created_at`: String - Created at UTC timestamp (example: "2024-04-26T04:36:28.766258Z")

#### ClaimFee
Fee claiming transaction data.

**Required Fields:**
- `tx_id`: Transaction ID
- `position_address`: Position address
- `pair_address`: Pair address
- `token_x_amount`: Amount of token X
- `token_y_amount`: Amount of token Y
- `token_x_usd_amount`: USD amount of token X
- `token_y_usd_amount`: USD amount of token Y
- `onchain_timestamp`: Transaction timestamp

#### ClaimReward
Reward claiming transaction data.

**Required Fields:**
- `tx_id`: Transaction ID
- `position_address`: Position address
- `pair_address`: Pair address
- `reward_mint_address`: Reward token mint address
- `token_amount`: Amount of reward token
- `token_usd_amount`: USD amount of reward token
- `onchain_timestamp`: Transaction timestamp

#### DepositWithdraw
Deposit/withdrawal transaction data.

**Required Fields:**
- `tx_id`: Transaction ID
- `position_address`: Position address
- `pair_address`: Pair address
- `active_bin_id`: Active bin ID
- `token_x_amount`: Amount of token X
- `token_y_amount`: Amount of token Y
- `price`: Liquidity pair price
- `token_x_usd_amount`: USD amount of token X
- `token_y_usd_amount`: USD amount of token Y
- `onchain_timestamp`: Transaction timestamp

### Analytics Types

#### FeeData
Fee analytics across different time periods.

**Required Fields:**
- `min_30`: Number (double) - 30-minute fee data
- `hour_1`: Number (double) - 1-hour fee data
- `hour_2`: Number (double) - 2-hour fee data
- `hour_4`: Number (double) - 4-hour fee data
- `hour_12`: Number (double) - 12-hour fee data
- `hour_24`: Number (double) - 24-hour fee data

#### VolumeData
Trading volume analytics across time periods.

**Required Fields:**
- `min_30`: Number (double) - 30-minute volume data
- `hour_1`: Number (double) - 1-hour volume data
- `hour_2`: Number (double) - 2-hour volume data
- `hour_4`: Number (double) - 4-hour volume data
- `hour_12`: Number (double) - 12-hour volume data
- `hour_24`: Number (double) - 24-hour volume data

#### FeeTvlRatioData
Fee to TVL ratio analytics across time periods.

**Required Fields:**
- `min_30`: Number (double) - 30-minute ratio
- `hour_1`: Number (double) - 1-hour ratio
- `hour_2`: Number (double) - 2-hour ratio
- `hour_4`: Number (double) - 4-hour ratio
- `hour_12`: Number (double) - 12-hour ratio
- `hour_24`: Number (double) - 24-hour ratio

### Group Types

#### AllGroupOfPairs
Response structure for grouped pairs.

**Required Fields:**
- `groups`: Array of `PairGroup` objects
- `total`: Integer (minimum: 0) - Total count

#### AllGroupOfPairsMetadata
Response structure for grouped pairs metadata.

**Required Fields:**
- `metadatas`: Array of `PairGroupMetadata` objects
- `total`: Integer (minimum: 0) - Total count

#### AllPairsWithPagination
Response structure for paginated pairs.

**Required Fields:**
- `pairs`: Array of `PairInfo` objects
- `total`: Integer (minimum: 0) - Total count

#### PairGroup
Group of trading pairs.

**Required Fields:**
- `name`: String - Group name
- `pairs`: Array of pairs in the group

#### PairGroupMetadata
Metadata for pair groups with comprehensive metrics.

**Required Fields:**
- `name`: String - Group name
- `lexical_order_mints`: String - Lexical order of token mints
- `total_trade_volume`: Number (double) - Total trading volume
- `total_tvl`: Number (double) - Total value locked
- `min_fee_tvl_ratio`: Number (double) - Minimum fee to TVL ratio
- `max_fee_tvl_ratio`: Number (double) - Maximum fee to TVL ratio
- `min_lm_apr`: Number (double) - Minimum liquidity mining APR
- `max_lm_apr`: Number (double) - Maximum liquidity mining APR
- `custom_fee_tvl_ratio`: CustomGroupFeeTvlRatio object - Custom fee to TVL ratio data
- `custom_volume`: CustomGroupVolume object - Custom volume data

#### CustomGroupVolume
Custom volume metrics for groups.

**Required Fields:**
- `total_trade_volume_min_30`: Number (double) - 30-minute volume
- `total_trade_volume_hour_1`: Number (double) - 1-hour volume
- `total_trade_volume_hour_2`: Number (double) - 2-hour volume
- `total_trade_volume_hour_4`: Number (double) - 4-hour volume
- `total_trade_volume_hour_12`: Number (double) - 12-hour volume

#### CustomGroupFeeTvlRatio
Custom fee to TVL ratio metrics for groups.

**Required Fields:**
- `min_fee_tvl_ratio_min_30`: Number (double) - Min ratio for 30 minutes
- `max_fee_tvl_ratio_min_30`: Number (double) - Max ratio for 30 minutes
- `min_fee_tvl_ratio_hour_1`: Number (double) - Min ratio for 1 hour
- `max_fee_tvl_ratio_hour_1`: Number (double) - Max ratio for 1 hour
- `min_fee_tvl_ratio_hour_2`: Number (double) - Min ratio for 2 hours
- `max_fee_tvl_ratio_hour_2`: Number (double) - Max ratio for 2 hours
- `min_fee_tvl_ratio_hour_4`: Number (double) - Min ratio for 4 hours
- `max_fee_tvl_ratio_hour_4`: Number (double) - Max ratio for 4 hours
- `min_fee_tvl_ratio_hour_12`: Number (double) - Min ratio for 12 hours
- `max_fee_tvl_ratio_hour_12`: Number (double) - Max ratio for 12 hours

### Bin and Cache Types

#### BinArrayCache
Cached bin array data.

**Required Fields:**
- `address`: String - Address of the bin array
- `index`: Integer (int64) - Index of the bin array
- `bins`: Array of `BinCache` objects - Bins

#### BinCache
Individual bin cache data.

**Required Fields:**
- `amount_x`: Integer (int64, minimum: 0) - Amount of token X in the bin (protocol fees excluded)
- `amount_y`: Integer (int64, minimum: 0) - Amount of token Y in the bin (protocol fees excluded)
- `price`: Number (double, minimum: 0) - Price of the bin
- `liquidity_supply`: Integer (minimum: 0) - Liquidities of the bin (same as LP mint supply)
- `reward_per_token_stored`: Array of integers - Accumulated reward per unit of liquidity deposited
- `fee_amount_x_per_token_stored`: Integer (minimum: 0) - Swap fee amount of token X per liquidity deposited
- `fee_amount_y_per_token_stored`: Integer (minimum: 0) - Swap fee amount of token Y per liquidity deposited
- `amount_x_in`: Integer (minimum: 0) - Total token X swap into the bin (tracking purpose)
- `amount_y_in`: Integer (minimum: 0) - Total token Y swap into the bin (tracking purpose)

#### WalletEarning
Wallet earning information for a specific pair.

**Fields:**
- `total_fee_usd_claimed`: Number (double, nullable) - Total fee amount has been claimed by the wallet in USD
- `total_fee_x_claimed`: String - Total fee x has been claimed by the wallet (example: "0")
- `total_fee_y_claimed`: String - Total fee y has been claimed by the wallet (example: "0")
- `total_reward_usd_claimed`: Number (double, nullable) - Total reward amount has been claimed by the wallet in USD

#### Swap
Swap transaction record with detailed trade information.

**Required Fields:**
- `tx_id`: String - Transaction hash
- `in_amount`: Integer (int64) - Amount of token swapped in
- `in_amount_usd`: Number (double) - Amount of USD value swapped in
- `out_amount`: Integer (int64) - Amount of token swapped out
- `out_amount_usd`: Number (double) - Amount of USD value swapped out
- `trade_fee`: Integer (int64) - Amount of fee charged
- `trade_fee_usd`: Number (double) - Amount of fee charged in USD
- `protocol_fee`: Integer (int64) - Amount of protocol fee charged
- `protocol_fee_usd`: Number (double) - Amount of protocol fee charged in USD
- `onchain_timestamp`: Integer (int64) - Timestamp of the swap activity
- `pair_address`: String - Address of the liquidity pair
- `start_bin_id`: Integer (int64) - Starting bin ID for the swap
- `end_bin_id`: Integer (int64) - Ending bin ID for the swap
- `bin_count`: Integer (int64) - Number of bin involved
- `fee_bps`: Number (double) - Fee in BPS
- `in_token`: String - Address of the token swapped in
- `out_token`: String - Address of the token swapped out

#### PositionLock
Position lock information.

**Required Fields:**
- `closed`: Boolean - Whether the position lock is closed

#### PositionSnapshot
Position snapshot data.

**Required Fields:**
- `created_at`: String - Created at UTC timestamp

#### PositionWithApy
Enhanced position information with APY calculations.

**Required Fields:**
- `daily_fee_yield`: Number - Daily fee yield percentage

## Enums

#### OrderBy
Sort order options.

**Values:**
- `asc`: Ascending order
- `desc`: Descending order

#### PairSortKey
Keys available for sorting pairs.

**Values:**
- `tvl`: Total Value Locked
- `volume`: Trading volume
- `feetvlratio`: Fee to TVL ratio
- `lm`: Liquidity mining
- `feetvlratio30m`: Fee to TVL ratio (30 minutes)
- `feetvlratio1h`: Fee to TVL ratio (1 hour)
- `feetvlratio2h`: Fee to TVL ratio (2 hours)
- `feetvlratio4h`: Fee to TVL ratio (4 hours)
- `feetvlratio12h`: Fee to TVL ratio (12 hours)
- `volume30m`: Volume (30 minutes)
- `volume1h`: Volume (1 hour)
- `volume2h`: Volume (2 hours)
- `volume4h`: Volume (4 hours)
- `volume12h`: Volume (12 hours)

---

## Notes

1. **Deprecated Endpoints:** Some endpoints are marked as deprecated and may be removed in future versions.
2. **Pagination:** Several endpoints support pagination parameters for efficient data retrieval.
3. **Time Periods:** Analytics endpoints often support multiple time period options (30min, 1h, 2h, 4h, 12h, 24h).
4. **Response Format:** The `/pair/all` endpoint returns a direct array, not an object wrapper.
5. **Numeric Minimums:** Many numeric fields have minimum value constraints (typically 0).
6. **Parameter Requirements:** Path parameters are always required, query parameters are typically optional with sensible defaults.
7. **Data Types:** All numeric values specify their format (int32, int64, double) and constraints.
8. **Array Responses:** Most analytics and historical endpoints return arrays of objects rather than single objects.

---

*Generated from OpenAPI specification version 3.1.0 with complete request/response model specifications*
