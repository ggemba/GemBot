using Discord.WebSocket;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace GemBot.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreDocumentId]
        public string UserId { get; set; }
        [FirestoreProperty("UserName")]
        public string UserName { get; set; }
        [FirestoreProperty("Discriminator")]
        public string Discriminator { get; set; }
        [FirestoreProperty("Activities")]
        public Dictionary<string, Activity> Activities { get; set; }
        [FirestoreProperty("Guilds")]
        public Dictionary<string, Guild> Guilds { get; set; }

        public User()
        {

        }

        public User(SocketUser socketUser, User snapshotUser, bool exiting)
        {
            string guildId = ((SocketGuildUser)socketUser).Guild.Id.ToString();

            UserId = socketUser.Id.ToString();
            UserName = socketUser.Username;
            Discriminator = socketUser.Discriminator;

            PrepareActivities(null, socketUser, snapshotUser, exiting);
            PrepareGuilds(socketUser, snapshotUser, exiting, guildId);
        }

        public User(SocketGuildUser socketGuildUser, User snapshotUser, bool exitingGame)
        {
            UserId = socketGuildUser.Id.ToString();
            UserName = socketGuildUser.Username;
            Discriminator = socketGuildUser.Discriminator;

            PrepareActivities(socketGuildUser, null, snapshotUser, exitingGame);
            PrepareGuilds(null, snapshotUser, exitingGame, null);
        }

        private void PrepareActivities(SocketGuildUser? socketGuildUser, SocketUser? socketUser, User snapshotUser, bool exitingGame)
        {
            string activity = socketGuildUser != null && socketGuildUser.Activity != null ? socketGuildUser.Activity.ToString()
                                                                                          : socketUser != null && socketUser.Activity != null ? socketUser.Activity.ToString() : String.Empty;

            if (snapshotUser != null && snapshotUser.Activities != null)
                Activities = snapshotUser.Activities;

            if (String.IsNullOrEmpty(activity))
                return;

            if (!String.IsNullOrEmpty(activity))
                if (snapshotUser != null && snapshotUser.Activities != null)
                {
                    Activities = snapshotUser.Activities;

                    if (Activities.ContainsKey(activity))
                    {
                        Activities[activity].Minutes = SumCurrentExperienceActivity(snapshotUser, activity, exitingGame);

                        if (!exitingGame)
                            Activities[activity].JoinGame = Timestamp.GetCurrentTimestamp();
                    }
                    else
                        Activities.Add(activity, new Activity() { JoinGame = Timestamp.GetCurrentTimestamp(), Minutes = SumCurrentExperienceActivity(snapshotUser, activity, exitingGame) });
                }
                else
                    Activities = new Dictionary<string, Activity>() { { activity, new Activity() { JoinGame = Timestamp.GetCurrentTimestamp(), Minutes = SumCurrentExperienceActivity(snapshotUser, activity, exitingGame) } } };
        }

        private void PrepareGuilds(SocketUser? socketUser, User snapshotUser, bool exiting, string? guildId)
        {
            if (snapshotUser != null && String.IsNullOrEmpty(guildId))
            {
                Guilds = snapshotUser.Guilds;
                return;
            }

            if (snapshotUser != null && snapshotUser.Guilds != null)
            {
                Guilds = snapshotUser.Guilds;

                if (Guilds.ContainsKey(guildId))
                {
                    Guilds[guildId].GuildName = ((SocketGuildUser)socketUser).Guild.Name;
                    Guilds[guildId].NickName = ((SocketGuildUser)socketUser).Nickname;
                    Guilds[guildId].Expirence = SumCurrentExperienceGuild(snapshotUser == null ? new User() : snapshotUser, guildId, exiting);
                    if (!exiting)
                        Guilds[guildId].JoinedChannel = Timestamp.GetCurrentTimestamp();
                }
                else
                    Guilds.Add(guildId, new Guild() { GuildName = ((SocketGuildUser)socketUser).Guild.Name, NickName = ((SocketGuildUser)socketUser).Nickname, JoinedChannel = Timestamp.GetCurrentTimestamp(), Expirence = SumCurrentExperienceGuild(snapshotUser == null ? new User() : snapshotUser, guildId, exiting) });
            }
            else
                Guilds = new Dictionary<string, Guild>() { { guildId, new Guild() { GuildName = ((SocketGuildUser)socketUser).Guild.Name, NickName = ((SocketGuildUser)socketUser).Nickname, JoinedChannel = Timestamp.GetCurrentTimestamp(), Expirence = SumCurrentExperienceGuild(snapshotUser, guildId, exiting) } } };
        }

        private int SumCurrentExperienceGuild(User? snapshotUser, string guildId, bool exiting)
        {
            int currentExperience = 0, minutes;

            if (snapshotUser != null)
                if (snapshotUser.Guilds != null && snapshotUser.Guilds.ContainsKey(guildId))
                    if (exiting)
                    {
                        currentExperience = snapshotUser.Guilds[guildId].Expirence;
                        minutes = Util.Date.GetMinutesBetweenDates(DateTime.Now, snapshotUser.Guilds[guildId].JoinedChannel.ToDateTime().ToLocalTime());
                        currentExperience += minutes * 10;
                    }
                    else
                        currentExperience = snapshotUser.Guilds[guildId].Expirence;

            return currentExperience;
        }

        private int SumCurrentExperienceActivity(User? snapshotUser, string activityName, bool exiting)
        {
            int minutes = 0;

            if (snapshotUser != null)
                if (exiting)
                    minutes = snapshotUser.Activities[activityName].Minutes + Util.Date.GetMinutesBetweenDates(DateTime.Now, snapshotUser.Activities[activityName].JoinGame.ToDateTime().ToLocalTime());
                else
                    if (snapshotUser.Activities != null && snapshotUser.Activities.ContainsKey(activityName))
                    minutes = snapshotUser.Activities[activityName].Minutes;

            return minutes;
        }
    }
}
