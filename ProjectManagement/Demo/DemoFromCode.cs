﻿using ProjectManagement.Core;
using ProjectManagement.Renderer;

namespace ProjectManagement.Demo;

public class DemoFromCode
{
    public static void Execute()
    {
        Console.WriteLine("Project Management Application - Code Example");
        ProjectGraph project = new();
        ExampleData(project);
        Console.WriteLine("--- Markdown Table ---" + Environment.NewLine + project.Export(new MarkdownTableVisitor()));
        Console.WriteLine("--- PlantUML WBS ---" + Environment.NewLine + project.Export(new PlantUmlWbsVisitor()));
        Console.WriteLine("--- PlantUML Gantt ---" + Environment.NewLine + project.ExportTopologicalSort(new PlantUmlGanttVisitor()));
        Console.WriteLine("--- DOT ---" + Environment.NewLine + project.Export(new DotVisitor()));
        Console.WriteLine("--- Mermaid ---" + Environment.NewLine + project.Export(new MermaidVisitor()));
        Console.WriteLine("--- PlantUML Dep ---" + Environment.NewLine + project.Export(new PlantUmlDependencyVisitor()));
    }

    private static void ExampleData(ProjectGraph project)
    {
        // 1. Mission Definition
        var t1 = project.AddTask(
            "1",
            "Mission Definition",
            "Define the overall objectives and scope of the lunar mission. " +
            "This includes defining mission phases, crew responsibilities, and timelines."
        );
        t1.AddTag("mission");
        t1.AddTag("planning");

        // 1.1 Requirements Definition
        var t1_1 = project.AddTask(
            "1.1",
            "Requirements Definition",
            "Gather and formalize mission requirements. " +
            "Ensure that all technical and safety requirements are captured. " +
            "Document constraints and interfaces with ground systems."
        );
        t1_1.AddTag("requirements");
        project.AddChild(t1.Id, t1_1.Id);

        // 1.1.1 Stakeholder Interviews
        var t1_1_1 = project.AddTask(
            "1.1.1",
            "Stakeholder Interviews",
            "Engage with key stakeholders to collect mission needs."
        );
        t1_1_1.AddTag("communication");
        project.AddChild(t1_1.Id, t1_1_1.Id);

        // 1.1.1.1 Prepare Interview Plan
        var t1_1_1_1 = project.AddTask(
            "1.1.1.1",
            "Prepare Interview Plan",
            "Plan stakeholder meeting logistics and prepare questionnaires."
        );
        project.AddChild(t1_1_1.Id, t1_1_1_1.Id);

        // 1.1.1.2 Conduct Interviews
        var t1_1_1_2 = project.AddTask(
            "1.1.1.2",
            "Conduct Interviews",
            "Conduct interviews with stakeholders to collect requirements."
        );
        t1_1_1_2.AddTag("critical");
        project.AddChild(t1_1_1.Id, t1_1_1_2.Id);
        project.AddDependency("1.1.1.1", "1.1.1.2");

        // 1.1.2 Requirement Documentation
        var t1_1_2 = project.AddTask(
            "1.1.2",
            "Requirement Documentation",
            "Compile all requirements into a formal document."
        );
        project.AddChild(t1_1.Id, t1_1_2.Id);

        // 1.1.2.1 Draft Requirements Document
        var t1_1_2_1 = project.AddTask(
            "1.1.2.1",
            "Draft Requirements Document",
            "Draft the requirements document based on interview notes."
        );
        project.AddChild(t1_1_2.Id, t1_1_2_1.Id);
        project.AddDependency("1.1.1.2", "1.1.2.1");

        // 1.1.3 Requirements Review
        var t1_1_3 = project.AddTask(
            "1.1.3",
            "Requirements Review",
            "Review and validate the documented requirements.\n\n" +
            "Facilitate stakeholder sign-off and capture any follow-up actions."
        );
        t1_1_3.AddTag("quality");
        project.AddChild(t1_1.Id, t1_1_3.Id);

        // 1.1.3.1 Host Review Workshop
        var t1_1_3_1 = project.AddTask(
            "1.1.3.1",
            "Host Review Workshop",
            "Facilitate workshop to review requirements."
        );
        project.AddChild(t1_1_3.Id, t1_1_3_1.Id);
        project.AddDependency("1.1.2.1", "1.1.3.1");

        // 1.2 Feasibility Study
        var t1_2 = project.AddTask(
            "1.2",
            "Feasibility Study",
            "Analyze feasibility: technical, budgetary, and risk.\n\n" +
            "Break the study into:\n" +
            "- Technical feasibility\n" +
            "- Cost feasibility\n" +
            "- Risk feasibility"
        );
        t1_2.AddTag("feasibility");
        project.AddChild(t1.Id, t1_2.Id);

        // 1.2.1 Budget Analysis
        var t1_2_1 = project.AddTask(
            "1.2.1",
            "Budget Analysis",
            "Estimate costs associated with mission development.\n\n" +
            "Include launch vehicle, spacecraft systems, and ground support."
        );
        t1_2_1.AddTag("budget");
        project.AddChild(t1_2.Id, t1_2_1.Id);

        // 1.2.1.1 Estimate Budget
        var t1_2_1_1 = project.AddTask(
            "1.2.1.1",
            "Estimate Budget",
            "Estimate overall mission budget considering all phases."
        );
        project.AddChild(t1_2_1.Id, t1_2_1_1.Id);

        // 1.2.2 Risk Assessment
        var t1_2_2 = project.AddTask(
            "1.2.2",
            "Risk Assessment",
            "Identify and assess mission risks."
        );
        t1_2_2.AddTag("risk");
        project.AddChild(t1_2.Id, t1_2_2.Id);

        // 1.2.2.1 Identify Risks
        var t1_2_2_1 = project.AddTask(
            "1.2.2.1",
            "Identify Risks",
            "Identify potential mission risks considering budget constraints."
        );
        project.AddChild(t1_2_2.Id, t1_2_2_1.Id);
        project.AddDependency("1.2.1.1", "1.2.2.1");

        // 2. System Design & Development
        var t2 = project.AddTask(
            "2",
            "System Design & Development",
            "Define and build all systems needed for the mission."
        );
        t2.AddTag("development");

        // 2.1 Architectural Design
        var t2_1 = project.AddTask(
            "2.1",
            "Architectural Design",
            "Design the high-level architecture of spacecraft and ground systems."
        );
        project.AddChild(t2.Id, t2_1.Id);

        // 2.1.1 Hardware Architecture
        var t2_1_1 = project.AddTask(
            "2.1.1",
            "Hardware Architecture",
            "Define the hardware components and interfaces.\n\n" +
            "Focus on reliability and redundancy."
        );
        t2_1_1.AddTag("hardware");
        project.AddChild(t2_1.Id, t2_1_1.Id);

        // 2.1.1.1 Define Hardware Specifications
        var t2_1_1_1 = project.AddTask(
            "2.1.1.1",
            "Define Hardware Specifications",
            "Define hardware specifications according to mission needs."
        );
        project.AddChild(t2_1_1.Id, t2_1_1_1.Id);

        // 2.1.2 Software Architecture
        var t2_1_2 = project.AddTask(
            "2.1.2",
            "Software Architecture",
            "Design software systems to control the mission."
        );
        project.AddChild(t2_1.Id, t2_1_2.Id);

        // 2.1.2.1 Define Software Specifications
        var t2_1_2_1 = project.AddTask(
            "2.1.2.1",
            "Define Software Specifications",
            "Define software specifications guided by hardware constraints."
        );
        project.AddChild(t2_1_2.Id, t2_1_2_1.Id);
        project.AddDependency("2.1.1.1", "2.1.2.1");

        // 2.2 Subsystem Development
        var t2_2 = project.AddTask(
            "2.2",
            "Subsystem Development",
            "Develop individual subsystems for the mission."
        );
        project.AddChild(t2.Id, t2_2.Id);

        // 2.2.1 Propulsion Development
        var t2_2_1 = project.AddTask(
            "2.2.1",
            "Propulsion Development",
            "Create and test the propulsion system.\n\n" +
            "This includes fuel tank design, feed lines, and thruster mounting."
        );
        project.AddChild(t2_2.Id, t2_2_1.Id);

        // 2.2.1.1 Develop Thruster Prototype
        var t2_2_1_1 = project.AddTask(
            "2.2.1.1",
            "Develop Thruster Prototype",
            "Develop a prototype thruster for initial testing."
        );
        project.AddChild(t2_2_1.Id, t2_2_1_1.Id);

        // 2.2.1.2 Test Thruster Prototype
        var t2_2_1_2 = project.AddTask(
            "2.2.1.2",
            "Test Thruster Prototype",
            "Test the developed thruster prototype for performance and safety."
        );
        project.AddChild(t2_2_1.Id, t2_2_1_2.Id);
        project.AddDependency("2.2.1.1", "2.2.1.2");

        // 2.2.2 Navigation Development
        var t2_2_2 = project.AddTask(
            "2.2.2",
            "Navigation Development",
            "Implement navigation and guidance software."
        );
        t2_2_2.AddTag("navigation");
        project.AddChild(t2_2.Id, t2_2_2.Id);

        // 2.2.2.1 Develop Navigation Algorithm
        var t2_2_2_1 = project.AddTask(
            "2.2.2.1",
            "Develop Navigation Algorithm",
            "Develop the navigation algorithm for lunar trajectory control."
        );
        project.AddChild(t2_2_2.Id, t2_2_2_1.Id);

        // 2.2.2.2 Validate Navigation Algorithm
        var t2_2_2_2 = project.AddTask(
            "2.2.2.2",
            "Validate Navigation Algorithm",
            "Validate navigation algorithm against simulation scenarios."
        );
        project.AddChild(t2_2_2.Id, t2_2_2_2.Id);
        project.AddDependency("2.2.2.1", "2.2.2.2");

        // 2.2.3 Communication Development
        var t2_2_3 = project.AddTask(
            "2.2.3",
            "Communication Development",
            "Set up communication systems between spacecraft and ground."
        );
        project.AddChild(t2_2.Id, t2_2_3.Id);

        // 2.2.3.1 Set Up Communication Module
        var t2_2_3_1 = project.AddTask(
            "2.2.3.1",
            "Set Up Communication Module",
            "Set up the communication module and ensure integration."
        );
        project.AddChild(t2_2_3.Id, t2_2_3_1.Id);
        project.AddDependency("2.2.2.2", "2.2.3.1");

        // 2.3 Testing
        var t2_3 = project.AddTask(
            "2.3",
            "Testing",
            "Plan and execute system-level testing."
        );
        t2_3.AddTag("testing");
        project.AddChild(t2.Id, t2_3.Id);

        // 2.3.1 Integration Testing
        var t2_3_1 = project.AddTask(
            "2.3.1",
            "Integration Testing",
            "Plan and prepare for integration testing."
        );
        project.AddChild(t2_3.Id, t2_3_1.Id);

        // 2.3.1.1 Plan Integration Tests
        var t2_3_1_1 = project.AddTask(
            "2.3.1.1",
            "Plan Integration Tests",
            "Plan comprehensive integration tests."
        );
        project.AddChild(t2_3_1.Id, t2_3_1_1.Id);
        project.AddDependency("2.2.3.1", "2.3.1.1");

        // 2.3.2 Validation Testing
        var t2_3_2 = project.AddTask(
            "2.3.2",
            "Validation Testing",
            "Perform final validation tests of the integrated system."
        );
        project.AddChild(t2_3.Id, t2_3_2.Id);

        // 2.3.2.1 Execute Validation Tests
        var t2_3_2_1 = project.AddTask(
            "2.3.2.1",
            "Execute Validation Tests",
            "Execute validation tests according to plan."
        );
        project.AddChild(t2_3_2.Id, t2_3_2_1.Id);
        project.AddDependency("2.3.1.1", "2.3.2.1");

        // 3. Launch Preparation
        var t3 = project.AddTask(
            "3",
            "Launch Preparation",
            "Prepare facilities, crew, and logistics for launch."
        );
        t3.AddTag("launch");

        // 3.1 Facility Preparation
        var t3_1 = project.AddTask(
            "3.1",
            "Facility Preparation",
            "Ready launch and ground support facilities. Coordinate with range safety and logistics teams."
        );
        project.AddChild(t3.Id, t3_1.Id);

        // 3.1.1 Launch Pad Retrofit
        var t3_1_1 = project.AddTask(
            "3.1.1",
            "Launch Pad Retrofit",
            "Retrofit the launch pad to support lunar mission hardware."
        );
        project.AddChild(t3_1.Id, t3_1_1.Id);
        project.AddDependency("2.1.1.1", "3.1.1");

        // 3.2 Crew Training
        var t3_2 = project.AddTask(
            "3.2",
            "Crew Training",
            "Train the crew in all mission operations."
        );
        t3_2.AddTag("training");
        project.AddChild(t3.Id, t3_2.Id);

        // 3.2.1 Astronaut Training
        var t3_2_1 = project.AddTask(
            "3.2.1",
            "Astronaut Training",
            "Conduct general astronaut training sessions. Cover EVA, docking procedures, and emergency protocols."
        );
        project.AddChild(t3_2.Id, t3_2_1.Id);

        // 3.2.1.1 Conduct Medical Training
        var t3_2_1_1 = project.AddTask(
            "3.2.1.1",
            "Conduct Medical Training",
            "Conduct medical training for crew health management."
        );
        project.AddChild(t3_2_1.Id, t3_2_1_1.Id);

        // 3.2.1.2 Conduct EVA Training
        var t3_2_1_2 = project.AddTask(
            "3.2.1.2",
            "Conduct EVA Training",
            "Conduct extravehicular activity (EVA) training."
        );
        project.AddChild(t3_2_1.Id, t3_2_1_2.Id);
        project.AddDependency("3.2.1.1", "3.2.1.2");

        // 3.2.2 Simulation Drills
        var t3_2_2 = project.AddTask(
            "3.2.2",
            "Simulation Drills",
            "Perform simulation drills for mission scenarios."
        );
        project.AddChild(t3_2.Id, t3_2_2.Id);

        // 3.2.2.1 Run Launch Simulation
        var t3_2_2_1 = project.AddTask(
            "3.2.2.1",
            "Run Launch Simulation",
            "Run full launch simulations in simulators."
        );
        t3_2_2_1.AddTag("simulation");
        project.AddChild(t3_2_2.Id, t3_2_2_1.Id);

        // 3.2.2.2 Run Abort Simulation
        var t3_2_2_2 = project.AddTask(
            "3.2.2.2",
            "Run Abort Simulation",
            "Run abort and contingency simulations."
        );
        t3_2_2_2.AddTag("contingency");
        project.AddChild(t3_2_2.Id, t3_2_2_2.Id);
        project.AddDependency("3.2.2.1", "3.2.2.2");

        // 4. Mission Operations
        var t4 = project.AddTask(
            "4",
            "Mission Operations",
            "Manage mission operations once the spacecraft is in lunar orbit."
        );
        t4.AddTag("operations");

        // 4.1 Orbit Operations
        var t4_1 = project.AddTask(
            "4.1",
            "Orbit Operations",
            "Monitor spacecraft during the lunar orbit phase."
        );
        project.AddChild(t4.Id, t4_1.Id);

        // 4.1.1 Telemetry Monitoring
        var t4_1_1 = project.AddTask(
            "4.1.1",
            "Telemetry Monitoring",
            "Set up and monitor telemetry data."
        );
        t4_1_1.AddTag("monitoring");
        project.AddChild(t4_1.Id, t4_1_1.Id);

        // 4.1.1.1 Configure Telemetry Network
        var t4_1_1_1 = project.AddTask(
            "4.1.1.1",
            "Configure Telemetry Network",
            "Configure the ground telemetry network and feeds."
        );
        project.AddChild(t4_1_1.Id, t4_1_1_1.Id);

        // 4.1.1.2 Analyze Flight Data
        var t4_1_1_2 = project.AddTask(
            "4.1.1.2",
            "Analyze Flight Data",
            "Analyze live flight data to assess mission health."
        );
        project.AddChild(t4_1_1.Id, t4_1_1_2.Id);
        project.AddDependency("4.1.1.1", "4.1.1.2");
    }
}