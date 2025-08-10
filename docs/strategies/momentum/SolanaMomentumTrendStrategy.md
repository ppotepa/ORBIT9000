# Solana Momentum Trend Strategy

## Strategy Overview

SolanaMomentumTrend is a momentum-based trading strategy designed for the TradeMonitor platform. Its purpose is to capture price momentum and volume spikes across Solana DEXes by identifying tokens with strong directional moves supported by increased trading activity. Unlike arbitrage strategies that exploit price differences, momentum strategies aim to ride trends by entering positions when momentum is building and exiting when momentum fades.

The strategy employs multi-timeframe analysis (ultra-short to long-term) to detect momentum at different scales, cross-exchange momentum confirmation to increase signal reliability, and volume spike detection to identify increased market interest. This comprehensive approach helps filter out false signals and focus on high-probability momentum plays.

**Use Case Example:** A trader running SolanaMomentumTrend might detect that BONK is showing strong upward momentum on Raydium with a 3% price increase in 5 minutes, accompanied by 2.5x normal volume and similar momentum patterns on Orca and Meteora. The strategy would generate a buy signal with calculated stop-loss and take-profit levels, allowing the trader to ride the momentum wave while managing risk.

## Integration with TradeMonitor Architecture

This strategy integrates seamlessly with TradeMonitor's clean architecture and follows the established ITradingStrategy interface pattern. It implements the same initialization and analysis methods as other strategies but focuses on momentum detection rather than arbitrage.

**Strategy Implementation:** The class `SolanaMomentumTrendStrategy` implements `ITradingStrategy` and exposes the standard methods including `InitializeAsync` and `AnalyzeMarketAsync`. The strategy is decorated with comprehensive attributes that define its characteristics, supported tokens, and exchange requirements.

**Dependency Injection Integration:** Like other strategies, it registers via DI in the TradeMonitor.Application.Extensions and can be configured in appsettings.json by specifying its Type name and parameters. The strategy loader instantiates it at runtime based on configuration.

**Exchange Services:** The strategy leverages TradeMonitor's Exchange Integration layer to access real-time market data from Solana DEXes. It uses the same `IExchangeService` abstractions as other strategies, ensuring it can work with any supported exchange (Raydium, Orca, Meteora, Jupiter, etc.) without modification.

**Data & Historical Analysis:** Unlike arbitrage strategies that primarily work with current prices, momentum strategies require historical price and volume data. The strategy maintains internal price and volume history caches and could optionally integrate with TradeMonitor's Data layer for persistence of longer-term historical data.

**Command Channel Execution:** Following TradeMonitor's command pattern, the strategy generates `TradingOpportunity` objects rather than executing trades directly. The bot framework handles the actual trade execution through the CommandChannel, maintaining separation of concerns.

**Risk Management Integration:** The strategy includes comprehensive risk assessment in Phase 5 of its analysis, calculating volatility, liquidity risk, and momentum sustainability. It integrates with TradeMonitor's risk management framework through standard risk scoring and position sizing calculations.

## Configuration & Adjustable Parameters

SolanaMomentumTrend offers extensive runtime-configurable parameters to adapt its behavior to different market conditions and risk tolerances:

**MinimumProfitThreshold:** Sets the minimum price movement required to trigger a momentum signal. For momentum strategies, this typically ranges from 0.3% for high-frequency scalping to 2% for longer-term trend following. Default: 0.8%

**MaxTradeAmount:** Maximum position size for any single momentum trade. Since momentum trades can be more volatile than arbitrage, this should be set conservatively relative to total capital. The strategy uses position sizing algorithms that scale with momentum strength and confidence.

**MinTradeAmount:** Minimum trade size to avoid executing tiny positions that might be unprofitable after fees. For momentum strategies, this should account for the need to capture meaningful price moves.

**MaxExecutionTimeSeconds:** Maximum time allowed for momentum trade execution. Unlike arbitrage which requires immediate execution, momentum trades can have longer execution windows (5-60 minutes) since the underlying trend may persist.

**SlippageTolerance:** Acceptable price movement during execution. Momentum strategies typically allow higher slippage tolerance since they're riding price movements rather than capturing exact price differences.

**SupportedTokens:** List of tokens to monitor for momentum signals. Should focus on liquid tokens with sufficient volatility to generate meaningful momentum signals. Example: ["SOL", "USDC", "BONK", "RAY", "ORCA", "MSOL", "JUP", "WIF"]

