# GitCommit Dating Application

A dating application with client-server architecture built using .NET technologies.

## Project Structure

- **GitCommit.Client**: WPF client application for users to swipe on profiles
- **GitCommit.Server**: ASP.NET Core Web API server for handling data processing, matchmaking, and user management
- **GitCommit.ServerAdmin**: WPF admin panel for monitoring users, viewing swipe history, and managing user states
- **GitCommit.Shared**: Shared library containing models, interfaces, and utilities used by both client and server

## Features

### Client Application
- User login and registration
- Profile viewing and swiping (like/dislike)
- Match notifications
- Profile management
- Preference settings

### Server Application
- User authentication
- Profile management
- Real-time matchmaking
- User status tracking
- Profile image transfer

### Admin Panel
- User management
- Match history viewing
- System logs
- Server configuration

## Requirements

- .NET 9.0 SDK
- Visual Studio 2022 or Visual Studio Code

## How to Run

### Server
1. Navigate to the GitCommit.Server directory
2. Run the following command:
   ```
   dotnet run
   ```
3. The server will start on https://localhost:7001

### Client
1. Navigate to the GitCommit.Client directory
2. Run the following command:
   ```
   dotnet run
   ```
3. The client application will start and connect to the server

### Admin Panel
1. Navigate to the GitCommit.ServerAdmin directory
2. Run the following command:
   ```
   dotnet run
   ```
3. The admin panel will start and connect to the server

## Default Credentials

For testing purposes, you can use the following credentials:

- Username: user1
- Password: password

## Project Requirements Met

- REQ-SYS-001: The system contains two applications (Client and Server)
- REQ-SYS-010: The software is designed using Object-Oriented Principles
- REQ-SYS-020: All data transferred between Client and Server uses a predefined structure (JSON)
- REQ-SYS-030: The data packet structure contains dynamically allocated elements (Lists, Arrays)
- REQ-SYS-040: The two applications have different user interfaces
- REQ-SYS-050: All applications log all Transmitted and Received data packets
- REQ-SYS-060: The server application contains an operational state machine for user status
- REQ-SYS-070: The system has commands that initiate large data transfers (profile images)
- REQ-SYS-080: The server requires user authentication before accepting commands
