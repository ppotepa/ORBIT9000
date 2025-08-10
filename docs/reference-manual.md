# TradeMonitor Reference Manual

## Table of Contents
1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Exchange Services](#exchange-services)
4. [API Endpoints](#api-endpoints)
5. [Configuration](#configuration)
6. [Integration Tests](#integration-tests)
7. [Error Handling](#error-handling)
8. [Troubleshooting](#troubleshooting)

## Overview

TradeMonitor is a comprehensive cryptocurrency trading and market data aggregation system built for the Solana ecosystem. It provides real-time price data, market analytics, and arbitrage opportunities across multiple exchanges.

### Key Features
- **Multi-Exchange Support**: Jupiter (Aggregator), Raydium (DEX), Orca (DEX), Meteora (DEX)
- **Real-Time Market Data**: 168 trading pairs across 80+ tokens
- **Arbitrage Detection**: Cross-exchange opportunity identification
- **Health Monitoring**: Exchange connectivity and performance tracking
- **Comprehensive Analytics**: Volume, liquidity, and price movement analysis

### System Requirements
- .NET 8.0
- HTTP Client connectivity to Solana mainnet
- Memory: ~100MB for full market scanning
- Network: Stable internet connection (4 concurrent API calls)

## Architecture

### Core Components

```
TradeMonitor
├── Core/                     # Core domain logic and interfaces
│   ├── Services/Base/        # ExchangeServiceBase abstract class
│   ├── Enums/               # Token, Exchange, Network enums
│   ├── Utils/               # TokenMapping, extension methods
│   └── Models/              # Data transfer objects
├── Exchanges/               # Exchange service implementations
│   ├── Aggregator/Solana/   # Jupiter aggregator service
│   └── Dex/Solana/         # DEX services (Raydium, Orca, Meteora)
├── Application/             # Main application entry point
└── Tests/                   # Integration and unit tests
```

### Design Patterns
- **Strategy Pattern**: Exchange services implement common interface
- **Factory Pattern**: Exchange service instantiation
- **Producer-Consumer**: Market data aggregation
- **Circuit Breaker**: Error handling and resilience

## Exchange Services

### 1. Jupiter Exchange Service (Aggregator)

**Type**: Price Aggregator  
**API Version**: v2  
**Base URL**: `https://api.jup.ag/price/v2`

#### Features
- Aggregates prices from multiple Solana DEXs
- Optimal routing for best prices
- Real-time price discovery
- Comprehensive token coverage

#### API Endpoints Used
```http
GET /price?ids={mint}&vsToken={quoteMint}
```

#### Response Format
```json
{
  "data": {
    "{mintAddress}": {
      "id": "So11111111111111111111111111111111111111112",
      "price": 145.47,
      "mintSymbol": "SOL",
      "vsToken": "EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v",
      "vsTokenSymbol": "USDC"
    }
  },
  "timeTaken": 0.015
}
```

#### Error Codes
- `401`: API key required for some endpoints
- `404`: Token mint not found
- `429`: Rate limit exceeded
- `500`: Internal server error

### 2. Raydium Exchange Service (DEX)

**Type**: Automated Market Maker (AMM)  
**API Version**: v3  
**Base URL**: `https://api-v3.raydium.io`

#### Features
- Standard AMM pools
- Concentrated Liquidity Market Maker (CLMM)
- Yield farming information
- Pool analytics

#### API Endpoints Used
```http
GET /mint/price?mints={mint1},{mint2}
GET /pools/info/list-v2?poolType=all&size=100
```

#### Response Format
```json
{
  "id": "request-id",
  "success": true,
  "data": {
    "So11111111111111111111111111111111111111112": "145.47",
    "EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v": "1.00"
  }
}
```

#### Pool Types
- **Standard**: Traditional AMM pools (50/50 liquidity)
- **Concentrated**: CLMM pools with custom price ranges
- **Farm**: Pools with additional token rewards

### 3. Orca Exchange Service (DEX)

**Type**: Automated Market Maker  
**API Version**: v2  
**Base URL**: `https://api.orca.so/v2/solana`

#### Features
- Whirlpool (concentrated liquidity) support
- Stable pools for correlated assets
- Multi-token pools
- Advanced fee structures

#### API Endpoints Used
```http
GET /pools?limit=100&sortBy=tvl&sortDirection=desc
GET /pools/search?query={tokenSymbol}
```

#### Response Format
```json
[
  {
    "address": "pool-address",
    "tokenA": {
      "mint": "So11111111111111111111111111111111111111112",
      "symbol": "SOL"
    },
    "tokenB": {
      "mint": "EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v",
      "symbol": "USDC"
    },
    "tvl": 5000000.00,
    "volume24h": 1000000.00,
    "feeRate": 0.003
  }
]
```

#### Pool Features
- **hasRewards**: Pool provides additional token incentives
- **hasWarning**: Pool has potential risks (new/unverified tokens)
- **useAdaptiveFee**: Dynamic fee structure based on volatility

### 4. Meteora Exchange Service (DEX)

**Type**: Dynamic Liquidity Market Maker (DLMM)  
**API Version**: 1.0.9  
**Base URL**: `https://dlmm-api.meteora.ag`

#### Features
- Dynamic bin-based liquidity
- MEV protection
- Auto-compounding rewards
- Cross-chain compatibility

#### API Endpoints Used
```http
GET /pair/all?include_unknown=false
GET /info/protocol_metrics
```

#### Response Format
```json
{
  "total_tvl": 50000000.00,
  "daily_trade_volume": 5000000.00,
  "total_trade_volume": 1000000000.00,
  "daily_fee": 15000.00,
  "total_fee": 3000000.00
}
```

#### DLMM Features
- **Bin Strategy**: Liquidity concentrated in discrete price bins
- **Auto-compounding**: Automatic reinvestment of trading fees
- **MEV Protection**: Front-running resistance through batch processing

## API Endpoints

### Comprehensive Market Data Endpoints

The system aggregates data from multiple endpoints to provide comprehensive market coverage:

#### Price Data Sources
1. **Jupiter Price API**: Real-time aggregated prices
2. **Raydium Mint Prices**: Direct DEX pricing
3. **Orca Pool Data**: Whirlpool pricing and analytics
4. **Meteora Pair Data**: DLMM pool information

#### Data Refresh Rates
- **Real-time**: Price queries (on-demand)
- **5 minutes**: Market data aggregation
- **1 hour**: Protocol metrics updates
- **Daily**: Analytics and reporting

### Token Support

The system supports 80+ tokens with comprehensive mint address mapping:

#### Major Tokens
- **SOL**: Native Solana token
- **USDC/USDT**: Stablecoins
- **RAY**: Raydium governance token
- **ORCA**: Orca protocol token
- **JUP**: Jupiter aggregator token

#### Token Mapping
```csharp
public static class TokenMapping
{
    public static string GetSolanaAddress(Token token) => token switch
    {
        Token.SOL => "So11111111111111111111111111111111111111112",
        Token.USDC => "EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v",
        Token.USDT => "Es9vMFrzaCERmJfrF4H2FYD4KCoNkY11McCe8BenwNYB",
        Token.RAY => "4k3Dyjzvzp8eMZWUXbBCjEvwSkkk59S5iCNLY3QrkX6R",
        Token.ORCA => "orcaEKTdK7LKz57vaAYr9QeNsVEPfiu6QeMU1kektZE",
        Token.JUP => "JUPyiwrYJFskUPiHa7hkeR8VUtAeFoSYbKedZNsDvCN",
        _ => string.Empty
    };
}
```

## Configuration

### Application Settings

The system uses `appsettings.json` for configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "TradeMonitor.Exchanges": "Debug",
      "System.Net.Http": "Warning"
    }
  },
  "HttpClient": {
    "TimeoutSeconds": 30,
    "MaxRetries": 3,
    "RetryDelayMs": 1000
  },
  "MarketScanning": {
    "IntervalSeconds": 300,
    "MaxConcurrentRequests": 4,
    "EnableArbitrageDetection": true
  },
  "Exchanges": {
    "Jupiter": {
      "Enabled": true,
      "Priority": 1,
      "ApiKey": ""
    },
    "Raydium": {
      "Enabled": true,
      "Priority": 2,
      "Network": "MAINNET"
    },
    "Orca": {
      "Enabled": true,
      "Priority": 3
    },
    "Meteora": {
      "Enabled": true,
      "Priority": 4
    }
  }
}
```

### Environment Configuration

#### Development
- Use test networks where available
- Reduced polling intervals
- Enhanced logging
- Mock data for unreliable endpoints

#### Production
- Mainnet only
- Optimized polling rates
- Error alerting
- Performance monitoring

## Integration Tests

### Test Coverage

The integration test suite validates:

1. **Core Functionality**: Price retrieval for all exchanges
2. **Cross-Exchange Validation**: Price consistency checks
3. **Health Monitoring**: Exchange availability and performance
4. **Error Handling**: Network failures and invalid requests
5. **Resilience**: Timeout and rate limit handling

### Test Execution

```bash
# Run all integration tests
dotnet test --filter "Category=Integration"

# Run specific exchange tests
dotnet test --filter "TestCategory=Jupiter"
dotnet test --filter "TestCategory=Raydium"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### Test Configuration

Tests use realistic scenarios with live API endpoints:

```csharp
[Fact]
public async Task Jupiter_GetPrice_ReturnsValidData()
{
    var service = new JupiterExchangeService(_logger, _httpClient);
    
    var price = await service.GetPriceAsync(Token.SOL, Token.USDC);
    
    Assert.True(price > 0, "Jupiter should return valid SOL/USDC price");
}
```

### Performance Benchmarks

Expected performance characteristics:

| Exchange | Avg Response Time | Success Rate | Supported Pairs |
|----------|------------------|--------------|-----------------|
| Jupiter  | 50-100ms         | >95%         | 150+            |
| Raydium  | 100-200ms        | >90%         | 100+            |
| Orca     | 80-150ms         | >90%         | 80+             |
| Meteora  | 100-300ms        | >85%         | 60+             |

## Error Handling

### Common Error Scenarios

#### 1. Network Connectivity Issues
```csharp
catch (HttpRequestException ex)
{
    Logger.LogError(ex, "Network error for {Exchange}: {Message}", 
        ExchangeName, ex.Message);
    return 0m; // Return default value
}
```

#### 2. API Rate Limiting
```csharp
catch (HttpRequestException ex) when (ex.Message.Contains("429"))
{
    Logger.LogWarning("Rate limit hit for {Exchange}, implementing backoff", 
        ExchangeName);
    await Task.Delay(TimeSpan.FromSeconds(5));
    // Retry logic here
}
```

#### 3. Invalid Token Pairs
```csharp
protected bool IsValidTokenPair(Token baseToken, Token quoteToken)
{
    return baseToken != Token.Unknown && 
           quoteToken != Token.Unknown && 
           baseToken != quoteToken;
}
```

#### 4. JSON Parsing Errors
```csharp
catch (JsonException ex)
{
    Logger.LogError(ex, "JSON parsing error for {Exchange}: {Message}", 
        ExchangeName, ex.Message);
    return new MarketData { IsValid = false };
}
```

### Resilience Patterns

#### Circuit Breaker
```csharp
public class ExchangeCircuitBreaker
{
    private int _failureCount = 0;
    private DateTime _nextAttempt = DateTime.MinValue;
    
    public bool ShouldAttempt()
    {
        if (_failureCount >= 5 && DateTime.UtcNow < _nextAttempt)
            return false;
            
        return true;
    }
}
```

#### Exponential Backoff
```csharp
private async Task<T> RetryWithBackoff<T>(Func<Task<T>> operation, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await operation();
        }
        catch when (i < maxRetries - 1)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(Math.Pow(2, i) * 1000));
        }
    }
    throw new InvalidOperationException("All retry attempts failed");
}
```

## Troubleshooting

### Common Issues and Solutions

#### 1. DNS Resolution Failures

**Symptoms**: `No such host is known` errors
**Cause**: Incorrect API endpoint URLs
**Solution**: Verify endpoint URLs in service configuration

```csharp
// Correct Jupiter endpoint
private const string PRICE_API_URL = "https://api.jup.ag/price/v2";

// Not: "https://price.jup.ag/v6" (old/incorrect)
```

#### 2. Authorization Errors

**Symptoms**: HTTP 401 responses
**Cause**: Missing or invalid API keys
**Solution**: 
- Jupiter: Some endpoints require API keys for production use
- Most read-only endpoints work without authentication
- Check rate limits for unauthenticated access

#### 3. Low Success Rates

**Symptoms**: Many failed price retrievals
**Cause**: Network issues, rate limiting, or exchange downtime
**Solution**:
1. Check exchange status pages
2. Implement exponential backoff
3. Use fallback exchanges
4. Monitor network connectivity

#### 4. Inconsistent Prices

**Symptoms**: Large price variations between exchanges
**Cause**: Different liquidity pools, arbitrage opportunities, or stale data
**Solution**:
1. Check timestamp of price data
2. Verify token mint addresses
3. Consider volume and liquidity differences
4. Implement price deviation alerts

### Debugging Tools

#### 1. Enhanced Logging
```csharp
Logger.LogDebug("🌐 {Exchange} API call: {Url}", ExchangeName, url);
Logger.LogDebug("📡 {Exchange} API response: {Response}", ExchangeName, jsonContent);
```

#### 2. Request Tracing
```csharp
var requestId = Guid.NewGuid().ToString("N")[..8];
Logger.LogDebug("🌐 [{RequestId}] {Exchange}: Starting {Description}",
    requestId, ExchangeName, description);
```

#### 3. Performance Monitoring
```csharp
var stopwatch = Stopwatch.StartNew();
// ... operation ...
stopwatch.Stop();
Logger.LogInformation("⏱️ {Exchange}: {Operation} completed in {ElapsedMs}ms",
    ExchangeName, operation, stopwatch.ElapsedMilliseconds);
```

### Health Check Dashboard

Monitor system health using the built-in health check endpoint:

```http
GET /health
```

Response includes:
- Exchange connectivity status
- Response times
- Success rates
- Error counts
- Last successful update times

### Log Analysis

Key log patterns to monitor:

#### Success Patterns
```
✅ Jupiter REAL price: SOL/USDC = $145.47 (took 50ms)
🎯 ExchangeAggregate: Successfully retrieved 168 trading pairs
📊 Health check completed: 3/4 tests passed (75.0%) in 250ms
```

#### Warning Patterns
```
⚠️ Jupiter API returned 429: Too Many Requests
⚠️ Raydium: Price deviation detected: $145.47 vs $148.23 (1.9%)
⚠️ Orca: Reduced liquidity detected for ORCA/USDC pair
```

#### Error Patterns
```
❌ Meteora: Network timeout after 30000ms
❌ Jupiter: No price data found for UNKNOWN/USDC
💥 ExchangeAggregate: Critical error in market scanning
```

---

## Support and Maintenance

### Regular Maintenance Tasks

1. **Weekly**: Review exchange API changes and deprecations
2. **Monthly**: Update token mint address mappings
3. **Quarterly**: Performance optimization and capacity planning
4. **As needed**: API endpoint updates and error handling improvements

### Monitoring Recommendations

1. **Uptime**: Monitor exchange availability (target: >95%)
2. **Response Times**: Alert on average response times >500ms
3. **Success Rates**: Alert on success rates <90%
4. **Error Patterns**: Monitor for new error types or increases in error rates

### Contact Information

- **Development Team**: TradeMonitor Development Team
- **API Support**: Individual exchange support channels
- **Emergency Contact**: System administrator for production issues

---

*Last Updated: February 2025*  
*Version: 2.0.0*
