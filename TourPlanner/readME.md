# Tour Planner Application
### Project Initialization
Welcome to the Tour Planner application! This document will guide you through the initial setup and configuration of the project.

### Prerequisites
- .NET Core SDK (version 7.0 or higher)
- Docker
### Setting Up the PostgreSQL Database 
This application needs a PostgreSQL database to store data.  

1. **Navigate to the Persistence Folder**:
   Open a terminal or command prompt and navigate to the `persistence\db` folder of the project.

2. **Run Docker Compose**:
   Execute the following command to start the PostgreSQL database using Docker Compose:
   ```bash
   docker-compose up

This command will initialize the PostgreSQL container as defined in the docker-compose.yml file. The included SQL script will be automatically processed to set up the necessary database schema and initial data.