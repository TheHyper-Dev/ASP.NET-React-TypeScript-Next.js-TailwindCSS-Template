# MCP Server Architecture for Adesso Customer Assistance

## Overview
This document describes the architecture for implementing an MCP (Model Context Protocol) server within the existing ASP.NET 9 MinimalAPI application. The MCP server will provide customer assistance capabilities for Adesso, an AI tech company in Germany, with a focus on business thinking and selling products with discounts/deals.

## System Components

### 1. Existing Components
- **ASP.NET 9 MinimalAPI**: Core web application with product management endpoints
- **Product Model**: Represents products with ID, name, and price
- **ProductContext**: Entity Framework context for product data management
- **In-Memory Database**: For storing product information
- **Next.js Frontend**: React/TypeScript application for user interface

### 2. New MCP Components

#### MCP Protocol Implementation
- **MCP Endpoints**: New endpoints that implement the MCP protocol for customer assistance
- **MCP Controllers**: Handle MCP protocol requests and responses
- **MCP Service Layer**: Business logic for customer assistance and deal generation

#### Customer Assistance Models
- **CustomerQuery**: Represents customer inquiries about products or services
  - `query`: The text of the customer's query
  - `customerId`: Identifier for the customer (optional)
  - `context`: Additional context about the customer's session
- **AssistanceResponse**: Response to customer queries with recommendations
  - `responseText`: The main response text
  - `recommendations`: List of recommended products
  - `deals`: List of personalized deals
  - `confidence`: Confidence level of the response
- **Deal**: Personalized offers and discounts for customers
  - `productId`: The product this deal applies to
  - `discountPercentage`: Percentage discount offered
  - `originalPrice`: Original price of the product
  - `discountedPrice`: Price after discount
  - `validUntil`: Expiration date of the deal
  - `dealDescription`: Description of the deal
- **CustomerContext**: Context information about the customer for personalization
  - `customerId`: Unique identifier for the customer
  - `preferences`: Customer preferences and interests
  - `purchaseHistory`: History of previous purchases
  - `location`: Customer's location (for regional offers)
  - `language`: Preferred language for communication

#### Business Logic Components
- **DealService**: Generates personalized deals based on product data and customer context
  - `GenerateDeals(CustomerContext context)`: Creates personalized deals for a customer
  - `CalculateDiscount(Product product, CustomerContext context)`: Calculates discount percentage
  - `GetBestDeals(List<Product> products, CustomerContext context)`: Finds best deals from product list
- **RecommendationEngine**: Provides product recommendations based on customer queries
  - `GetRecommendations(CustomerQuery query, List<Product> products)`: Recommends products based on query
  - `GetSimilarProducts(Product product, List<Product> allProducts)`: Finds similar products
  - `GetTrendingProducts(List<Product> products)`: Identifies trending products
- **BusinessIntelligenceService**: Applies business thinking to customer interactions
  - `AnalyzeQuery(CustomerQuery query)`: Analyzes customer query for intent
  - `GenerateResponseText(CustomerQuery query, List<Product> recommendations, List<Deal> deals)`: Creates natural language response
  - `ApplyBusinessRules(CustomerContext context, List<Product> products)`: Applies business rules for deals
- **McpProtocolService**: Handles MCP protocol message formatting and parsing
  - `ParseMcpRequest(string json)`: Parses incoming MCP requests
  - `FormatMcpResponse(object result)`: Formats responses according to MCP protocol
  - `HandleMcpError(Exception ex)`: Formats error responses according to MCP protocol

## Integration Points

### With Existing Product System
- Access to ProductContext for retrieving product information
- Extension of existing API with MCP endpoints
- Shared models between traditional API and MCP endpoints

### With Frontend
- New API endpoints for customer assistance
- Integration with existing product display components
- Enhanced UI for showing personalized deals

## MCP Protocol Implementation Details

