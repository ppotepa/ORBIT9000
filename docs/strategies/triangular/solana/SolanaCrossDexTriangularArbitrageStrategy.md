# Solana Cross-DEX Triangular Arbitrage Strategy

## Strategy Overview

SolanaCrossDexTriangularArbitrage is a cross-exchange triangular arbitrage strategy designed for the TradeMonitor platform. Its purpose is to exploit price discrepancies between three different tokens across multiple Solana DEXs, executing a three-leg loop (Token A → Token B → Token C → back to Token A) for profit. In traditional triangular arbitrage, all three trades occur on one exchange. In contrast, SolanaCrossDexTriangularArbitrage extends this concept across multiple decentralized exchanges on Solana (e.g. Raydium, Orca, Jupiter, Meteora). This broader scope allows the strategy to capture inter-exchange inefficiencies that a single-DEX strategy might miss.

The expected use case is an algorithmic trading bot continuously scanning Solana DEX markets for triangular cycles that yield a net gain in the starting asset after accounting for fees and slippage. This strategy is particularly useful for advanced DeFi arbitrageurs aiming to maximize returns from Solana's vibrant multi-DEX ecosystem.

**Use Case Example:** A trader running SolanaCrossDexTriangularArbitrage might find an opportunity where SOL, USDC, and BONK prices are misaligned across Raydium, Orca, and Jupiter. For instance, starting with SOL on Raydium, swapping SOL → USDC, then using Orca to swap USDC → BONK, and finally swapping BONK → SOL on Jupiter could result in more SOL than initially started (if such price differences exist). The strategy automates detection of such cycles and executes them faster than a human could, aiming to lock in risk-free (or minimal risk) profit.

## Integration with TradeMonitor Architecture

This strategy is implemented in alignment with TradeMonitor's clean architecture and plugin-style strategy framework. It is provided as a new strategy class (`SolanaCrossDexTriangularArbitrageStrategy`) that implements the `ITradingStrategy` interface. The class exposes a Name and an `AnalyzeMarketAsync` method containing the core arbitrage-finding logic. By adhering to the ITradingStrategy contract, the strategy cleanly plugs into TradeMonitor's strategy pattern architecture, which treats trading algorithms as interchangeable components.

**Dependency Injection Integration:** When integrating SolanaCrossDexTriangularArbitrage into TradeMonitor, you would register it via dependency injection (DI) so that the TradeMonitor Core can instantiate and use it. The strategy can be added to the service container and bound to the ITradingStrategy interface. This allows the Trading Bot component to receive the strategy instance via constructor injection, consistent with the platform's dependency inversion principle.

**Configuration:** The strategy will typically be configured in the appsettings.json or similar config file by specifying its Type name and parameters. TradeMonitor's strategy loader uses this Type field to instantiate the correct strategy class at runtime.

**Exchange Services:** SolanaCrossDexTriangularArbitrage leverages TradeMonitor's Exchange Integration layer to interact with Solana DEX APIs. It uses Solana DEX service implementations (e.g. RaydiumService, OrcaService, JupiterService, MeteoraService) provided under TradeMonitor.Exchanges.Services. These services supply real-time market data (order books, pool reserves, prices) and execute trades on their respective DEXs. The strategy itself does not hard-code API calls; instead, it relies on these exchange service abstractions to fetch the latest prices and carry out swap orders.

**Data & Persistence:** The strategy can utilize TradeMonitor's Data layer for any required persistence or reference data. Generally, most strategy state (current opportunities, in-flight trades) is kept in memory or in the bot's context, not long-term storage. Any executed trades and outcomes will be recorded to the database via the normal TradeMonitor bot lifecycle.

**Command Channel for Execution:** Consistent with TradeMonitor's command pattern design, SolanaCrossDexTriangularArbitrage does not execute trades directly in the strategy method. Instead, once an arbitrage opportunity is identified and validated, the strategy constructs a sequence of trade instructions and dispatches them to the platform's Command Channel as atomic commands. This decoupling via the command pattern enhances reliability.

