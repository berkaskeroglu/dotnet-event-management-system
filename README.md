still in development

Next task: Complete the reservation system and refactor the connection handling architecture. Instead of opening a new connection for each action, establish and maintain major connections upfront to minimize repeated opening and closing of connections. This approach will also help reduce latency.

# dotnet-event-management-system
A robust and scalable Event Management System designed to handle seat reservations, dynamic pricing, and real-time updates using .NET 8.0 and PostgreSQL.


Key Features 

Reservation System:
Ensure smooth and conflict-free seat reservations with advanced seat-locking mechanisms to prevent double bookings.

Dynamic Pricing:
Adjust ticket prices dynamically based on seat availability thresholds for fair and optimized pricing.

Real-time Updates with SignalR:
Notify users about seat availability and pricing updates instantly through WebSocket-based real-time communication.

PostgreSQL Integration:
Utilize raw SQL queries for efficient database operations, avoiding the overhead of ORM tools.

Quartz.NET for Scheduling:
Schedule automated price updates every 20 minutes to keep the pricing model consistent and transparent.

Technology Stack 

Backend: .NET 8.0

Database: PostgreSQL

Real-time Communication: SignalR

Task Scheduling: Quartz.NET

API Design: RESTful services
