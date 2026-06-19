using System;
using System.Linq;
using System.Reflection;
using TagTool.Common.Logging;
using TagTool.Commands.Common;

var logType = typeof(Log);
var handlersField = logType.GetField("_handlers", BindingFlags.NonPublic | BindingFlags.Static);
var handlers = handlersField.GetValue(null);
var handlersEnumerable = (System.Collections.Generic.IEnumerable<ILogHandler>)handlers;

foreach (var handler in handlersEnumerable.Where(h => h.GetType().Name == "RunMetricsLogHandler").ToList())
{
    Log.RemoveHandler(handler);
}

foreach (var handler in handlersEnumerable.Where(h => h.GetType().Name == "ConsoleLogHandler").ToList())
{
    Log.RemoveHandler(handler);
}

Console.WriteLine("Error count paused");
