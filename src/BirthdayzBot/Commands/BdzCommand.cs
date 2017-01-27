﻿using System;
using System.Linq;
using BirthdayzBot.Models;
using NetTelegramBotApi.Requests;
using NetTelegramBotApi.Types;

namespace BirthdayzBot.Commands
{
    public class BdzCommand : BaseBotCommand
    {
        private readonly BirthdayzContext _dbContext;

        public override string Name => _commandName;

        private static readonly string _commandName = "bdz";

        public BdzCommand(Update update, string args = null)
            : base(update, args)
        {
            _dbContext = new BirthdayzContext();
        }

        public override RequestBase<Message> GetResponse()
        {
            var responseText = "";
            var birthdays = _dbContext.Birthdays.Where(birthday => birthday.ChatId == Update.Message.Chat.Id).OrderBy(bd => new DateTime(2000, bd.Birthdate.Month, bd.Birthdate.Day)).ToArray();

            if (birthdays.Length == 0)
            {
                responseText = "I have no idea about birthdayz in this chat :disappointed:\nTell me your birthday please";
            }
            foreach (var birthday in birthdays)
            {
                var mention = string.IsNullOrEmpty(birthday.User.UserName)
                    ? $"@{birthday.User.UserName}"
                    : $"@{birthday.User.Id} ({birthday.User.FirstName + " " + birthday.User.LastName})";
                responseText += $"`{birthday.Birthdate:MMM dd}` {mention}\n";
            }
            return new SendMessage(Update.Message.Chat.Id, responseText)
            {
                DisableNotification = true,
                ReplyToMessageId = Update.Message.MessageId,
                ParseMode = SendMessage.ParseModeEnum.Markdown,
            };
        }
    }
}