**Core and Risk Management Integration:** The strategy integrates with the Trading Bot which orchestrates its lifecycle. The bot calls the strategy's analyze method at a configured interval, passing in aggregated market data. The results are delivered as a TradingOpportunity object, which the bot then evaluates against risk management rules before execution.

## Configuration & Adjustable Parameters

SolanaCrossDexTriangularArbitrage offers a range of runtime-configurable parameters to fine-tune its behavior:

**MinimumProfitThreshold:** Profit Floor for Execution. This represents the minimum expected profit required for an arbitrage cycle to be considered worthwhile. It is typically expressed as a proportion of the starting amount (e.g. 0.005 for 0.5% profit). The strategy will ignore any triangular opportunity unless the estimated net gain (after fees and slippage) exceeds this threshold. This guards against trivial or potentially negative-profit trades once transaction costs are considered.

**MaxTradeAmount:** Upper Trade Size Limit. This parameter caps the amount of the starting token that the strategy will use in any arbitrage loop. Even if a cycle appears profitable, the strategy will not trade more than this amount in one go. This helps limit exposure and slippage; large trades can move prices significantly, so capping trade size is a risk control.

**MinTradeAmount:** Lower Trade Size Limit. This is the smallest trade amount the strategy will attempt. It prevents execution of very tiny arbitrages that would yield negligible absolute profit or be lost to fees. This ensures efficient use of resources and avoids wasting transaction fees on dust trades.

**MaxExecutionTimeSeconds:** Timeout for Cycle Execution. This defines the maximum allowed time (in seconds) to complete the full three-leg arbitrage transaction. If the sequence of trades cannot be completed within this time, the opportunity is considered too risky and should be skipped. This parameter helps mitigate execution risk by avoiding stale trades.

**SlippageTolerance:** Allowed Price Slippage. This setting indicates how much worse the execution price is allowed to be compared to the initial quote before the trade is aborted. Each leg of the triangle can move in price by the time the trade executes; slippage tolerance ensures we account for that. The strategy uses this in simulation to only consider opportunities that remain profitable even with slippage.

**SupportedTokens:** Whitelist of Tokens to Consider. A list of token symbols that the strategy will include in its search for cycles. By restricting to a known set of tokens, we limit the search space and avoid illiquid or unwanted assets. This list is adjustable at runtime – adding or removing tokens changes which triangles are scanned.

**SupportedExchanges:** Which DEXs to Use. This is a list of Solana exchanges on which the strategy will look for price differences. Only pools/routes on these exchanges are considered when building the arbitrage cycles. All exchanges listed here must have corresponding IExchangeService implementations in TradeMonitor.

**MaxConcurrentTrades:** Parallel Trade Limit. This setting controls how many arbitrage cycles the strategy can execute simultaneously. A value of 1 means the strategy will only undertake one triangle arbitrage at a time. Higher values allow multiple independent opportunities to be executed concurrently if found.

**ExecutionMode:** Execution strategy for triangular arbitrage cycles:
- **SEQUENTIAL:** Execute three legs sequentially (safer, higher execution risk)
- **ATOMIC:** Execute all three legs in one atomic transaction (preferred on Solana)
- **ATOMIC_PREFERRED:** Try atomic first, fallback to sequential if needed

## Risk Profiles (Low, Medium, High)

To accommodate different risk appetites, the strategy can be run under preset risk profiles:

### Low Risk Profile
Prioritizes capital preservation and high-confidence trades:
- **MinimumProfitThreshold:** 1.0% (require substantial profit cushion)
- **SlippageTolerance:** 0.5% (tight execution requirements)
- **MaxTradeAmount:** Conservative trade sizes
- **SupportedTokens:** Major tokens only (SOL, USDC, USDT, wETH)
- **SupportedExchanges:** Most reliable exchanges (Raydium, Orca, Jupiter)
- **MaxConcurrentTrades:** 1 (no simultaneous positions)
- **ExecutionMode:** ATOMIC (safest execution)

Focus is on quality over quantity: only obvious, low-risk arbitrage loops are executed.

