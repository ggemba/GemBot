using Discord.WebSocket;
using GemBot.Models;
using GemBot.Repository;
using GemBot.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemBot.Events
{
    public class GuildMemberUpdated
    {
        private readonly UserRepository _userRepository;
        private readonly LogRepository _logRepository;
        private List<TaskLoggerManager> _taskLogger;

        public GuildMemberUpdated(LogRepository logRepository, UserRepository userRepository, List<TaskLoggerManager> taskLogger)
        {
            _userRepository = userRepository;
            _logRepository = logRepository;
            _taskLogger = taskLogger;
        }

        public async Task UserUpdatePresence(SocketGuildUser beforeUserGuild, SocketGuildUser afterUserGuild)
        {
            if (afterUserGuild.IsBot)
                return;

            TaskLoggerManager task = Util.TaskLoggerManager.CreateTask(afterUserGuild.Id, "UserUpdatePresence");

            try
            {
                if (afterUserGuild.VoiceChannel != null)
                {
                    bool exitingGame = false, persist = true, afterUser = false;

                    if (_taskLogger.Contains(task) && _taskLogger.Find(x => x.DateStart < DateTime.Now.AddSeconds(5)) != null)
                        return;
                    else
                        _taskLogger.Add(task);
                    
                    if (afterUserGuild.Activity != null) // Entrou o jogo
                        persist = afterUser = true;
                    else // Saiu do jogo
                        if (beforeUserGuild.Activity != null)
                            exitingGame = persist = true;

                    if (persist)
                    {
                        User snapshotUser = await _userRepository.GetUser(afterUser ? afterUserGuild.Id : beforeUserGuild.Id);
                        await _userRepository.UserCreateOrUpdate(new User(afterUser ? afterUserGuild : beforeUserGuild, snapshotUser, beforeUserGuild.Activity != null && !beforeUserGuild.Activity.Equals(afterUserGuild.Activity) ? true : exitingGame));
                    }                    
                }
            }
            catch (Exception e)
            {
                await _logRepository.InsertLogError("GuildMemberUpdated", "UserUpdatePresence", e.Message);
            }
            finally
            {
                if (_taskLogger.Contains(task))
                    _taskLogger.Remove(task);
            }
        }
    }
}
