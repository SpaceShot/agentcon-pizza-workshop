using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI.Responses;


var projectEndpoint = Environment.GetEnvironmentVariable("ProjectEndpoint");
//projectEndpoint = "https://cg-pizza-workshop-001.services.ai.azure.com";

// read instructions from instructions.txt file
string instructions = File.ReadAllText("instructions.txt");

AIProjectClient client = new (new Uri(projectEndpoint), new DefaultAzureCredential());

var agentVersion = await client.Agents.CreateAgentVersionAsync(
    agentName: "pizza-agent",
    options: new AgentVersionCreationOptions(
        new PromptAgentDefinition("gpt-4o")
        {
            Instructions = instructions,
            Tools = [new FileSearchTool([vectorStoreid])],

        }
    )
);
Console.WriteLine($"Created agent name: {agentVersion.Value.Name}. version: {agentVersion.Value.Version}");

ChatClientAgent agent = client.GetAIAgent("pizza-agent");
Console.WriteLine($"Created agent with ID: {agent.Id}");


string[] exitCommands = ["exit", "quit"];
while (true)
{
    Console.Write("You: ");
    string? userInput = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(userInput))
    {
        continue;
    }

    if (exitCommands.Contains(userInput.Trim().ToLower()))
    {
        break;
    }   

    AgentRunResponse response = await agent.RunAsync(userInput);
    Console.WriteLine($"Agent: {response}");
}