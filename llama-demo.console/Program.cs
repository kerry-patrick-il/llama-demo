using LLama.Common;
using LLama;

string modelPath = "C:\\git\\edu\\ai\\models\\llama-2-7b-guanaco-qlora.Q6_K.gguf"; // change it to your own model path
//var prompt = "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.\r\n\r\nUser: Hello, Bob.\r\nBob: Hello. How may I help you today?\r\nUser: Please tell me the largest city in Europe.\r\nBob: Sure. The largest city in Europe is Moscow, the capital of Russia.\r\nUser:"; // use the "chat-with-bob" prompt here.
//var prompt = """
//             You are Homer Simpson, and respond to User with funny Homer Simpson-like comments.
             
//             User:
//             """; // Maarten Balliauw 1
var prompt = """
             You are a polite and helpful pair programming assistant.
             You MUST reply in a polite and helpful manner.
             When asked for your name, you MUST reply that your name is 'LocalLLM'.
             You MUST use Markdown formatting in your replies when the content is a block of code.
             You MUST include the programming language name in any Markdown code blocks.
             Your code responses MUST be using C# language syntax.
             
             User:
             """; // Maarten Balliauw 1


// Load model
var parameters = new ModelParams(modelPath)
{
    ContextSize = 2048
};
using var model = LLamaWeights.LoadFromFile(parameters);

// Initialize a chat session
using var context = model.CreateContext(parameters);
var ex = new InteractiveExecutor(context);
ChatSession session = new ChatSession(ex);

// show the prompt
Console.WriteLine();
Console.Write(prompt);

// run the inference in a loop to chat with LLM
while (true)
{
    foreach (var text in session.Chat(prompt, new InferenceParams() { Temperature = 0.1f, AntiPrompts = new List<string> { "User:" } }))
    {
        Console.Write(text);
    }

    Console.ForegroundColor = ConsoleColor.Green;
    prompt = Console.ReadLine();
    Console.ForegroundColor = ConsoleColor.White;
}