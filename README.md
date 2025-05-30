# Project Management Toolkit

A .NET-based project-planning library and console application that lets you:

- **Import** your work-breakdown structure (WBS) from Markdown files  
- **Model** tasks as a hybrid WBS + directed-acyclic graph (DAG) with dependencies, descriptions and tags  
- **Export** to multiple formats (Markdown tables, PlantUML WBS & Gantt, GraphViz DOT, Mermaid, PlantUML dependency diagrams)  
- **Analyze** critical paths  

---

## Features

- **Flexible WBS import**  
  - Heading levels (`#` → root, `##` → children, …) become task hierarchy  
  - Hierarchical IDs auto-generated (`1`, `1.1`, `1.1.1`, …) or picked up from numeric prefixes  
  - Multiline task descriptions from paragraphs under headings

- **Inline markers**  
  - `@(X.Y…)` in text → dependency on task with ID `X.Y…`  
  - `#tagName` anywhere in text → adds a tag to the current task  

- **Unified in-memory model**  
  - `TaskNode` objects hold `Id`, `Name`, `Description`, `Tags`, `Children` (WBS), `Dependencies` (DAG)  
  - `ProjectGraph` manages tasks, parent-child hierarchy, dependency cycle-checks, topological sort  

- **Visitor-based exporters**  
  - **`MarkdownTableVisitor`** → plain Markdown task table  
  - **`PlantUmlWbsVisitor`** → `@startwbs`…`@endwbs` WBS diagram  
  - **`PlantUmlGanttVisitor`** → `@startgantt`… Gantt chart  
  - **`DotVisitor`** → GraphViz `.dot` directed graph  
  - **`MermaidVisitor`** → `graph TD` for Mermaid.js  
  - **`PlantUmlDependencyVisitor`** → `@startuml`… dependency diagram  
  - **`CriticalPathVisitor`** → runs CP analysis and emits a Markdown report  

- **Analysis**  
  - **Critical-path analysis**: earliest/latest starts and finishes, slack, zero-slack path  

---

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download) or later  

### Installation

```bash
git clone https://github.com/your-org/project-management-toolkit.git
cd project-management-toolkit
dotnet restore
dotnet build
