using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GemBot.Events;
using GemBot.Models;
using GemBot.Repository;
using GemBot.Util;
using Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace GemBot
{
    class Program
    {        
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        #region Declare variables default bot
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private SettingsRepository _settingsRepository;
        private Settings _settings;
        #endregion

        public async Task RunBotAsync()
        {
            #region standard instances
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            FirestoreDb _db = FirestoreDb.Create("gembot-7559e");
            _settingsRepository = new SettingsRepository(_db);
            _settings = await _settingsRepository.GetSettingsBot();
            #endregion

            UserRepository _userRepository = new UserRepository(_db);
            LogRepository _logRepository = new LogRepository(_db);
            List<TaskLoggerManager> listTaskLogger = new List<TaskLoggerManager>();
            UserVoiceStateUpdate _UserVoiceStateUpdate = new UserVoiceStateUpdate(_logRepository, _userRepository, listTaskLogger);
            GuildMemberUpdated _GuildMemberUpdated = new GuildMemberUpdated(_logRepository, _userRepository, listTaskLogger);

            _services = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton(_settings)
            .AddSingleton(_userRepository)
            .AddSingleton(_logRepository)
            .BuildServiceProvider();

            _client.Log += Log;
            _client.UserVoiceStateUpdated += _UserVoiceStateUpdate.UserJoinedChannel;
            _client.GuildMemberUpdated += _GuildMemberUpdated.UserUpdatePresence;

            #region Starting Bot  
            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, _settings.Token);

            await _client.StartAsync();
            await Task.Delay(-1);
            #endregion
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        #region Command Register
        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message is null || message.Author.IsBot)
                return;
            int argPos = 0;

            if (message.HasStringPrefix(_settings.Prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess)
                    System.Console.WriteLine(result.ErrorReason);
            }
        }
        #endregion
    }
}
