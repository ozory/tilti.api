# Feature Specification: Passenger Travel Payment Flow

**Feature Branch**: `001-passenger-payment-flow`  
**Created**: May 3, 2026  
**Status**: Draft  
**Input**: User description: "Passenger travel payment flow - pricing, payment, and order creation. Integration with Asaas payment provider. May require wallet creation for passenger."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Price Travel Order (Priority: P1)

The passenger prices a travel order by providing origin, destination, and travel details to see the estimated cost before committing to the trip.

**Why this priority**: This is the first step in the flow. Without pricing, the passenger cannot proceed to payment or order creation. It provides transparency on cost before commitment.

**Independent Test**: Can be fully tested by requesting a price estimate with origin/destination and verifying a cost is returned to the passenger.

**Acceptance Scenarios**:

1. **Given** passenger is authenticated, **When** passenger provides origin and destination addresses, **Then** system returns estimated travel price including distance, duration, and total cost
2. **Given** passenger provides invalid addresses, **When** pricing is requested, **Then** system returns appropriate error message
3. **Given** passenger requests pricing, **When** system calculates cost, **Then** price includes all applicable fees (base fare, distance, duration)

---

### User Story 2 - Create Passenger Wallet (Priority: P2)

The passenger has a digital wallet created in the payment system to enable payment processing.

**Why this priority**: Required for payment processing with the payment provider. Must be completed before any payment can be made.

**Independent Test**: Can be fully tested by submitting passenger information and verifying a wallet is created and linked to the passenger account.

**Acceptance Scenarios**:

1. **Given** passenger does not have a wallet, **When** passenger initiates payment flow, **Then** system creates a wallet for the passenger in the payment system
2. **Given** passenger already has a wallet, **When** payment flow is initiated, **Then** system reuses existing wallet without creating duplicate
3. **Given** wallet creation fails, **When** passenger attempts payment, **Then** system returns appropriate error and prevents payment

---

### User Story 3 - Process Payment for Order (Priority: P1)

The passenger makes a payment for the priced travel order using the integrated payment system.

**Why this priority**: This is the critical step that enables the order to be created. Without successful payment, the travel order cannot be confirmed.

**Independent Test**: Can be fully tested by submitting payment for a priced order and verifying payment is processed successfully with confirmation.

**Acceptance Scenarios**:

1. **Given** passenger has a priced order, **When** passenger submits payment, **Then** system processes payment through payment provider and returns confirmation
2. **Given** passenger has insufficient funds, **When** payment is submitted, **Then** system returns payment failure with appropriate message
3. **Given** payment provider is unavailable, **When** payment is submitted, **Then** system returns appropriate error and allows retry
4. **Given** payment is successful, **When** confirmation is received, **Then** system marks payment as completed and links it to the order

---

### User Story 4 - Create Order After Payment (Priority: P1)

The travel order is created and confirmed only after successful payment, making the order available for driver matching.

**Why this priority**: This is the final step that delivers the core value - a confirmed travel order ready for fulfillment.

**Independent Test**: Can be fully tested by verifying that an order is only created after successful payment and appears in the system for driver matching.

**Acceptance Scenarios**:

1. **Given** payment has been successfully processed, **When** order creation is triggered, **Then** system creates the travel order with status ready for driver matching
2. **Given** payment has not been completed, **When** order creation is attempted, **Then** system rejects order creation and returns appropriate error
3. **Given** order is created, **When** passenger views their orders, **Then** the new order appears with correct details (origin, destination, price, status)

---

### Edge Cases

- What happens when passenger prices an order but waits too long to pay (price expiration)?
- How does system handle partial payment failures (payment provider charges but confirmation is not received)?
- What happens when passenger closes app during payment processing?
- How does system handle duplicate payment attempts for the same order?
- What happens when wallet creation succeeds but payment fails immediately after?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow passengers to request travel price estimates by providing origin and destination
- **FR-002**: System MUST calculate and display total travel cost including distance, duration, and applicable fees
- **FR-003**: System MUST create a digital wallet for passengers in the payment system if one does not exist
- **FR-004**: System MUST reuse existing wallet if passenger already has one in the payment system
- **FR-005**: System MUST process payments through the integrated payment provider (Asaas)
- **FR-006**: System MUST only create travel orders after successful payment confirmation
- **FR-007**: System MUST link payments to their corresponding travel orders
- **FR-008**: System MUST prevent order creation if payment has not been completed successfully
- **FR-009**: System MUST handle payment failures gracefully and provide clear error messages to passengers
- **FR-010**: System MUST return payment confirmation details to passenger after successful payment
- **FR-011**: System MUST validate that passenger is authenticated before allowing pricing, payment, or order creation
- **FR-012**: System MUST log all payment transactions for audit and dispute resolution purposes

### Key Entities *(include if feature involves data)*

- **Order**: Represents a travel request with origin, destination, passenger, pricing details, payment status, and order status
- **Payment**: Represents a payment transaction with amount, status, payment method, transaction ID from payment provider, and linked order
- **Wallet**: Represents passenger's digital wallet in the payment system with wallet ID from payment provider and linked passenger account
- **Passenger**: The user requesting the travel service, includes personal information and linked wallet reference

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Passengers can receive travel price estimates in under 5 seconds
- **SC-002**: Payment processing completes in under 30 seconds for 95% of transactions
- **SC-003**: 98% of successfully processed payments result in order creation without errors
- **SC-004**: System successfully creates wallets for 99% of passengers who don't have one
- **SC-005**: Passengers can complete the full flow (price → payment → order) in under 3 minutes
- **SC-006**: Zero orders are created without corresponding successful payment
- **SC-007**: Payment failure rate remains below 5% for valid payment attempts

## Assumptions

- Passengers have stable internet connectivity during the pricing and payment flow
- Payment provider (Asaas) API is available and responds within acceptable timeframes
- Passenger personal information (name, email, CPF) is already collected and verified during account registration
- Order pricing is valid for a reasonable time period (e.g., 15 minutes) before requiring re-pricing
- The payment provider handles all regulatory compliance for financial transactions
- Drivers will be matched to orders through a separate process after order creation
- Refund handling for cancelled orders is out of scope for this initial implementation
- Mobile and web platforms will both use the same API endpoints for this flow