### Medium Risk Profile
A balanced approach between safety and opportunity:
- **MinimumProfitThreshold:** 0.5% (moderate profit requirement)
- **SlippageTolerance:** 1.5% (accepting some price movement risk)
- **MaxTradeAmount:** Reasonable fraction of typical pool liquidity
- **SupportedTokens:** Extended list including ecosystem tokens
- **SupportedExchanges:** All major Solana DEXs
- **MaxConcurrentTrades:** 1-2 (limited concurrency)
- **ExecutionMode:** ATOMIC_PREFERRED (atomic with sequential fallback)

Executes opportunities that have solid expected profit and manageable risk.

### High Risk Profile
Focuses on maximizing profit and capturing even small arbitrage gaps:
- **MinimumProfitThreshold:** 0.1% (capture fine margin trades)
- **SlippageTolerance:** 2.0% (wider tolerance for price movement)
- **MaxTradeAmount:** Larger positions utilizing more capital
- **SupportedTokens:** Broad selection including volatile/lesser-known tokens
- **SupportedExchanges:** All available exchanges
- **MaxConcurrentTrades:** 2-3 (multiple parallel cycles)
- **ExecutionMode:** ATOMIC_PREFERRED with aggressive parameters

This profile is aggressive and expects close monitoring and experience.

## Arbitrage Cycle Detection Logic (8-Phase Algorithm)

Detecting profitable cycles across multiple exchanges is implemented through an 8-phase algorithm:

### Phase 1: Graph Construction
The strategy builds directed graphs for each exchange where nodes represent tokens and edges represent available swap paths. Each edge carries a weight representing the current conversion rate from the source token to the target token on that exchange, factoring in fees.

### Phase 2: Cycle Search
With multi-exchange liquidity graphs, the strategy searches for cycles of length 3 that start and end at the same token (A → B → C → A). The search evaluates all possible cross-exchange combinations:
- Brute-force approach: Iterate over all triples of distinct tokens and check if edges exist across different exchange combinations
- Graph traversal: For each token A, look at all possible B such that an edge A→B exists on any exchange, then find C and check for C→A on any exchange

### Phase 3: Candidate Filtering
Not every cycle is profitable. For each detected cycle A→B→C→A, the strategy computes the cumulative conversion rate by multiplying the rates for each leg. If the resulting product > 1, an arbitrage exists. The strategy further requires it to exceed (1 + MinimumProfitThreshold) to account for desired profit.

### Phase 4: Cycle Selection
If multiple profitable cycles are found, the strategy ranks them by highest expected ROI, absolute profit potential, and liquidity considerations. It considers criteria like whether a cycle might scale to useful size and selects the best opportunity.

### Phase 5: Trade Simulation
The strategy simulates the trade to estimate actual profit, accounting for slippage and fees:
- **Determine Trade Amount:** Calculate optimal trade size based on liquidity and parameters
- **Simulate Leg-by-Leg Conversion:** Use current pricing data to simulate each swap
- **Apply Slippage Tolerance:** Account for price movement during execution

### Phase 6: Slippage-Adjusted Profit Estimation
Calculate final profit after applying conservative slippage estimates. Compare the final simulated amount versus the initial amount, subtract transaction fees and other costs, and verify the cycle still yields acceptable profit.

### Phase 7: Cross-Exchange Route Optimization
Optimize the execution path considering:
- Exchange reliability and performance
- Optimal timing for cross-exchange execution
- Gas/fee optimization across the route
- Atomic vs sequential execution decision

### Phase 8: Opportunity Generation
Create a TradingOpportunity object containing:
- The complete path: A → B → C → A and specific exchanges for each hop
- Recommended trade amount and execution parameters
- Estimated profit and confidence score
- Execution instructions and slippage parameters

## Execution Path: Sequential vs. Atomic Transactions

The strategy supports multiple execution modes:

### Sequential Execution
Execute each leg as a separate trade in sequence:
- **Pros:** Simpler implementation, easier debugging, normal swap through exchange APIs
- **Cons:** Execution risk (market can move between legs), higher transaction fees, longer execution time

### Atomic Execution
Execute all three legs within a single atomic transaction:
- **Pros:** No execution risk (all-or-nothing), faster execution, eliminates mid-cycle market movement risk
- **Cons:** More complex transaction building, potential compute/size limits, requires low-level program knowledge

