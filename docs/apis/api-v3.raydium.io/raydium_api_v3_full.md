# Raydium API V3 Documentation

**Title:** Raydium Api V3
**Version:** 3.0.0
**Description:**

All apis have outermost status information wrapped around them. 
 success { id: string, success: true, data: object } 
 failure { id: string, success: false, msg: string }


## Tags

- **MAIN**: Main API
- **MINT**: Mint Info
- **POOLS**: Pools Info And Keys
- **FARMS**: Farms Info And Keys
- **IDO**: IDO Pool Info

## Endpoints


### `GET /farms/info/ids`
- **Summary:** Farm Pool Info
- **Description:** get farm pool info
- **Tags:** FARMS
- **Parameters:**
  - `ids` (query, `string`): id.join(',')
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`

### `GET /farms/info/lp`
- **Summary:** Search Farm By Lp Mint
- **Description:** get farm by lp mint
- **Tags:** FARMS
- **Parameters:**
  - `lp` (query, `string`): stake lp
  - `pageSize` (query, `number`): page size, max 100
  - `page` (query, `number`): page index
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`

### `GET /farms/key/ids`
- **Summary:** Farm Pool Key
- **Description:** get farm pool key
- **Tags:** FARMS
- **Parameters:**
  - `ids` (query, `string`): id.join(',')
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`

### `GET /ido/key/ids`
- **Summary:** Ido Pool Keys
- **Description:** get ido pool keys
- **Tags:** IDO
- **Parameters:**
  - `ids` (query, `string`): id.join(',')
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`
      - `programId`: `string` - The program ID.
      - `id`: `string` - The ID.
      - `authority`: `string` - The authority.
      - `projectInfo`: `object` - Information about the project.
        - Type: `object`
        - `mint`: `object` - Information about the mint.
          - Type: `object`
        - `vault`: `string` - The vault.
      - `buyInfo`: `object` - Information about the buy.
        - Type: `object`
        - `mint`: `object` - Information about the mint.
          - Type: `object`
        - `vault`: `string` - The vault.

### `GET /main/version`
- **Summary:** UI V3 current version
- **Description:** UI version
- **Tags:** MAIN
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`
      - `latest`: `string` - UI current version
      - `least`: `string` - Minimum UI version number that users can use

### `GET /main/rpcs`
- **Summary:** UI RPCS
- **Description:** UI rpcs
- **Tags:** MAIN
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`

### `GET /main/chain-time`
- **Summary:** Chain Time
- **Description:** offset = local utc time - chain time
- **Tags:** MAIN
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`
      - `offset`: `integer` - local utc time - chain time

### `GET /main/info`
- **Summary:** TVL and 24 hour volume
- **Description:** statistical information
- **Tags:** MAIN
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`
      - `tvl`: `integer` - all pool tvl
      - `volume24`: `integer` - 24 hour volume

### `GET /main/stake-pools`
- **Summary:** RAY Stake
- **Description:** get Ray stake
- **Tags:** MAIN
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`

### `GET /main/migrate-lp`
- **Summary:** Migrate Lp Pool List
- **Description:** advised to transfer position pool information
- **Tags:** MAIN
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`
        - `name`: `string` - pool symbol
        - `ammId`: `string` - pool id
        - `lpMint`: `string` - amm pool lp mint
        - `farmIds`: `array` - 
          - Items: `string`
        - `clmmId`: `string` - pool id
        - `defaultPriceMin`: `number` - defaultPriceMin
        - `defaultPriceMax`: `number` - defaultPriceMax

### `GET /main/auto-fee`
- **Summary:** transaction auto fee
- **Description:** Estimated on-chain costs based on recent transactions
- **Tags:** MAIN
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`
      - `default`: `object` - 
        - Type: `object`
        - `m`: `number` - lower fee
        - `h`: `number` - moderate fee
        - `vh`: `number` - higher fee

### `GET /main/clmm-config`
- **Summary:** Clmm Config
- **Description:** The list contains only the config info allowed when the UI creates the pool
- **Tags:** MAIN
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`
        - `id`: `string` - config id
        - `index`: `number` - config id pda index
        - `protocolFeeRate`: `number` - protocol fee rate
        - `tradeFeeRate`: `number` - trade fee rate
        - `tickSpacing`: `number` - tick spacing
        - `fundFeeRate`: `number` - fund fee rate
        - `defaultRange`: `number` - default range
        - `defaultRangePoint`: `array` - 
          - Items: `number`

