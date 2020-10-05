using Discord.WebSocket;
using GemBot.Models;
using GemBot.Repository;
using GemBot.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemBot.Events
{
    public class UserVoiceStateUpdate
    {
        private readonly UserRepository _userRepository;
        private readonly LogRepository _logRepository;
        private List<TaskLoggerManager> _listTaskLogger;

        public UserVoiceStateUpdate(LogRepository logRepository, UserRepository userRepository, List<TaskLoggerManager> listTaskLogger)
        {
            _userRepository = userRepository;
            _logRepository = logRepository;
            _listTaskLogger = listTaskLogger;
        }

        public async Task UserJoinedChannel(SocketUser socketUser, SocketVoiceState beforeChannel, SocketVoiceState afterChannel)
        {
            if (socketUser.IsBot)
                return;

            try
            {
                bool exiting = false, persist = true;
                User snapshotUser = await _userRepository.GetUser(socketUser.Id);

                // Saiu do discord que tem o bot
                if (afterChannel.ToString().Contains("Unknown") || (afterChannel.VoiceChannel.Guild.AFKChannel != null && afterChannel.VoiceChannel.Guild.AFKChannel.Name.Equals(afterChannel.VoiceChannel.Name)))
                    exiting = true;
                else // Trocou de channel
                    if (beforeChannel.ToString().Contains("Unknown")) // Entrou no discord
                        exiting = false;
                    else // Trocou de channel na mesma guild
                        persist = false;                  

                if (persist)
                    await _userRepository.UserCreateOrUpdate(new User(socketUser, snapshotUser, exiting));
            }
            catch (Exception e)
            {

                await _logRepository.InsertLogError("UserVoiceStateUpdate", "UserJoinedChannel", e.Message);
            }            
        }

    }
}