### Atomic Preferred (Recommended)
Try atomic execution first, fallback to sequential if needed:
- Combines safety of atomic execution with reliability of sequential fallback
- Handles edge cases where atomic execution might fail
- Provides optimal execution in most scenarios

## Example Configuration Blocks

### Default Medium Risk Configuration:
```json
{
  "TradingBot": {
    "WalletId": "cross-dex-arbitrage-bot",
    "InitialBalance": 15000.0,
    "Strategy": {
      "Type": "SolanaCrossDexTriangularArbitrage",
      "MinimumProfitThreshold": 0.005,
      "MaxTradeAmount": 500.0,
      "MinTradeAmount": 50.0,
      "MaxExecutionTimeSeconds": 30,
      "SlippageTolerance": 0.015,
      "SupportedTokens": ["SOL", "USDC", "USDT", "BONK", "RAY", "ORCA"],
      "SupportedExchanges": ["RAYDIUM", "ORCA", "METEORA", "JUPITER"],
      "CustomParameters": {
        "RiskProfile": "MEDIUM",
        "ExecutionMode": "ATOMIC_PREFERRED"
      }
    },
    "MonitoringInterval": 3000,
    "MaxConcurrentTrades": 1
  }
}
```

### Low Risk Configuration:
```json
"Strategy": {
  "Type": "SolanaCrossDexTriangularArbitrage",
  "MinimumProfitThreshold": 0.01,
  "MaxTradeAmount": 200.0,
  "SlippageTolerance": 0.005,
  "SupportedTokens": ["SOL", "USDC", "USDT", "wETH"],
  "SupportedExchanges": ["RAYDIUM", "ORCA", "JUPITER"],
  "CustomParameters": {
    "RiskProfile": "LOW",
    "ExecutionMode": "ATOMIC"
  }
}
```

### High Risk Configuration:
```json
"Strategy": {
  "Type": "SolanaCrossDexTriangularArbitrage",
  "MinimumProfitThreshold": 0.001,
  "MaxTradeAmount": 1000.0,
  "SlippageTolerance": 0.02,
  "SupportedTokens": ["SOL", "USDC", "BONK", "RAY", "ORCA", "MSOL", "STSOL", "wETH"],
  "SupportedExchanges": ["RAYDIUM", "ORCA", "METEORA", "JUPITER", "PHOENIX", "LIFINITY"],
  "MaxConcurrentTrades": 3,
  "CustomParameters": {
    "RiskProfile": "HIGH",
    "ExecutionMode": "ATOMIC_PREFERRED"
  }
}
```

## Example Arbitrage Scenario

**Scenario Setup:** Cross-DEX price discrepancy between SOL, USDC, and BONK

**Market Conditions:**
- Raydium: 1 SOL = 25 USDC (SOL slightly underpriced)
- Orca: 1 USDC = 200 BONK (BONK underpriced relative to USDC)
- Jupiter: 5,000 BONK = 1.10 SOL (effective rate via aggregation)

**Triangle Detection:**
Starting with 1 SOL, the cycle SOL → USDC → BONK → SOL using (Raydium, Orca, Jupiter):

1. **Leg 1 (SOL → USDC on Raydium):** 1 SOL → 25 USDC
2. **Leg 2 (USDC → BONK on Orca):** 25 USDC → 5,000 BONK
3. **Leg 3 (BONK → SOL on Jupiter):** 5,000 BONK → 1.10 SOL

**Result:** Net profit of 0.10 SOL (10% gain) minus transaction fees.

**Execution:** The strategy would execute this atomically in one transaction calling Raydium, Orca, and Jupiter swap instructions, ensuring either the entire cycle succeeds yielding ~1.10 SOL, or fails with no partial trades.

This strategy integrates tightly with TradeMonitor's modular system, using Core logic to evaluate opportunities, the Exchanges layer to get prices and execute swaps, the Data layer to log outcomes, and the CommandChannel to perform trades in an orchestrated fashion. By adhering to strategy pattern principles and robust risk management, it aims to capitalize on cross-DEX inefficiencies in Solana's ecosystem while safeguarding the trader through configurable thresholds, slippage protection, and strict execution controls.