### `GET /main/cpmm-config`
- **Summary:** Cpmm Config
- **Description:** The list contains only the config info allowed when the UI creates the pool
- **Tags:** MAIN
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`
        - `id`: `string` - config id
        - `index`: `number` - config id pda index
        - `protocolFeeRate`: `number` - protocol fee rate
        - `tradeFeeRate`: `number` - trade fee rate
        - `fundFeeRate`: `number` - fund fee rate
        - `createPoolFee`: `string` - create pool fee

### `GET /main/mint-filter-config`
- **Summary:** Mint Filter Config
- **Description:** The list contains only the config info allowed when the UI creates the pool
- **Tags:** MAIN
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`
      - `row`: `array` - List of icon statistics
        - Items: `object`
          - Type: `object`
          - `name`: `string` - The name of the item
          - `icon`: `array` - A list of icon URLs or identifiers
            - Items: `string`
          - `count`: `integer` - The count associated with the item

### `GET /mint/list`
- **Summary:** Default Mint List
- **Description:** UI default mint list
- **Tags:** MINT
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`
      - `blockList`: `array` - 
        - Items: `string`
      - `mintList`: `array` - 
        - Items: `object`

### `GET /mint/ids`
- **Summary:** Mint Info
- **Description:** get mint info
- **Tags:** MINT
- **Parameters:**
  - `mints` (query, `string`): id.join(',')
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`

### `GET /mint/price`
- **Summary:** Mint Price
- **Description:** get mint price
- **Tags:** MINT
- **Parameters:**
  - `mints` (query, `string`): id.join(',')
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`
      - `[ mint id ]`: `string` - mint price.

### `GET /pools/info/ids`
- **Summary:** Pool Info
- **Description:** get pool info
- **Tags:** POOLS
- **Parameters:**
  - `ids` (query, `string`): id.join(',')
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`

### `GET /pools/info/lps`
- **Summary:** Pool Info By Lp Mint
- **Description:** get pool info by lp mint
- **Tags:** POOLS
- **Parameters:**
  - `lps` (query, `string`): lp.join(',')
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`

### `GET /pools/info/list`
- **Summary:** Pool Info List
- **Description:** get pool info list
- **Tags:** POOLS
- **Parameters:**
  - `poolType` (query, `string`): pool type
  - `poolSortField` (query, `string`): pool field
  - `sortType` (query, `string`): sort type
  - `pageSize` (query, `number`): page size, max 1000
  - `page` (query, `number`): page index
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`

### `GET /pools/info/mint`
- **Summary:** Pool Info By Token Mint
- **Description:** get pool info by token mint
- **Tags:** POOLS
- **Parameters:**
  - `mint1` (query, `string`): search mint
  - `mint2` (query, `string`): search mint
  - `poolType` (query, `string`): pool type
  - `poolSortField` (query, `string`): pool field
  - `sortType` (query, `string`): sort type
  - `pageSize` (query, `number`): page size, max 1000
  - `page` (query, `number`): page index
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`

### `GET /pools/info/list-v2`
- **Summary:** Pool Info List V2
- **Description:** get pool info list
- **Tags:** POOLS
- **Parameters:**
  - `poolType` (query, `string`): Type of pool. Can be `Concentrated` or `Standard`. Optional — if not set, all types are returned.
  - `mintFilter` (query, `string`): Whether to filter by mint. Optional. please refer /main/mint-filter-config
  - `hasReward` (query, `boolean`): Whether to return only pools that have rewards. Optional.
  - `sortField` (query, `string`): Field to sort by. Optional.
  - `sortType` (query, `string`): Sorting order, either ascending (`asc`) or descending (`desc`). Optional.
  - `size` (query, `integer`): Number of records to return per page.
  - `nextPageId` (query, `string`): The `id` returned from the previous page, used for pagination. Optional — if not provided, the first page is returned.
  - `mint1` (query, `string`): Filter for pools that include this mint. Optional.
  - `mint2` (query, `string`): Filter for pools that include this mint. Optional.
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`

### `GET /pools/key/ids`
- **Summary:** Pool Key
- **Description:** get pool key
- **Tags:** POOLS
- **Parameters:**
  - `ids` (query, `string`): id.join(',')
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `array`
        - Type: `object`

### `GET /pools/line/liquidity`
- **Summary:** Pool Liquidity history
- **Description:** pool liquidity history (max 30d)
- **Tags:** POOLS
- **Parameters:**
  - `id` (query, `string`): pool id
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`
      - `count`: `number` - day
      - `line`: `array` - 
        - Items: `object`
          - Type: `object`
          - `time`: `number` - utc time.
          - `liquidity`: `number` - pool tvl.

### `GET /pools/line/position`
- **Summary:** Clmm Position
- **Description:** clmm position line
- **Tags:** POOLS
- **Parameters:**
  - `id` (query, `string`): pool id
- **Responses:**
  - **200**: success
    - **Content Type:** `application/json`
      - Type: `object`
      - `count`: `number` - day
      - `line`: `array` - 
        - Items: `object`
          - Type: `object`
          - `price`: `number` - pool price.
          - `tick`: `number` - pool tick.
          - `liquidity`: `number` - pool tvl.