**SupportedExchanges:** DEXes to monitor for momentum detection. Priority is given to high-volume exchanges for primary signals, with smaller exchanges used for confirmation.

**MomentumTimeframe:** Primary timeframe for momentum analysis (ULTRA_SHORT: 1-5min, SHORT: 5-15min, MEDIUM: 15-60min, LONG: 1-4hr). This is automatically set based on MaxExecutionTime but can be overridden.

**ConfidenceThreshold:** Minimum confidence score (0-1) required for signal validation. Higher values reduce false positives but may miss opportunities. Default: 0.75

**StopLossThreshold:** Base stop-loss percentage, adjusted dynamically based on volatility and momentum strength. Default: 2%

**PositionSizePercentage:** Percentage of MaxTradeAmount to use as base position size, scaled by confidence and risk metrics. Default: 10%

## Risk Profiles (Low, Medium, High)

The strategy supports three risk profiles that adjust parameters for different trading styles:

**Low Risk Profile:**
- MinimumProfitThreshold: 1.5% (only strong momentum signals)
- SlippageTolerance: 0.8% (tight execution requirements)
- ConfidenceThreshold: 0.85 (high confidence required)
- PositionSizePercentage: 5% (small positions)
- StopLossThreshold: 1.5% (tight stop losses)
- MomentumTimeframe: MEDIUM to LONG (more stable trends)
- SupportedTokens: Major tokens only (SOL, USDC, USDT, RAY)

**Medium Risk Profile (Default):**
- MinimumProfitThreshold: 0.8% (balanced signal sensitivity)
- SlippageTolerance: 1.2% (reasonable execution flexibility)
- ConfidenceThreshold: 0.75 (balanced confidence requirement)
- PositionSizePercentage: 10% (moderate positions)
- StopLossThreshold: 2% (standard stop losses)
- MomentumTimeframe: SHORT to MEDIUM (good balance)
- SupportedTokens: Extended list including popular memecoins

**High Risk Profile:**
- MinimumProfitThreshold: 0.3% (capture smaller moves)
- SlippageTolerance: 2% (flexible execution)
- ConfidenceThreshold: 0.6 (lower confidence threshold)
- PositionSizePercentage: 15% (larger positions)
- StopLossThreshold: 3% (wider stop losses)
- MomentumTimeframe: ULTRA_SHORT to SHORT (fast moves)
- SupportedTokens: Full list including newer/riskier tokens

## Momentum Detection Algorithm (6-Phase Analysis)

The strategy implements a sophisticated 6-phase momentum detection algorithm:

**Phase 1: Price Change Analysis**
- Calculates momentum indicators across multiple timeframes
- Uses rate of change, acceleration, and trend strength metrics
- Identifies tokens showing significant directional movement
- Filters for moves exceeding MinimumProfitThreshold

**Phase 2: Volume Spike Detection**
- Monitors volume patterns to identify unusual activity
- Calculates volume ratios against historical baselines
- Detects volume spikes indicating increased market interest
- Uses logarithmic intensity scaling for volume analysis

**Phase 3: Cross-Exchange Momentum**
- Compares momentum signals across multiple DEXes
- Requires 70% consensus among exchanges for signal validation
- Increases confidence when momentum is consistent across platforms
- Helps filter out exchange-specific anomalies

**Phase 4: Signal Validation**
- Combines price, volume, and cross-exchange signals
- Calculates composite momentum scores using weighted averages
- Applies confidence thresholds to filter weak signals
- Generates validated momentum signals with direction and strength

**Phase 5: Risk Assessment**
- Evaluates token volatility from historical data
- Assesses liquidity risk based on volume patterns
- Calculates momentum sustainability metrics
- Applies risk-based filtering and position sizing

**Phase 6: Opportunity Generation**
- Selects the highest-scoring momentum signal
- Calculates optimal position size based on strength and risk
- Sets dynamic stop-loss and take-profit levels
- Generates executable TradingOpportunity with metadata

## Execution Strategy & Risk Management

**Position Sizing:** Dynamic position sizing based on momentum strength, confidence level, and risk assessment. Stronger signals with lower risk get larger allocations within the defined limits.

**Stop-Loss Management:** Adaptive stop-losses that account for token volatility and momentum characteristics. Trailing stops can be implemented for trending moves.

**Take-Profit Strategy:** Profit targets set based on momentum strength and historical patterns. Can use partial profit-taking as momentum weakens.

**Risk Controls:** Multiple layers including per-trade risk limits, total exposure caps, volatility-based adjustments, and momentum sustainability checks.

