---
description: Create a new project and set up the basic structure
---

# Create Project Workflow

Follow this workflow when starting a new project.

## Execution Steps

### 1. Determine Project Name and Tech Stack

Based on the issue description, decide on the project name and the technology stack to use (e.g., C#, Python).

### 2. Create Project Directory

Create a new project folder within the `Projects` directory.

```powershell
mkdir c:\Workspace\AntiCorp\Projects\[ProjectName]
cd c:\Workspace\AntiCorp\Projects\[ProjectName]
```

### 3. Initialize Project

Initialize the project according to the chosen technology stack.

**For C# Projects:**
```powershell
dotnet new console -n [ProjectName]
# or for a solution
dotnet new sln -n [ProjectName]
dotnet new console -n [ProjectName].Core
dotnet sln add [ProjectName].Core
```

**For Web App (React/Next.js):**
```powershell
npx create-next-app@latest [ProjectName]
```

### 4. Git Initialization and Commit

Add the created project to Git and perform the initial commit.
(If Git is already initialized in the root, just add)

```powershell
git add .
git commit -m "feat: Initialize [ProjectName] project (Close #[IssueNumber])"
```

### 5. Report Results

Report the completion of project creation to the issue and close it.

```powershell
gh issue close [IssueNumber] --repo yj7-park/AntiCorp --comment "Project [ProjectName] initialization complete"
```