### Supported MCP Operations
1. **Text Completion**: Handle customer queries about products and services
2. **Tool Calling**: Execute business logic for deal generation and recommendations
3. **Resource Access**: Access product data and customer context

### MCP Endpoints
- `/mcp/chat`: Main endpoint for customer assistance conversations
  - POST: Accept customer queries and return assistance responses
  - Request body: CustomerQuery object
  - Response: AssistanceResponse object with recommendations and deals
- `/mcp/tools/deals`: Endpoint for generating personalized deals
  - POST: Generate deals based on customer context and product data
  - Request body: CustomerContext object
  - Response: List of Deal objects
- `/mcp/tools/recommendations`: Endpoint for product recommendations
  - POST: Generate product recommendations based on customer query
  - Request body: CustomerQuery object
  - Response: List of recommended Product objects
- `/mcp/resources/products`: Endpoint for accessing product data
  - GET: Retrieve all products
  - GET /{id}: Retrieve specific product by ID
  - Response: Product objects or list of Product objects
- `/mcp/resources/customer-context`: Endpoint for accessing customer context
  - POST: Store or update customer context
  - GET /{customerId}: Retrieve customer context by ID
  - Response: CustomerContext object

### MCP Protocol Message Format
The MCP endpoints will use JSON format for requests and responses, following the MCP protocol specifications:

```json
{
  "method": "method_name",
  "params": {
    "parameter1": "value1",
    "parameter2": "value2"
  }
}
```

Responses will follow the format:
```json
{
  "result": {
    "response_data": "data"
  }
}
```

Or for errors:
```json
{
  "error": {
    "code": 500,
    "message": "Error description"
  }
}
```

## Data Flow

1. **Customer Query Processing**:
   - Customer submits query through frontend
   - MCP controller receives and parses the request
   - Query is processed by assistance service
   - Response is generated with recommendations and deals

2. **Deal Generation**:
   - Customer context and query are analyzed
   - Relevant products are retrieved from ProductContext
   - Business rules are applied to generate personalized deals
   - Response is formatted according to MCP protocol

3. **Response Generation**:
   - Assistance response is created with product recommendations
   - Personalized deals are included in the response
   - Response is formatted according to MCP protocol specifications

## Business Logic for Personalized Deals

### Deal Generation Algorithm
The deal generation process follows these steps:

1. **Customer Analysis**:
   - Analyze customer context (preferences, purchase history, location)
   - Identify customer segments (new customer, returning customer, high-value customer)
   - Determine customer interests based on query content

2. **Product Matching**:
   - Match customer interests with available products
   - Consider product categories and relationships
   - Apply product popularity and seasonal factors

3. **Discount Calculation**:
   - Base discount: 5% for all customers
   - Loyalty bonus: Additional 5% for returning customers
   - Volume discount: Additional 3% for bulk purchases
   - Seasonal promotion: Additional 2-10% based on current promotions
   - Special offers: Additional 5-15% for targeted campaigns

4. **Deal Personalization**:
   - Adjust discounts based on customer value
   - Apply time-limited offers for urgency
   - Include bundle deals for complementary products
   - Consider customer's price sensitivity

### Business Rules
- Maximum discount: 30% for standard customers
- Maximum discount: 50% for high-value customers
- Minimum order value for deals: €50
- Deal validity: 7 days from generation
- One deal per product per customer per week
- Special anniversary deals for long-term customers

### Recommendation Logic
- Product similarity based on name and category matching
- Trending products based on recent sales data
- Complementary products based on common purchase patterns
- New arrivals for customers interested in latest products

## Security Considerations
- CORS configuration for frontend communication
- Input validation for customer queries
- Rate limiting for MCP endpoints
- Error handling and logging

## Deployment Considerations
- Integration with existing ASP.NET application
- Shared configuration settings
- Monitoring and logging integration
- Performance considerations for real-time assistance

## Future Enhancements
- Integration with external AI services for advanced natural language processing
- Machine learning models for better deal personalization
- Analytics for customer interaction insights
- Multi-language support for international customers