**Execution Timing:** Unlike arbitrage which requires immediate execution, momentum trades can be executed over longer periods, allowing for better entry prices and reduced slippage.

## Example Configuration Blocks

### Default Medium Risk Configuration:
```json
{
  "TradingBot": {
    "WalletId": "sol-momentum-bot-1",
    "InitialBalance": 15000.0,
    "Strategy": {
      "Type": "SolanaMomentumTrend",
      "MinimumProfitThreshold": 0.008,
      "MaxTradeAmount": 750.0,
      "MinTradeAmount": 25.0,
      "MaxExecutionTimeSeconds": 900,
      "SlippageTolerance": 0.012,
      "ConfidenceThreshold": 0.75,
      "StopLossThreshold": 0.02,
      "PositionSizePercentage": 0.10,
      "SupportedTokens": ["SOL", "USDC", "USDT", "RAY", "ORCA", "BONK", "MSOL", "JUP"],
      "SupportedExchanges": ["RAYDIUM", "ORCA", "METEORA", "JUPITER"],
      "MomentumTimeframe": "SHORT"
    },
    "MonitoringInterval": 5000,
    "MaxConcurrentTrades": 2
  }
}
```

### Low Risk Configuration:
```json
"Strategy": {
  "Type": "SolanaMomentumTrend",
  "MinimumProfitThreshold": 0.015,
  "MaxTradeAmount": 300.0,
  "SlippageTolerance": 0.008,
  "ConfidenceThreshold": 0.85,
  "StopLossThreshold": 0.015,
  "PositionSizePercentage": 0.05,
  "SupportedTokens": ["SOL", "USDC", "USDT", "RAY"],
  "MomentumTimeframe": "MEDIUM"
}
```

### High Risk Configuration:
```json
"Strategy": {
  "Type": "SolanaMomentumTrend",
  "MinimumProfitThreshold": 0.003,
  "MaxTradeAmount": 1200.0,
  "SlippageTolerance": 0.02,
  "ConfidenceThreshold": 0.6,
  "StopLossThreshold": 0.03,
  "PositionSizePercentage": 0.15,
  "SupportedTokens": ["SOL", "USDC", "BONK", "RAY", "ORCA", "MSOL", "JUP", "WIF", "POPCAT", "GOAT"],
  "MomentumTimeframe": "ULTRA_SHORT",
  "MaxConcurrentTrades": 3
}
```

## Example Momentum Scenario

**Scenario Setup:** BONK showing strong momentum across multiple Solana DEXes

**Market Conditions:**
- Raydium: BONK/SOL up 2.8% in 5 minutes with 3.2x normal volume
- Orca: BONK/USDC up 2.5% in 5 minutes with 2.8x normal volume  
- Meteora: Similar upward momentum confirmed
- Jupiter: Routing showing consistent pricing across pools

**Strategy Detection Process:**

*Phase 1 - Price Change Analysis:*
- Detects 2.8% short-term momentum (exceeds 0.8% threshold)
- Medium-term momentum shows 1.5% over 15 minutes
- Momentum strength calculated as 0.85 (strong)

*Phase 2 - Volume Spike Detection:*
- Volume spike of 3.2x on Raydium detected
- Volume intensity score of 0.82 calculated
- Confirms increased market interest

*Phase 3 - Cross-Exchange Momentum:*
- 100% consensus across 3 exchanges (all showing bullish momentum)
- Cross-exchange strength: 1.0 (maximum)
- High confidence in signal validity

*Phase 4 - Signal Validation:*
- Composite momentum score: 0.89 (high)
- Exceeds confidence threshold of 0.75
- Direction: BULLISH confirmed

*Phase 5 - Risk Assessment:*
- BONK volatility: 0.45 (moderate)
- Liquidity risk: 0.3 (good volume)
- Momentum sustainability: 0.75 (strong)
- Overall risk score: 0.38 (acceptable)

*Phase 6 - Opportunity Generation:*
- Position size: 450 USDC (60% of max due to high confidence)
- Stop-loss: 2.5% (based on volatility + base threshold)
- Take-profit: 4.2% (1.5x expected move)
- Execution exchange: Raydium (primary momentum source)

**Expected Outcome:** Enter BONK long position to ride the momentum wave, with clear risk management parameters and profit targets based on momentum analysis.

This strategy complements the existing triangular arbitrage strategy by focusing on a different market inefficiency - momentum rather than price discrepancies - while maintaining the same high standards for risk management and systematic execution.
