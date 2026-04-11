var tagToolAssembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "TagTool");
var version = tagToolAssembly.GetName().Version;
ContextStack.Context.GetCommand("SetVariable").Execute(new List<string> { "TagToolVersion", version.ToString() });