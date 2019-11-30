using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApi.KafkaMock
{
    public class App
    {
        private readonly CommandFileReader _commandFileReader;
        private readonly CommandSender _commandSender;
        private readonly PostSaver _postSaver;
        private readonly ILogger<App> _logger;

        public App(
            CommandFileReader commandFileReader,
            CommandSender commandSender,
            PostSaver postSaver,
            ILogger<App> logger)
        {
            _commandFileReader = commandFileReader;
            _commandSender = commandSender;
            _postSaver = postSaver;
            _logger = logger;
        }

        public async Task DoAsync(
            string commandFilePath,
            string configTopic,
            DirectoryInfo outputDirectory)
        {
            _logger.LogInformation("Reading commands from {file}", commandFilePath);
            var commands = await _commandFileReader.ReadCommandsAsync(commandFilePath);

            _logger.LogInformation("Started listening to '{configTopic}'", configTopic);
            _logger.LogInformation("Output saved to '{output}'", outputDirectory.FullName);
            var listenTasks = commands.Select(r =>

                Task.Run(() => _postSaver.ListenAndSaveAsync(
                    r.AnalysisOutputChannel ?? r.OutputMessageBrokerChannels[0],
                    outputDirectory,
                    CancellationToken.None)))
                .ToList();

            var delay = TimeSpan.FromSeconds(10);
            var commandList = commands.ToList();
            _logger.LogInformation("Sending {count} commands with delay ", commandList.Count, delay);
            await _commandSender.SendCommands(configTopic, commands, delay);

            _logger.LogInformation("Waiting for data to arrive");
            await Task.WhenAll(listenTasks);
        }
    }
}
