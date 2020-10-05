using Discord.Commands;
using GemBot.Models;
using GemBot.Repository;
using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;

namespace GemBot.Commands
{
    public class UserCommands : ModuleBase<SocketCommandContext>
    {
        private readonly UserRepository _userRepository;

        public UserCommands(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Command("testResp")]
        public async Task Teste()
        {
            await ReplyAsync($"Funfando! {Util.Date.GetResponseTimeStr(Context.Message.CreatedAt)} as {Timestamp.GetCurrentTimestamp().ToDateTime().ToLocalTime()}");
        }

        [Command("getuser")]
        public async Task GetXp()
        {
            try
            {
                User user = await _userRepository.GetUser(Context.User.Id);
                DateTimeOffset a = user.Guilds[Context.Guild.Id.ToString()].JoinedChannel.ToDateTime().ToLocalTime();
                await ReplyAsync($"{Context.User.Mention} Usuário: {user.UserName} " +
                                 $"você entrou em uma sala as {a} ");
            }
            catch (System.Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }
    }
}
